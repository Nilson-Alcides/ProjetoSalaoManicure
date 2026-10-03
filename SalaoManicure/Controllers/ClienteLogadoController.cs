
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repositories;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    public class ClienteLogadoController : Controller
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteLogadoController(
            IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // ==========================================
        // PAINEL DO CLIENTE
        // ==========================================

        [HttpGet]
        public IActionResult Index()
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            // Se ainda não completou o cadastro,
            // direciona para completar cadastro.
            if (cliente == null)
            {
                return RedirectToAction(nameof(CompletarCadastro));
            }

            return View(cliente);
        }

        // ==========================================
        // COMPLETAR CADASTRO
        // ==========================================

        [HttpGet]
        public IActionResult CompletarCadastro()
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente != null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.UsuarioNome =
                HttpContext.Session.GetString("UsuarioNome");

            ViewBag.UsuarioEmail =
                HttpContext.Session.GetString("UsuarioEmail");

            return View();
        }

        // ==========================================
        // SALVAR CADASTRO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompletarCadastro(Cliente cliente)
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (string.IsNullOrWhiteSpace(cliente.Telefone))
            {
                ModelState.AddModelError(
                    "Telefone",
                    "Informe o telefone.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UsuarioNome =
                    HttpContext.Session.GetString("UsuarioNome");

                ViewBag.UsuarioEmail =
                    HttpContext.Session.GetString("UsuarioEmail");

                return View(cliente);
            }

            // Nunca confiar no UsuarioId vindo do formulário
            cliente.UsuarioId = usuarioId.Value;

            cliente.Nome =
                HttpContext.Session.GetString("UsuarioNome")
                ?? string.Empty;

            cliente.Email =
                HttpContext.Session.GetString("UsuarioEmail")
                ?? string.Empty;

            cliente.DataCadastro = DateTime.Now;

            var clienteExistente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (clienteExistente != null)
            {
                return RedirectToAction(nameof(Index));
            }

            _clienteRepository.Cadastrar(cliente);

            TempData["MensagemSucesso"] =
                "Cadastro do cliente realizado com sucesso!";

            // Vai para o painel do cliente
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // EDITAR PERFIL
        // ==========================================

        [HttpGet]
        public IActionResult Editar()
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var cliente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (cliente == null)
            {
                return RedirectToAction(nameof(CompletarCadastro));
            }

            return View(cliente);
        }

        // ==========================================
        // SALVAR EDIÇÃO
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Cliente cliente)
        {
            int? usuarioId =
                HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            var clienteExistente =
                _clienteRepository.BuscarPorUsuarioId(usuarioId.Value);

            if (clienteExistente == null)
            {
                return RedirectToAction(nameof(CompletarCadastro));
            }

            // Utiliza o cliente do usuário logado
            cliente.Id = clienteExistente.Id;
            cliente.UsuarioId = usuarioId.Value;

            cliente.Nome =
                HttpContext.Session.GetString("UsuarioNome")
                ?? clienteExistente.Nome;

            cliente.Email =
                HttpContext.Session.GetString("UsuarioEmail")
                ?? clienteExistente.Email;

            _clienteRepository.Atualizar(cliente);

            TempData["MensagemSucesso"] =
                "Cadastro atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}

