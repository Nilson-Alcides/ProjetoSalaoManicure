
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repositories;
using SalaoManicure.Repository.Contract;
using System;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace SalaoManicure.Controllers
{
    [ApiController]
    [Route("api/cliente")]
    [Authorize]
    public class ClienteLogadoApiController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ClienteLogadoApiController(
            IClienteRepository clienteRepository,
            IUsuarioRepository usuarioRepository)
        {
            _clienteRepository = clienteRepository;
            _usuarioRepository = usuarioRepository;
        }

        // ============================================================
        // GET: api/cliente/me
        // Retorna os dados do cliente logado
        // ============================================================
        [HttpGet("me")]
        public IActionResult Me()
        {
            int? usuarioId = ObterUsuarioId();

            if (usuarioId == null)
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não identificado."
                });
            }

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
            {
                return NotFound(new
                {
                    mensagem = "Cadastro do cliente não encontrado."
                });
            }

            return Ok(new
            {
                cliente.Id,
                cliente.UsuarioId,
                cliente.Nome,
                cliente.Email,
                cliente.Telefone,
                cliente.DataCadastro
            });
        }

        // ============================================================
        // POST: api/cliente
        // Completa o cadastro do cliente
        // ============================================================
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente cliente)
        {
            int? usuarioId = ObterUsuarioId();

            if (usuarioId == null)
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não identificado."
                });
            }

            var clienteExistente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (clienteExistente != null)
            {
                return Conflict(new
                {
                    mensagem = "O cadastro do cliente já existe."
                });
            }

            var usuario =
                _usuarioRepository.BuscarPorId(usuarioId.Value);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensagem = "Usuário não encontrado."
                });
            }

            if (string.IsNullOrWhiteSpace(cliente.Telefone))
            {
                return BadRequest(new
                {
                    mensagem = "Informe o telefone."
                });
            }

            // Dados controlados pelo servidor
            cliente.UsuarioId = usuarioId.Value;
            cliente.Nome = usuario.Nome;
            cliente.Email = usuario.Email;
            cliente.DataCadastro = DateTime.Now;

            _clienteRepository.Cadastrar(cliente);

            return CreatedAtAction(
                nameof(Me),
                null,
                new
                {
                    cliente.Id,
                    cliente.UsuarioId,
                    cliente.Nome,
                    cliente.Email,
                    cliente.Telefone,
                    cliente.DataCadastro
                });
        }

        // ============================================================
        // PUT: api/cliente/me
        // Atualiza os dados do cliente logado
        // ============================================================
        [HttpPut("me")]
        public IActionResult Atualizar([FromBody] Cliente cliente)
        {
            int? usuarioId = ObterUsuarioId();

            if (usuarioId == null)
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não identificado."
                });
            }

            var clienteExistente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (clienteExistente == null)
            {
                return NotFound(new
                {
                    mensagem = "Cadastro do cliente não encontrado."
                });
            }

            if (string.IsNullOrWhiteSpace(cliente.Telefone))
            {
                return BadRequest(new
                {
                    mensagem = "Informe o telefone."
                });
            }

            // Nunca confiar no Id ou UsuarioId enviados pelo aplicativo
            cliente.Id = clienteExistente.Id;
            cliente.UsuarioId = usuarioId.Value;

            // Nome e e-mail continuam vinculados ao Usuario
            cliente.Nome = clienteExistente.Nome;
            cliente.Email = clienteExistente.Email;

            _clienteRepository.Atualizar(cliente);

            var clienteAtualizado =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            return Ok(new
            {
                mensagem = "Cadastro atualizado com sucesso.",
                cliente = clienteAtualizado
            });
        }

        // ============================================================
        // Obtém o UsuarioId a partir do JWT
        // ============================================================
        private int? ObterUsuarioId()
        {
            var claim =
                User.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return null;
            }

            if (int.TryParse(claim.Value, out int usuarioId))
            {
                return usuarioId;
            }

            return null;
        }
    }
}
