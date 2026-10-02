
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentosApiController : ControllerBase
    {
        private readonly IAgendamentoRepository _agendamentoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IServicoRepository _servicoRepository;
        private readonly IDisponibilidadeRepository _disponibilidadeRepository;
        private readonly IBloqueioRepository _bloqueioRepository;

        public AgendamentosApiController(
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
        // GET: api/AgendamentosApi
        // LISTAR TODOS
        // ============================================================

        [HttpGet]
        public IActionResult Listar()
        {
            var agendamentos =
                _agendamentoRepository.Listar();

            var resultado =
                agendamentos.Select(a => new
                {
                    id = a.Id,

                    data = a.Data.ToString("yyyy-MM-dd"),

                    horario =
                        a.Horario.ToString(@"hh\:mm"),

                    clienteId =
                        a.ClienteId,

                    cliente =
                        a.Cliente?.Nome,

                    profissionalId =
                        a.ProfissionalId,

                    profissional =
                        a.Profissional?.Nome,

                    servicoId =
                        a.ServicoId,

                    servico =
                        a.Servico?.Nome,

                    preco =
                        a.Servico?.Preco ?? 0,

                    status =
                        a.Status
                })
                .ToList();

            return Ok(resultado);
        }


        // ============================================================
        // GET: api/AgendamentosApi/5
        // BUSCAR POR ID
        // ============================================================

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var agendamento =
                _agendamentoRepository.BuscarPorId(id);

            if (agendamento == null)
            {
                return NotFound(new
                {
                    mensagem = "Agendamento não encontrado."
                });
            }

            var resultado = new
            {
                id = agendamento.Id,

                data =
                    agendamento.Data.ToString("yyyy-MM-dd"),

                horario =
                    agendamento.Horario.ToString(@"hh\:mm"),

                clienteId =
                    agendamento.ClienteId,

                cliente =
                    agendamento.Cliente?.Nome,

                profissionalId =
                    agendamento.ProfissionalId,

                profissional =
                    agendamento.Profissional?.Nome,

                servicoId =
                    agendamento.ServicoId,

                servico =
                    agendamento.Servico?.Nome,

                preco =
                    agendamento.Servico?.Preco ?? 0,

                status =
                    agendamento.Status
            };

            return Ok(resultado);
        }


        // ============================================================
        // POST: api/AgendamentosApi
        // CADASTRAR
        // ============================================================

        [HttpPost]
        public IActionResult Cadastrar(
            [FromBody] Agendamento agendamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // --------------------------------------------------------
            // STATUS PADRÃO
            // --------------------------------------------------------

            agendamento.Status = "Agendado";


            // --------------------------------------------------------
            // VERIFICAR HORÁRIO
            // --------------------------------------------------------

            if (!HorarioEstaDisponivel(agendamento))
            {
                return BadRequest(new
                {
                    mensagem =
                        "O horário selecionado não está disponível."
                });
            }


            // --------------------------------------------------------
            // VERIFICAR SERVIÇO
            // --------------------------------------------------------

            var servico =
                _servicoRepository
                    .Listar()
                    .FirstOrDefault(s =>
                        s.Id == agendamento.ServicoId &&
                        s.Ativo);

            if (servico == null)
            {
                return BadRequest(new
                {
                    mensagem =
                        "O serviço selecionado não existe ou está inativo."
                });
            }


            // --------------------------------------------------------
            // CADASTRAR
            // --------------------------------------------------------

            _agendamentoRepository.Cadastrar(
                agendamento);


            return CreatedAtAction(
                nameof(BuscarPorId),
                new
                {
                    id = agendamento.Id
                },
                new
                {
                    mensagem =
                        "Agendamento realizado com sucesso.",

                    id =
                        agendamento.Id,

                    status =
                        agendamento.Status
                });
        }


        // ============================================================
        // PUT: api/AgendamentosApi/5
        // ATUALIZAR
        // ============================================================

        [HttpPut("{id}")]
        public IActionResult Atualizar(
            int id,
            [FromBody] Agendamento agendamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            // --------------------------------------------------------
            // BUSCAR AGENDAMENTO
            // --------------------------------------------------------

            var existente =
                _agendamentoRepository.BuscarPorId(id);

            if (existente == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Agendamento não encontrado."
                });
            }


            // --------------------------------------------------------
            // GARANTIR ID
            // --------------------------------------------------------

            agendamento.Id = id;


            // --------------------------------------------------------
            // NÃO PERMITIR ALTERAÇÃO AUTOMÁTICA DO STATUS
            // --------------------------------------------------------

            if (string.IsNullOrWhiteSpace(
                agendamento.Status))
            {
                agendamento.Status =
                    existente.Status;
            }


            // --------------------------------------------------------
            // VERIFICAR HORÁRIO
            // --------------------------------------------------------

            if (!HorarioEstaDisponivel(
                    agendamento,
                    id))
            {
                return BadRequest(new
                {
                    mensagem =
                        "O horário selecionado não está disponível."
                });
            }


            // --------------------------------------------------------
            // VERIFICAR SERVIÇO
            // --------------------------------------------------------

            var servico =
                _servicoRepository
                    .Listar()
                    .FirstOrDefault(s =>
                        s.Id == agendamento.ServicoId &&
                        s.Ativo);

            if (servico == null)
            {
                return BadRequest(new
                {
                    mensagem =
                        "O serviço selecionado não existe ou está inativo."
                });
            }


            // --------------------------------------------------------
            // ATUALIZAR
            // --------------------------------------------------------

            _agendamentoRepository.Atualizar(
                agendamento);


            return Ok(new
            {
                mensagem =
                    "Agendamento atualizado com sucesso."
            });
        }


        // ============================================================
        // DELETE: api/AgendamentosApi/5
        // EXCLUIR
        // ============================================================

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var agendamento =
                _agendamentoRepository.BuscarPorId(id);

            if (agendamento == null)
            {
                return NotFound(new
                {
                    mensagem =
                        "Agendamento não encontrado."
                });
            }


            _agendamentoRepository.Excluir(id);


            return Ok(new
            {
                mensagem =
                    "Agendamento excluído com sucesso."
            });
        }


        // ============================================================
        // GET:
        // api/AgendamentosApi/horarios-disponiveis
        // ============================================================

        [HttpGet("horarios-disponiveis")]
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
                return BadRequest(new
                {
                    mensagem =
                        "Profissional inválido."
                });
            }


            // --------------------------------------------------------
            // 2. DISPONIBILIDADES
            // --------------------------------------------------------

            var disponibilidades =
                _disponibilidadeRepository.Listar()
                    .Where(d =>
                        d.ProfissionalId ==
                            profissionalId &&

                        d.Data.Date ==
                            data.Date &&

                        d.Ativo)
                    .OrderBy(d => d.Horario)
                    .ToList();


            if (!disponibilidades.Any())
            {
                return Ok(new List<object>());
            }


            // --------------------------------------------------------
            // 3. BLOQUEIOS
            // --------------------------------------------------------

            var bloqueios =
                _bloqueioRepository.Listar()
                    .Where(b =>
                        b.ProfissionalId ==
                            profissionalId &&

                        b.Data.Date ==
                            data.Date &&

                        b.Ativo)
                    .ToList();


            // --------------------------------------------------------
            // 4. DIA TODO BLOQUEADO
            // --------------------------------------------------------

            if (bloqueios.Any(
                b => !b.Horario.HasValue))
            {
                return Ok(new List<object>());
            }


            // --------------------------------------------------------
            // 5. AGENDAMENTOS EXISTENTES
            // --------------------------------------------------------

            var agendamentos =
                _agendamentoRepository.Listar()
                    .Where(a =>
                        a.ProfissionalId ==
                            profissionalId &&

                        a.Data.Date ==
                            data.Date &&

                        a.Status != "Cancelado" &&

                        (
                            !agendamentoId.HasValue ||
                            a.Id != agendamentoId.Value
                        ))
                    .ToList();


            // --------------------------------------------------------
            // 6. FILTRAR HORÁRIOS
            // --------------------------------------------------------

            var horarios =
                disponibilidades
                    .Where(d =>

                        !bloqueios.Any(b =>
                            b.Horario.HasValue &&
                            b.Horario.Value ==
                                d.Horario)

                        &&

                        !agendamentos.Any(a =>
                            a.Horario ==
                                d.Horario)

                    )
                    .Select(d => new
                    {
                        valor =
                            d.Horario.ToString(
                                @"hh\:mm"),

                        texto =
                            d.Horario.ToString(
                                @"hh\:mm")
                    })
                    .ToList();


            return Ok(horarios);
        }


        // ============================================================
        // VERIFICAR HORÁRIO
        // ============================================================

        private bool HorarioEstaDisponivel(
            Agendamento agendamento,
            int? idIgnorar = null)
        {
            // --------------------------------------------------------
            // 1. DISPONIBILIDADE
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

                        d.Ativo);


            if (disponibilidade == null)
            {
                return false;
            }


            // --------------------------------------------------------
            // 2. BLOQUEIOS
            // --------------------------------------------------------

            var bloqueios =
                _bloqueioRepository.Listar()
                    .Where(b =>
                        b.ProfissionalId ==
                            agendamento.ProfissionalId &&

                        b.Data.Date ==
                            agendamento.Data.Date &&

                        b.Ativo)
                    .ToList();


            // --------------------------------------------------------
            // 3. DIA INTEIRO BLOQUEADO
            // --------------------------------------------------------

            if (bloqueios.Any(
                b => !b.Horario.HasValue))
            {
                return false;
            }


            // --------------------------------------------------------
            // 4. HORÁRIO BLOQUEADO
            // --------------------------------------------------------

            if (bloqueios.Any(b =>
                b.Horario.HasValue &&
                b.Horario.Value ==
                    agendamento.Horario))
            {
                return false;
            }


            // --------------------------------------------------------
            // 5. OUTROS AGENDAMENTOS
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

                        a.Status != "Cancelado")
                    .ToList();


            // --------------------------------------------------------
            // 6. IGNORAR O PRÓPRIO AGENDAMENTO
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
            // 7. VERIFICAR CONFLITO
            // --------------------------------------------------------

            if (agendamentos.Any())
            {
                return false;
            }


            return true;
        }
    }
}
