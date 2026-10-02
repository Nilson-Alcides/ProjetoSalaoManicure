
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloqueiosApiController : ControllerBase
    {
        private readonly IBloqueioRepository _bloqueioRepository;

        public BloqueiosApiController(
            IBloqueioRepository bloqueioRepository)
        {
            _bloqueioRepository = bloqueioRepository;
        }

        // GET: api/BloqueiosApi
        [HttpGet]
        public IActionResult Listar()
        {
            var bloqueios =
                _bloqueioRepository.Listar();

            return Ok(bloqueios);
        }

        // GET: api/BloqueiosApi/1
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var bloqueio =
                _bloqueioRepository.BuscarPorId(id);

            if (bloqueio == null)
            {
                return NotFound(new
                {
                    mensagem = "Bloqueio não encontrado."
                });
            }

            return Ok(bloqueio);
        }

        // POST: api/BloqueiosApi
        [HttpPost]
        public IActionResult Cadastrar(Bloqueio bloqueio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _bloqueioRepository.Cadastrar(bloqueio);

            return Ok(new
            {
                mensagem = "Bloqueio cadastrado com sucesso."
            });
        }

        // PUT: api/BloqueiosApi/1
        [HttpPut("{id}")]
        public IActionResult Atualizar(
            int id,
            Bloqueio bloqueio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bloqueioExistente =
                _bloqueioRepository.BuscarPorId(id);

            if (bloqueioExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Bloqueio não encontrado."
                });
            }

            bloqueio.Id = id;

            _bloqueioRepository.Atualizar(bloqueio);

            return Ok(new
            {
                mensagem = "Bloqueio atualizado com sucesso."
            });
        }

        // DELETE: api/BloqueiosApi/1
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var bloqueio =
                _bloqueioRepository.BuscarPorId(id);

            if (bloqueio == null)
            {
                return NotFound(new
                {
                    mensagem = "Bloqueio não encontrado."
                });
            }

            _bloqueioRepository.Excluir(id);

            return Ok(new
            {
                mensagem = "Bloqueio excluído com sucesso."
            });
        }
    }
}

