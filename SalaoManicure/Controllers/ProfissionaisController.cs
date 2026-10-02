using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers
{
    public class ProfissionaisController : Controller
    {
        private readonly IProfissionalRepository _profissionalRepository;

        public ProfissionaisController(
            IProfissionalRepository profissionalRepository)
        {
            _profissionalRepository = profissionalRepository;
        }

        // GET: Profissionais
        public IActionResult Index()
        {
            var profissionais = _profissionalRepository.Listar();

            return View(profissionais);
        }

        // GET: Profissionais/Cadastrar
        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        // POST: Profissionais/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Profissional profissional)
        {
            if (!ModelState.IsValid)
            {
                return View(profissional);
            }

            _profissionalRepository.Cadastrar(profissional);

            return RedirectToAction(nameof(Index));
        }

        // GET: Profissionais/Editar/1
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var profissional =
                _profissionalRepository.BuscarPorId(id);

            if (profissional == null)
            {
                return NotFound();
            }

            return View(profissional);
        }

        // POST: Profissionais/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Profissional profissional)
        {
            if (!ModelState.IsValid)
            {
                return View(profissional);
            }

            _profissionalRepository.Atualizar(profissional);

            return RedirectToAction(nameof(Index));
        }

        // GET: Profissionais/Excluir/1
        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var profissional =
                _profissionalRepository.BuscarPorId(id);

            if (profissional == null)
            {
                return NotFound();
            }

            return View(profissional);
        }

        // POST: Profissionais/Excluir
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExclusao(int id)
        {
            _profissionalRepository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }
    }
}


