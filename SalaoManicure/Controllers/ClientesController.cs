using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // ==========================================
        // LISTAR CLIENTES
        // GET: Clientes
        // ==========================================

        public IActionResult Index()
        {
            var clientes = _clienteRepository.Listar();

            return View(clientes);
        }


        // ==========================================
        // CADASTRAR - GET
        // GET: Clientes/Cadastrar
        // ==========================================

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }


        // ==========================================
        // CADASTRAR - POST
        // POST: Clientes/Cadastrar
        // ==========================================

        [HttpPost]
        //   [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            cliente.DataCadastro = DateTime.Now;

            _clienteRepository.Cadastrar(cliente);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // EDITAR - GET
        // GET: Clientes/Editar/5
        // ==========================================

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var cliente = _clienteRepository.BuscarPorId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        // ==========================================
        // EDITAR - POST
        // POST: Clientes/Editar
        // ==========================================

        [HttpPost]
        //  [ValidateAntiForgeryToken]
        public IActionResult Editar(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            _clienteRepository.Atualizar(cliente);

            return RedirectToAction(nameof(Index));
        }


        // ==========================================
        // EXCLUIR
        // GET: Clientes/Excluir/5
        // ==========================================

        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var cliente = _clienteRepository.BuscarPorId(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        // ==========================================
        // CONFIRMAR EXCLUSÃO
        // POST: Clientes/Excluir
        // ==========================================

        [HttpPost, ActionName("Excluir")]
      //  [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExclusao(int id)
        {
            _clienteRepository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }
    }
}

