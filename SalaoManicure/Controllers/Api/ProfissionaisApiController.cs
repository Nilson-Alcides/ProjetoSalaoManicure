using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers.Api
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProfissionaisApiController : ControllerBase
    {
        private readonly IProfissionalRepository _profissionalRepository;

        public ProfissionaisApiController(
            IProfissionalRepository profissionalRepository)
        {
            _profissionalRepository = profissionalRepository;
        }

        // GET: api/ProfissionaisApi
        [HttpGet]
        public IActionResult Listar()
        {
            var profissionais = _profissionalRepository.Listar();

            return Ok(profissionais);
        }

        // GET: api/ProfissionaisApi/1
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var profissional =
                _profissionalRepository.BuscarPorId(id);

            if (profissional == null)
            {
                return NotFound(new
                {
                    mensagem = "Profissional não encontrada."
                });
            }

            return Ok(profissional);
        }

        // POST: api/ProfissionaisApi
        [HttpPost]
        public IActionResult Cadastrar(Profissional profissional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _profissionalRepository.Cadastrar(profissional);

            return Ok(new
            {
                mensagem = "Profissional cadastrada com sucesso."
            });
        }

        // PUT: api/ProfissionaisApi/1
        [HttpPut("{id}")]
        public IActionResult Atualizar(
            int id,
            Profissional profissional)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var profissionalExistente =
                _profissionalRepository.BuscarPorId(id);

            if (profissionalExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Profissional não encontrada."
                });
            }

            profissional.Id = id;

            _profissionalRepository.Atualizar(profissional);

            return Ok(new
            {
                mensagem = "Profissional atualizada com sucesso."
            });
        }

        // DELETE: api/ProfissionaisApi/1
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var profissional =
                _profissionalRepository.BuscarPorId(id);

            if (profissional == null)
            {
                return NotFound(new
                {
                    mensagem = "Profissional não encontrada."
                });
            }

            _profissionalRepository.Excluir(id);

            return Ok(new
            {
                mensagem = "Profissional excluída com sucesso."
            });
        }
    }
}
