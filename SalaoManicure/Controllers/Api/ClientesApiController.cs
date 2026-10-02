using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesApiController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesApiController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // GET: api/ClientesApi
        [HttpGet]
        public IActionResult Listar()
        {
            var clientes = _clienteRepository.Listar();

            return Ok(clientes);
        }

        // GET: api/ClientesApi/1
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var cliente = _clienteRepository.BuscarPorId(id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensagem = "Cliente não encontrado."
                });
            }

            return Ok(cliente);
        }

        // POST: api/ClientesApi
        [HttpPost]
        public IActionResult Cadastrar(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            cliente.DataCadastro = DateTime.Now;

            _clienteRepository.Cadastrar(cliente);

            return Ok(new
            {
                mensagem = "Cliente cadastrado com sucesso."
            });
        }

        // PUT: api/ClientesApi/1
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var clienteExistente = _clienteRepository.BuscarPorId(id);

            if (clienteExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Cliente não encontrado."
                });
            }

            cliente.Id = id;

            _clienteRepository.Atualizar(cliente);

            return Ok(new
            {
                mensagem = "Cliente atualizado com sucesso."
            });
        }

        // DELETE: api/ClientesApi/1
        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            var cliente = _clienteRepository.BuscarPorId(id);

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensagem = "Cliente não encontrado."
                });
            }

            _clienteRepository.Excluir(id);

            return Ok(new
            {
                mensagem = "Cliente excluído com sucesso."
            });
        }
    }
}

