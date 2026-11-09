
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalaoManicure.Libraries.Filtro;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;
using SalaoManicure.Services.Contract;

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
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly IMercadoPagoService _mercadoPagoService;
        private readonly IConfiguration _configuration;


        public AgendamentoClienteController(
            IAgendamentoRepository agendamentoRepository,
            IClienteRepository clienteRepository,
            IProfissionalRepository profissionalRepository,
            IServicoRepository servicoRepository,
            IDisponibilidadeRepository disponibilidadeRepository,
            IBloqueioRepository bloqueioRepository,
            IPagamentoRepository pagamentoRepository,
            IMercadoPagoService mercadoPagoService,
            IConfiguration configuration
            )   
        {
            _agendamentoRepository = agendamentoRepository;
            _clienteRepository = clienteRepository;
            _profissionalRepository = profissionalRepository;
            _servicoRepository = servicoRepository;
            _disponibilidadeRepository = disponibilidadeRepository;
            _bloqueioRepository = bloqueioRepository;
            _pagamentoRepository = pagamentoRepository;
            _mercadoPagoService = mercadoPagoService;
            _configuration = configuration;
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
                Status = "PendentePagamento"
            };

            return View(agendamento);
        }

        // ============================================================
        // CADASTRAR - POST
        // ============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(Agendamento agendamento)
        {
            // 1. Verificar se o cliente está logado
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var cliente = _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
                return RedirectToAction("Index", "ClienteLogado");

            // 2. O cliente e o status são definidos pelo servidor
            agendamento.ClienteId = cliente.Id;
            agendamento.Status = "PendentePagamento";

            ModelState.Remove(nameof(Agendamento.ClienteId));
            ModelState.Remove(nameof(Agendamento.Status));

            // 3. Validar data
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

            // 4. Verificar disponibilidade do horário
            if (!HorarioEstaDisponivel(agendamento))
            {
                ModelState.AddModelError(
                    nameof(Agendamento.Horario),
                    "O horário selecionado não está disponível. Escolha outro horário.");

                CarregarDados();
                return View(agendamento);
            }

            // 5. Buscar o serviço selecionado
            var servico = _servicoRepository.Listar()
                .FirstOrDefault(s =>
                    s.Id == agendamento.ServicoId && s.Ativo);

            if (servico == null)
            {
                ModelState.AddModelError(
                    nameof(Agendamento.ServicoId),
                    "Serviço não encontrado ou indisponível.");

                CarregarDados();
                return View(agendamento);
            }

            // 6. Calcular o sinal de 30%
            decimal valorSinal = Math.Round(
                servico.Preco * 0.30m,
                2,
                MidpointRounding.AwayFromZero);

            // 7. Salvar o agendamento
            int agendamentoId = _agendamentoRepository.Cadastrar(agendamento);

            agendamento.Id = agendamentoId;

            // 8. Criar registro de pagamento
            int pagamentoId = 0;

            try
            {
                var pagamento = new Pagamento
                {
                    AgendamentoId = agendamentoId,
                    Valor = valorSinal,
                    Percentual = 30.00m,
                    Gateway = "MercadoPago",
                    Status = "Pendente"
                };

                pagamentoId = _pagamentoRepository.Cadastrar(pagamento);

                // 9. Preparar URLs públicas de retorno
                string baseUrl = _configuration["App:PublicBaseUrl"]?
                    .TrimEnd('/')
                    ?? throw new InvalidOperationException(
                        "URL pública do sistema não configurada.");

                string urlSucesso =
                     $"{baseUrl}/AgendamentoCliente/PagamentoSucesso?agendamentoId={agendamentoId}";

                string urlFalha =
                    $"{baseUrl}/AgendamentoCliente/PagamentoFalha?agendamentoId={agendamentoId}";

                string urlPendente =
                    $"{baseUrl}/AgendamentoCliente/PagamentoPendente?agendamentoId={agendamentoId}";

                // 10. Criar Order no Mercado Pago
                var resultado = await _mercadoPagoService.CriarOrderAsync(
                    agendamentoId,
                    servico.Nome,
                    valorSinal,
                    cliente.Email,
                    cliente.Nome,
                    urlSucesso,
                    urlFalha,
                    urlPendente

                );


                // 11. Atualizar pagamento com os dados do checkout
                _pagamentoRepository.AtualizarCheckout(
                    pagamentoId,
                    resultado.OrderId,
                    resultado.CheckoutUrl,
                    "Pendente"
                );

                // 12. Redirecionar para o Mercado Pago
                return Redirect(resultado.CheckoutUrl);
            }
            catch (Exception ex)
            {
                // 13. Registrar falha no pagamento
                if (pagamentoId > 0)
                {
                    _pagamentoRepository.AtualizarStatus(
                        pagamentoId,
                        "Falha");
                }

                // 14. Cancelar agendamento para liberar o horário
                agendamento.Status = "Cancelado";

                _agendamentoRepository.Atualizar(agendamento);

                TempData["Erro"] =
                    "Não foi possível iniciar o pagamento. Tente novamente. " +
                    ex.Message;

                return RedirectToAction(nameof(MeusAgendamentos));
            }
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
            var profissionais = _profissionalRepository.Listar();
            var servicos = _servicoRepository.Listar();

            ViewBag.Profissionais = profissionais
                .Where(p => p.Ativo)
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Nome
                })
                .ToList();

            var servicosAtivos = servicos
                .Where(s => s.Ativo)
                .ToList();

            ViewBag.Servicos = servicosAtivos
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{s.Nome} - {s.Preco:C2}"
                })
                .ToList();

            // Dados utilizados pelo JavaScript para calcular o sinal
            ViewBag.ServicosDados = servicosAtivos;
        }

        // ============================================================
        // RETORNO DO MERCADO PAGO
        // ============================================================

       [HttpGet]
        public IActionResult PagamentoSucesso(int agendamentoId)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Login", "Auth");

            var cliente = _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
                return RedirectToAction("Index", "ClienteLogado");

            var agendamento = _agendamentoRepository.BuscarPorId(agendamentoId);

            if (agendamento == null || agendamento.ClienteId != cliente.Id)
                return NotFound();

            var pagamento = _pagamentoRepository.BuscarPorAgendamentoId(agendamentoId);

            if (pagamento == null)
                return NotFound();

            agendamento.Pagamento = pagamento;

            return View(agendamento);
        }

        [HttpGet]
        public IActionResult PagamentoFalha(int agendamentoId)
        {
            return RedirectToAction(
                nameof(PagamentoSucesso),
                new { agendamentoId });
        }

        [HttpGet]
        public IActionResult PagamentoPendente(int agendamentoId)
        {
            return RedirectToAction(
                nameof(PagamentoSucesso),
                new { agendamentoId });
        }

        [HttpGet]
        public IActionResult PagamentoPendente()
        {
            TempData["Aviso"] =
                "Seu pagamento está aguardando confirmação do Mercado Pago.";

            return RedirectToAction(nameof(MeusAgendamentos));
        }
    }
}
