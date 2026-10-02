
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicosApiController : ControllerBase
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicosApiController(
            IServicoRepository servicoRepository)
        {
            _servicoRepository = servicoRepository;
        }

        // GET: api/ServicosApi
        [HttpGet]
        public IActionResult Listar()
        {
            var servicos = _servicoRepository.Listar();

            return Ok(servicos);
        }

        // GET: api/ServicosApi/1
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var servico = _servicoRepository.BuscarPorId(id);

            if (servico == null)
            {
                return NotFound(new
                {
                    mensagem = "Serviço não encontrado."
                });
            }

            return Ok(servico);
        }

        // POST: api/ServicosApi
        [HttpPost]
        public IActionResult Cadastrar(Servico servico)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _servicoRepository.Cadastrar(servico);

            return Ok(new
            {
                mensagem = "Serviço cadastrado com sucesso."
            });
        }

        // PUT: api/ServicosApi/1
        [HttpPut("{id}")]
        public IActionResult Atualizar(
            int id,
            Servico servico)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var servicoExistente =
                _servicoRepository.BuscarPorId(id);

            if (servicoExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Serviço não encontrado."
                });
            }

            servico.Id = id;

            _servicoRepository.Atualizar(servico);

            return Ok(new
            {
                mensagem = "Serviço atualizado com sucesso."
            });
        }

        // DELETE: api/ServicosApi/1
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var servico = _servicoRepository.BuscarPorId(id);

            if (servico == null)
            {
                return NotFound(new
                {
                    mensagem = "Serviço não encontrado."
                });
            }

            _servicoRepository.Excluir(id);

            return Ok(new
            {
                mensagem = "Serviço excluído com sucesso."
            });
        }
    }
}

