
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Models.Constants;
using SalaoManicure.Repositories;
using SalaoManicure.Repository.Contract;
using SalaoManicure.Services;

namespace SalaoManicure.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly JwtService _jwtService;

        public AuthController(
            IUsuarioRepository usuarioRepository,
           IClienteRepository clienteRepository,
            JwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _clienteRepository = clienteRepository;
            _jwtService = jwtService;
        }


        // =========================================================
        // LOGIN - GET
        // =========================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // =========================================================
        // LOGIN - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Informe o e-mail."
                );
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError(
                    "Senha",
                    "Informe a senha."
                );
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var usuario = _usuarioRepository.Login(
                email.Trim().ToLower(),
                senha
            );

            if (usuario == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "E-mail ou senha inválidos."
                );

                return View();
            }

            if (!usuario.Ativo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Usuário inativo."
                );

                return View();
            }

            // Cria o token JWT
            var token = _jwtService.GerarToken(usuario);

            // Guarda o token na sessão
            HttpContext.Session.SetString(
                "JwtToken",
                token
            );

            HttpContext.Session.SetInt32(
                "UsuarioId",
                usuario.Id
            );

            HttpContext.Session.SetString(
                "UsuarioNome",
                usuario.Nome
            );

            HttpContext.Session.SetString(
                "UsuarioEmail",
                usuario.Email
            );

            HttpContext.Session.SetString(
                "UsuarioPerfil",
                usuario.Perfil
            );

            // Redirecionamento conforme o perfil
            if (usuario.Perfil == PerfisUsuario.Administrador ||
                usuario.Perfil == PerfisUsuario.Profissional)
            {
                return RedirectToAction(
                    "Index",
                    "Home"
                );
            }
            if (usuario.Perfil == PerfisUsuario.Cliente) 
            {
                var cliente = _clienteRepository.BuscarPorUsuarioId(usuario.Id);
                if (cliente == null) 
                { 
                    return RedirectToAction("CompletarCadastro", "Cliente");
                } 
            }
            // Cliente
            return RedirectToAction("Index", "Home");           
           
        }


        // =========================================================
        // CADASTRO - GET
        // =========================================================

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }


        // =========================================================
        // CADASTRO - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registrar(
            string nome,
            string email,
            string senha)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                ModelState.AddModelError(
                    "Nome",
                    "Informe o nome."
                );
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Informe o e-mail."
                );
            }

            if (string.IsNullOrWhiteSpace(senha))
            {
                ModelState.AddModelError(
                    "Senha",
                    "Informe a senha."
                );
            }

            if (senha.Length < 6)
            {
                ModelState.AddModelError(
                    "Senha",
                    "A senha deve possuir pelo menos 6 caracteres."
                );
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var usuarioExistente =
                _usuarioRepository.BuscarPorEmail(
                    email.Trim().ToLower()
                );

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "Este e-mail já está cadastrado."
                );

                return View();
            }

            var usuario = new Usuario
            {
                Nome = nome.Trim(),
                Email = email.Trim().ToLower(),

                SenhaHash =
                    BCrypt.Net.BCrypt.HashPassword(senha)
            };

            // O Repository define:
            // Perfil = Cliente
            // Ativo = true
            // DataCadastro = DateTime.Now

            _usuarioRepository.Cadastrar(usuario);

            TempData["Sucesso"] =
                "Cadastro realizado com sucesso!";

            return RedirectToAction("Login");
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}

