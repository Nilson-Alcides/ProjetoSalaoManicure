
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    public class AgendamentosController : Controller
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IServicoRepository _servicoRepository;
        private readonly IDisponibilidadeRepository _disponibilidadeRepository;
        private readonly IBloqueioRepository _bloqueioRepository;

        public AgendamentosController(
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
        // LISTAR
        // ============================================================

        public IActionResult Index()
        {
            var agendamentos =
                _agendamentoRepository.Listar();

            return View(agendamentos);
        }


        // ============================================================
        // CADASTRAR - GET
        // ============================================================

        [HttpGet]
        public IActionResult Cadastrar()
        {
            CarregarDados();

            return View();
        }


        // ============================================================
        // CADASTRAR - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(
            Agendamento agendamento)
        {
            // Verifica os dados recebidos
            if (!ModelState.IsValid)
            {
                CarregarDados();

                return View(agendamento);
            }


            // Verifica se o horário realmente está disponível
            if (!HorarioEstaDisponivel(agendamento))
            {
                ModelState.AddModelError(
                    "Horario",
                    "O horário selecionado não está disponível."
                );

                CarregarDados();

                return View(agendamento);
            }


            // Cadastra o agendamento
            _agendamentoRepository.Cadastrar(
                agendamento);

            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // EDITAR - GET
        // ============================================================

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var agendamento =
                _agendamentoRepository.BuscarPorId(id);

            if (agendamento == null)
            {
                return NotFound();
            }

            CarregarDados();

            return View(agendamento);
        }


        // ============================================================
        // EDITAR - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(
            Agendamento agendamento)
        {
            // Verifica os dados recebidos
            if (!ModelState.IsValid)
            {
                CarregarDados();

                return View(agendamento);
            }


            // Busca o agendamento original
            var agendamentoExistente =
                _agendamentoRepository.BuscarPorId(
                    agendamento.Id);

            if (agendamentoExistente == null)
            {
                return NotFound();
            }


            // Verifica se o novo horário está disponível.
            // O próprio agendamento é ignorado
            // durante essa verificação.
            if (!HorarioEstaDisponivel(
                    agendamento,
                    agendamento.Id))
            {
                ModelState.AddModelError(
                    "Horario",
                    "O horário selecionado não está disponível."
                );

                CarregarDados();

                return View(agendamento);
            }


            // Atualiza o agendamento
            _agendamentoRepository.Atualizar(
                agendamento);

            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // EXCLUIR - GET
        // ============================================================

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var agendamento =
                _agendamentoRepository.BuscarPorId(id);

            if (agendamento == null)
            {
                return NotFound();
            }

            return View(agendamento);
        }


        // ============================================================
        // EXCLUIR - POST
        // ============================================================

        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExclusao(int id)
        {
            var agendamento =
                _agendamentoRepository.BuscarPorId(id);

            if (agendamento == null)
            {
                return NotFound();
            }


            _agendamentoRepository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }


        // ============================================================
        // HORÁRIOS DISPONÍVEIS
        // ============================================================

        [HttpGet]
        public IActionResult HorariosDisponiveis(
            int profissionalId,
            DateTime data,
            int? agendamentoId = null)
        {
            // --------------------------------------------------------
            // 1. VALIDAR PROFISSIONAL
            // --------------------------------------------------------

            if (profissionalId <= 0)
            {
                return Json(new List<object>());
            }


            // --------------------------------------------------------
            // 2. BUSCAR DISPONIBILIDADES DA DATA
            // --------------------------------------------------------

            var disponibilidades =
                _disponibilidadeRepository.Listar()
                    .Where(d =>
                        d.ProfissionalId == profissionalId &&
                        d.Data.Date == data.Date &&
                        d.Ativo)
                    .OrderBy(d => d.Horario)
                    .ToList();


            // Não existem horários cadastrados
            if (!disponibilidades.Any())
            {
                return Json(new List<object>());
            }


            // --------------------------------------------------------
            // 3. BUSCAR BLOQUEIOS DA DATA
            // --------------------------------------------------------

            var bloqueios =
                _bloqueioRepository.Listar()
                    .Where(b =>
                        b.ProfissionalId == profissionalId &&
                        b.Data.Date == data.Date &&
                        b.Ativo)
                    .ToList();


            // --------------------------------------------------------
            // 4. VERIFICAR SE O DIA INTEIRO ESTÁ BLOQUEADO
            // --------------------------------------------------------

            bool diaBloqueado =
                bloqueios.Any(
                    b => !b.Horario.HasValue);

            if (diaBloqueado)
            {
                return Json(new List<object>());
            }


            // --------------------------------------------------------
            // 5. BUSCAR AGENDAMENTOS EXISTENTES
            // --------------------------------------------------------

            var agendamentos =
                _agendamentoRepository.Listar()
                    .Where(a =>
                        a.ProfissionalId == profissionalId &&
                        a.Data.Date == data.Date &&
                        a.Status != "Cancelado" &&

                        // Se estiver editando,
                        // ignora o próprio agendamento.
                        (
                            !agendamentoId.HasValue ||
                            a.Id != agendamentoId.Value
                        ))
                    .ToList();


            // --------------------------------------------------------
            // 6. FILTRAR HORÁRIOS DISPONÍVEIS
            // --------------------------------------------------------

            var horariosDisponiveis =
                disponibilidades
                    .Where(d =>

                        // Não está bloqueado
                        !bloqueios.Any(b =>
                            b.Horario.HasValue &&
                            b.Horario.Value == d.Horario
                        )

                        &&

                        // Não está ocupado por outro agendamento
                        !agendamentos.Any(a =>
                            a.Horario == d.Horario
                        )

                    )
                    .Select(d => new
                    {
                        valor =
                            d.Horario.ToString(@"hh\:mm"),

                        texto =
                            d.Horario.ToString(@"hh\:mm")
                    })
                    .ToList();


            return Json(horariosDisponiveis);
        }


        // ============================================================
        // VERIFICAR SE O HORÁRIO ESTÁ DISPONÍVEL
        // ============================================================

        private bool HorarioEstaDisponivel(
            Agendamento agendamento,
            int? idIgnorar = null)
        {
            // --------------------------------------------------------
            // 1. VERIFICAR SE EXISTE DISPONIBILIDADE
            // --------------------------------------------------------

            var disponibilidade =
                _disponibilidadeRepository.Listar()
                    .FirstOrDefault(d =>
                        d.ProfissionalId ==
                            agendamento.ProfissionalId &&

                        d.Data.Date ==
                            agendamento.Data.Date &&

                        d.Horario ==
                            agendamento.Horario &&

                        d.Ativo
                    );


            if (disponibilidade == null)
            {
                return false;
            }


            // --------------------------------------------------------
            // 2. BUSCAR BLOQUEIOS
            // --------------------------------------------------------

            var bloqueios =
                _bloqueioRepository.Listar()
                    .Where(b =>
                        b.ProfissionalId ==
                            agendamento.ProfissionalId &&

                        b.Data.Date ==
                            agendamento.Data.Date &&

                        b.Ativo
                    )
                    .ToList();


            // --------------------------------------------------------
            // 3. VERIFICAR BLOQUEIO DO DIA INTEIRO
            // --------------------------------------------------------

            if (bloqueios.Any(
                b => !b.Horario.HasValue))
            {
                return false;
            }


            // --------------------------------------------------------
            // 4. VERIFICAR BLOQUEIO DO HORÁRIO
            // --------------------------------------------------------

            if (bloqueios.Any(
                b =>
                    b.Horario.HasValue &&
                    b.Horario.Value ==
                        agendamento.Horario))
            {
                return false;
            }


            // --------------------------------------------------------
            // 5. BUSCAR OUTROS AGENDAMENTOS
            // --------------------------------------------------------

            var agendamentos =
                _agendamentoRepository.Listar()
                    .Where(a =>
                        a.ProfissionalId ==
                            agendamento.ProfissionalId &&

                        a.Data.Date ==
                            agendamento.Data.Date &&

                        a.Horario ==
                            agendamento.Horario &&

                        a.Status != "Cancelado"
                    )
                    .ToList();


            // --------------------------------------------------------
            // 6. IGNORAR O PRÓPRIO AGENDAMENTO NA EDIÇÃO
            // --------------------------------------------------------

            if (idIgnorar.HasValue)
            {
                agendamentos =
                    agendamentos
                        .Where(a =>
                            a.Id != idIgnorar.Value)
                        .ToList();
            }


            // --------------------------------------------------------
            // 7. VERIFICAR SE EXISTE OUTRO AGENDAMENTO
            // --------------------------------------------------------

            if (agendamentos.Any())
            {
                return false;
            }


            return true;
        }

        // ============================================================
        // CARREGAR DADOS PARA AS VIEWS
        // ============================================================


        private void CarregarDados()
        {
            // CLIENTES
            ViewBag.Clientes =
                _clienteRepository.Listar()
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Nome
                    })
                    .ToList();


            // PROFISSIONAIS
            ViewBag.Profissionais =
                _profissionalRepository.Listar()
                    .Where(p => p.Ativo)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = p.Nome
                    })
                    .ToList();


            // SERVIÇOS
            // SERVIÇOS
            ViewBag.Servicos =
                _servicoRepository.Listar()
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

