
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Models.DTOs.Auth;
using SalaoManicure.Repositories;
using SalaoManicure.Services;

namespace SalaoManicure.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly JwtService _jwtService;

        public AuthApiController(
            IUsuarioRepository usuarioRepository,
            JwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _jwtService = jwtService;
        }


        // =========================================================
        // REGISTRAR CLIENTE
        // POST: /api/auth/registrar
        // =========================================================

        [HttpPost("registrar")]
        public IActionResult Registrar(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Nome))
            {
                return BadRequest(new
                {
                    mensagem = "Informe o nome."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    mensagem = "Informe o e-mail."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new
                {
                    mensagem = "Informe a senha."
                });
            }

            // Verifica se o e-mail já está cadastrado
            var usuarioExistente =
                _usuarioRepository.BuscarPorEmail(request.Email);

            if (usuarioExistente != null)
            {
                return Conflict(new
                {
                    mensagem = "Este e-mail já está cadastrado."
                });
            }

            // Cria o usuário
            var usuario = new Usuario
            {
                Nome = request.Nome.Trim(),
                Email = request.Email.Trim().ToLower(),

                // A senha nunca é salva diretamente.
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Senha
                )
            };

            // O Repository define o Perfil = Cliente
            _usuarioRepository.Cadastrar(usuario);

            return Created("", new
            {
                mensagem = "Cliente cadastrado com sucesso.",
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                perfil = usuario.Perfil
            });
        }


        // =========================================================
        // LOGIN
        // POST: /api/auth/login
        // =========================================================

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    mensagem = "Informe o e-mail."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Senha))
            {
                return BadRequest(new
                {
                    mensagem = "Informe a senha."
                });
            }

            var usuario = _usuarioRepository.Login(
                request.Email.Trim().ToLower(),
                request.Senha
            );

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensagem = "E-mail ou senha inválidos."
                });
            }

            // Gera o JWT
            var token = _jwtService.GerarToken(usuario);

            var response = new AuthResponse
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                Token = token
            };

            return Ok(response);
        }
    }
}

