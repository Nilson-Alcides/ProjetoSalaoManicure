
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers
{
    public class BloqueiosController : Controller
    {
        private readonly IBloqueioRepository _bloqueioRepository;
        private readonly IProfissionalRepository _profissionalRepository;

        public BloqueiosController(
            IBloqueioRepository bloqueioRepository,
            IProfissionalRepository profissionalRepository)
        {
            _bloqueioRepository = bloqueioRepository;
            _profissionalRepository = profissionalRepository;
        }

        // GET: Bloqueios
        public IActionResult Index()
        {
            var bloqueios =
                _bloqueioRepository.Listar();

            return View(bloqueios);
        }

        // GET: Bloqueios/Cadastrar
        [HttpGet]
        public IActionResult Cadastrar()
        {
            CarregarProfissionais();

            return View();
        }

        // POST: Bloqueios/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Bloqueio bloqueio)
        {
            if (!ModelState.IsValid)
            {
                CarregarProfissionais();

                return View(bloqueio);
            }

            _bloqueioRepository.Cadastrar(bloqueio);

            return RedirectToAction(nameof(Index));
        }

        // GET: Bloqueios/Editar/1
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var bloqueio =
                _bloqueioRepository.BuscarPorId(id);

            if (bloqueio == null)
            {
                return NotFound();
            }

            CarregarProfissionais();

            return View(bloqueio);
        }

        // POST: Bloqueios/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(Bloqueio bloqueio)
        {
            if (!ModelState.IsValid)
            {
                CarregarProfissionais();

                return View(bloqueio);
            }

            var bloqueioExistente =
                _bloqueioRepository.BuscarPorId(bloqueio.Id);

            if (bloqueioExistente == null)
            {
                return NotFound();
            }

            _bloqueioRepository.Atualizar(bloqueio);

            return RedirectToAction(nameof(Index));
        }

        // GET: Bloqueios/Excluir/1
        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var bloqueio =
                _bloqueioRepository.BuscarPorId(id);

            if (bloqueio == null)
            {
                return NotFound();
            }

            return View(bloqueio);
        }

        // POST: Bloqueios/Excluir
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExclusao(int id)
        {
            _bloqueioRepository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }

        // Carrega somente profissionais ativos
        private void CarregarProfissionais()
        {
            ViewBag.Profissionais =
                _profissionalRepository.Listar()
                    .Where(p => p.Ativo)
                    .ToList();
        }
    }
}

