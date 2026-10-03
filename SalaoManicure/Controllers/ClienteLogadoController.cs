
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
                _clienteRepository.BuscarPorUsuarioId(
                    usuarioId.Value);

            // Cliente ainda não completou o cadastro
            if (cliente == null)
            {
                return RedirectToAction(
                    nameof(CompletarCadastro));
            }

            return View(cliente);
        }

        // ==========================================
        // COMPLETAR CADASTRO - GET
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
                _clienteRepository.BuscarPorUsuarioId(
                    usuarioId.Value);

            // Já possui cadastro
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
        // COMPLETAR CADASTRO - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CompletarCadastro(
            Cliente cliente)
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
                    HttpContext.Session.GetString(
                        "UsuarioNome");

                ViewBag.UsuarioEmail =
                    HttpContext.Session.GetString(
                        "UsuarioEmail");

                return View(cliente);
            }

            // ==========================================
            // DADOS CONTROLADOS PELO SISTEMA
            // ==========================================

            // Nunca confiar no UsuarioId do formulário
            cliente.UsuarioId = usuarioId.Value;

            cliente.Nome =
                HttpContext.Session.GetString(
                    "UsuarioNome")
                ?? string.Empty;

            cliente.Email =
                HttpContext.Session.GetString(
                    "UsuarioEmail")
                ?? string.Empty;

            cliente.DataCadastro = DateTime.Now;

            // ==========================================
            // VERIFICA SE JÁ EXISTE
            // ==========================================

            var clienteExistente =
                _clienteRepository.BuscarPorUsuarioId(
                    usuarioId.Value);

            if (clienteExistente != null)
            {
                return RedirectToAction(nameof(Index));
            }

            // ==========================================
            // CADASTRA
            // ==========================================

            _clienteRepository.Cadastrar(cliente);

            TempData["MensagemSucesso"] =
                "Cadastro do cliente realizado com sucesso!";

            // Vai para o painel do cliente
            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // EDITAR PERFIL - GET
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
                _clienteRepository.BuscarPorUsuarioId(
                    usuarioId.Value);

            if (cliente == null)
            {
                return RedirectToAction(
                    nameof(CompletarCadastro));
            }

            return View(cliente);
        }

        // ==========================================
        // EDITAR PERFIL - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(
            Cliente cliente)
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

            // Busca o cliente do usuário logado
            var clienteExistente =
                _clienteRepository.BuscarPorUsuarioId(
                    usuarioId.Value);

            if (clienteExistente == null)
            {
                return RedirectToAction(
                    nameof(CompletarCadastro));
            }

            // ==========================================
            // DADOS CONTROLADOS PELO SISTEMA
            // ==========================================

            // Não confiar no Id enviado pelo formulário
            cliente.Id = clienteExistente.Id;

            // Não confiar no UsuarioId enviado pelo formulário
            cliente.UsuarioId = usuarioId.Value;

            cliente.Nome =
                HttpContext.Session.GetString(
                    "UsuarioNome")
                ?? clienteExistente.Nome;

            cliente.Email =
                HttpContext.Session.GetString(
                    "UsuarioEmail")
                ?? clienteExistente.Email;

            // ==========================================
            // ATUALIZA
            // ==========================================

            _clienteRepository.Atualizar(cliente);

            TempData["MensagemSucesso"] =
                "Cadastro atualizado com sucesso!";

            return RedirectToAction(nameof(Index));
        }
    }
}

