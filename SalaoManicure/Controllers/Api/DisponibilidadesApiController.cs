
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilidadesApiController : ControllerBase
    {
        private readonly IDisponibilidadeRepository _disponibilidadeRepository;

        public DisponibilidadesApiController(
            IDisponibilidadeRepository disponibilidadeRepository)
        {
            _disponibilidadeRepository = disponibilidadeRepository;
        }

        // ============================================================
        // LISTAR TODAS
        // ============================================================

        [HttpGet]
        public IActionResult Listar()
        {
            var disponibilidades =
                _disponibilidadeRepository.Listar();

            return Ok(disponibilidades);
        }

        // ============================================================
        // BUSCAR POR ID
        // ============================================================

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var disponibilidade =
                _disponibilidadeRepository.BuscarPorId(id);

            if (disponibilidade == null)
            {
                return NotFound(new
                {
                    mensagem = "Disponibilidade não encontrada."
                });
            }

            return Ok(disponibilidade);
        }

        // ============================================================
        // CADASTRAR
        // ============================================================

        [HttpPost]
        public IActionResult Cadastrar(
            Disponibilidade disponibilidade)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _disponibilidadeRepository.Cadastrar(
                disponibilidade);

            return Ok(new
            {
                mensagem = "Disponibilidade cadastrada com sucesso."
            });
        }

        // ============================================================
        // ATUALIZAR
        // ============================================================

        [HttpPut("{id}")]
        public IActionResult Atualizar(
            int id,
            Disponibilidade disponibilidade)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var disponibilidadeExistente =
                _disponibilidadeRepository.BuscarPorId(id);

            if (disponibilidadeExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Disponibilidade não encontrada."
                });
            }

            disponibilidade.Id = id;

            _disponibilidadeRepository.Atualizar(
                disponibilidade);

            return Ok(new
            {
                mensagem = "Disponibilidade atualizada com sucesso."
            });
        }

        // ============================================================
        // EXCLUIR
        // ============================================================

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var disponibilidade =
                _disponibilidadeRepository.BuscarPorId(id);

            if (disponibilidade == null)
            {
                return NotFound(new
                {
                    mensagem = "Disponibilidade não encontrada."
                });
            }

            _disponibilidadeRepository.Excluir(id);

            return Ok(new
            {
                mensagem = "Disponibilidade excluída com sucesso."
            });
        }

        // ============================================================
        // HORÁRIOS DISPONÍVEIS POR PROFISSIONAL E DATA
        // ============================================================

        [HttpGet("profissional/{profissionalId}/data/{data}")]
        public IActionResult HorariosPorData(
            int profissionalId,
            DateTime data)
        {
            var disponibilidades =
                _disponibilidadeRepository.Listar()
                    .Where(d =>
                        d.ProfissionalId == profissionalId &&
                        d.Data.Date == data.Date &&
                        d.Ativo)
                    .OrderBy(d => d.Horario)
                    .ToList();

            return Ok(disponibilidades);
        }
    }
}

