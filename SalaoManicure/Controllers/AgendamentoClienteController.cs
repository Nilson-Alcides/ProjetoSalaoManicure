
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    public class AgendamentoClienteController : Controller
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IServicoRepository _servicoRepository;
        private readonly IDisponibilidadeRepository _disponibilidadeRepository;
        private readonly IBloqueioRepository _bloqueioRepository;

        public AgendamentoClienteController(
            IAgendamentoRepository agendamentoRepository,
            IClienteRepository clienteRepository,
            IProfissionalRepository profissionalRepository,
            IServicoRepository servicoRepository,
            IDisponibilidadeRepository disponibilidadeRepository,
            IBloqueioRepository bloqueioRepository)
        {
            _agendamentoRepository = agendamentoRepository;
            _clienteRepository = clienteRepository;
            _profissionalRepository = profissionalRepository;
            _servicoRepository = servicoRepository;
            _disponibilidadeRepository = disponibilidadeRepository;
            _bloqueioRepository = bloqueioRepository;
        }

        // ============================================================
        // MEUS AGENDAMENTOS
        // ============================================================

        [HttpGet]
        public IActionResult MeusAgendamentos()
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
                return RedirectToAction("Index", "ClienteLogado");

            var agendamentos =
                _agendamentoRepository.ListarPorCliente(cliente.Id);

            return View(agendamentos);
        }

        // ============================================================
        // CADASTRAR - GET
        // ============================================================

        [HttpGet]
        public IActionResult Cadastrar()
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
                return RedirectToAction("Index", "ClienteLogado");

            CarregarDados();

            var agendamento = new Agendamento
            {
                Data = DateTime.Today,
                ClienteId = cliente.Id,
                Status = "Agendado"
            };

            return View(agendamento);
        }

        // ============================================================
        // CADASTRAR - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Agendamento agendamento)
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
                return RedirectToAction("Index", "ClienteLogado");

            // O cliente é definido pelo servidor, nunca pelo formulário.
            agendamento.ClienteId = cliente.Id;
            agendamento.Status = "Agendado";

            ModelState.Remove(nameof(Agendamento.ClienteId));
            ModelState.Remove(nameof(Agendamento.Status));

            // Não permite agendamentos em datas passadas.
            if (agendamento.Data.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(Agendamento.Data),
                    "Não é possível agendar para uma data passada.");
            }

            if (!ModelState.IsValid)
            {
                CarregarDados();
                return View(agendamento);
            }

            // Confere novamente a disponibilidade no servidor.
            if (!HorarioEstaDisponivel(agendamento))
            {
                ModelState.AddModelError(
                    nameof(Agendamento.Horario),
                    "O horário selecionado não está disponível. Escolha outro horário.");

                CarregarDados();
                return View(agendamento);
            }

            _agendamentoRepository.Cadastrar(agendamento);

            TempData["Sucesso"] =
                "Agendamento realizado com sucesso!";

            return RedirectToAction(nameof(MeusAgendamentos));
        }

        // ============================================================
        // HORÁRIOS DISPONÍVEIS
        // Mesma lógica do AgendamentosController
        // ============================================================

        [HttpGet]
        public IActionResult HorariosDisponiveis(
            int profissionalId,
            DateTime data)
        {
            // 1. Disponibilidades cadastradas para a data.
            var disponibilidades = _disponibilidadeRepository.Listar()
                .Where(d =>
                    d.Data.Date == data.Date &&
                    d.Disponivel)
                .OrderBy(d => d.Horario)
                .ToList();

            if (!disponibilidades.Any())
                return Json(new List<object>());

            // 2. Bloqueios do profissional.
            var bloqueios = _bloqueioRepository.Listar()
                .Where(b =>
                    b.ProfissionalId == profissionalId &&
                    b.Data.Date == data.Date &&
                    b.Ativo)
                .ToList();

            // 3. Bloqueio do dia inteiro.
            if (bloqueios.Any(b => !b.Horario.HasValue))
                return Json(new List<object>());

            // 4. Agendamentos existentes.
            var agendamentos = _agendamentoRepository.Listar()
                .Where(a =>
                    a.ProfissionalId == profissionalId &&
                    a.Data.Date == data.Date &&
                    a.Status != "Cancelado")
                .ToList();

            // 5. Retorna apenas os horários livres.
            var horariosDisponiveis = disponibilidades
                .Where(d =>
                    !bloqueios.Any(b =>
                        b.Horario.HasValue &&
                        b.Horario.Value == d.Horario) &&
                    !agendamentos.Any(a =>
                        a.Horario == d.Horario))
                .Select(d => new
                {
                    valor = d.Horario.ToString(@"hh\:mm"),
                    texto = d.Horario.ToString(@"hh\:mm")
                })
                .ToList();

            return Json(horariosDisponiveis);
        }

        // ============================================================
        // VALIDAR DISPONIBILIDADE ANTES DE SALVAR
        // ============================================================

        private bool HorarioEstaDisponivel(Agendamento agendamento)
        {
            // 1. Verifica se o horário existe na tabela disponibilidade.
            var disponibilidade = _disponibilidadeRepository.Listar()
                .FirstOrDefault(d =>
                    d.Data.Date == agendamento.Data.Date &&
                    d.Horario == agendamento.Horario &&
                    d.Disponivel);

            if (disponibilidade == null)
                return false;

            // 2. Busca bloqueios do profissional na data.
            var bloqueios = _bloqueioRepository.Listar()
                .Where(b =>
                    b.ProfissionalId == agendamento.ProfissionalId &&
                    b.Data.Date == agendamento.Data.Date &&
                    b.Ativo)
                .ToList();

            // 3. Verifica bloqueio do dia inteiro.
            if (bloqueios.Any(b => !b.Horario.HasValue))
                return false;

            // 4. Verifica bloqueio do horário escolhido.
            if (bloqueios.Any(b =>
                b.Horario.HasValue &&
                b.Horario.Value == agendamento.Horario))
            {
                return false;
            }

            // 5. Verifica conflito com outros agendamentos.
            var conflito = _agendamentoRepository.Listar()
                .Any(a =>
                    a.ProfissionalId == agendamento.ProfissionalId &&
                    a.Data.Date == agendamento.Data.Date &&
                    a.Horario == agendamento.Horario &&
                    a.Status != "Cancelado");

            return !conflito;
        }

        // ============================================================
        // CARREGAR PROFISSIONAIS E SERVIÇOS
        // ============================================================

        private void CarregarDados()
        {
            ViewBag.Profissionais = _profissionalRepository.Listar()
                .Where(p => p.Ativo)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                })
                .ToList();

            ViewBag.Servicos = _servicoRepository.Listar()
                .Where(s => s.Ativo)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Nome} - {s.Preco:C2}"
                })
                .ToList();
        }
    }
}
