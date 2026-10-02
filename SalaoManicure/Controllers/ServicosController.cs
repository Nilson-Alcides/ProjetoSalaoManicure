using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers
{
    public class ServicosController : Controller
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicosController(
            IServicoRepository servicoRepository)
        {
            _servicoRepository = servicoRepository;
        }

        // GET: Servicos
        public IActionResult Index()
        {
            var servicos = _servicoRepository.Listar();

            return View(servicos);
        }

        // GET: Servicos/Cadastrar
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        // POST: Servicos/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Servico servico)
        {
            if (!ModelState.IsValid)
            {
                return View(servico);
            }

            _servicoRepository.Cadastrar(servico);

            return RedirectToAction(nameof(Index));
        }

        // GET: Servicos/Editar/1
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var servico = _servicoRepository.BuscarPorId(id);

            if (servico == null)
            {
                return NotFound();
            }

            return View(servico);
        }

        // POST: Servicos/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Servico servico)
        {
            if (!ModelState.IsValid)
            {
                return View(servico);
            }

            _servicoRepository.Atualizar(servico);

            return RedirectToAction(nameof(Index));
        }

        // GET: Servicos/Excluir/1
        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var servico = _servicoRepository.BuscarPorId(id);

            if (servico == null)
            {
                return NotFound();
            }

            return View(servico);
        }

        // POST: Servicos/Excluir
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExclusao(int id)
        {
            _servicoRepository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
