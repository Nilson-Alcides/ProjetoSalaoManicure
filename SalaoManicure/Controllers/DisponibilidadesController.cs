using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Controllers
{
    public class DisponibilidadesController : Controller
    {
        private readonly IDisponibilidadeRepository _disponibilidadeRepository;
        private readonly IProfissionalRepository _profissionalRepository;

        public DisponibilidadesController(
            IDisponibilidadeRepository disponibilidadeRepository,
            IProfissionalRepository profissionalRepository)
        {
            _disponibilidadeRepository = disponibilidadeRepository;
            _profissionalRepository = profissionalRepository;
        }

        // GET: Disponibilidades
        public IActionResult Index()
        {
            var disponibilidades =
                _disponibilidadeRepository.Listar();

            return View(disponibilidades);
        }

        // GET: Disponibilidades/Cadastrar
        [HttpGet]
        public IActionResult Cadastrar()
        {
            CarregarProfissionais();

            return View();
        }

        // POST: Disponibilidades/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(
            Disponibilidade disponibilidade)
        {
            if (!ModelState.IsValid)
            {
                CarregarProfissionais();

                return View(disponibilidade);
            }

            _disponibilidadeRepository.Cadastrar(
                disponibilidade);

            return RedirectToAction(nameof(Index));
        }

        // GET: Disponibilidades/Editar/1
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var disponibilidade =
                _disponibilidadeRepository.BuscarPorId(id);

            if (disponibilidade == null)
            {
                return NotFound();
            }

            CarregarProfissionais();

            return View(disponibilidade);
        }

        // POST: Disponibilidades/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(
            Disponibilidade disponibilidade)
        {
            if (!ModelState.IsValid)
            {
                CarregarProfissionais();

                return View(disponibilidade);
            }

            _disponibilidadeRepository.Atualizar(
                disponibilidade);

            return RedirectToAction(nameof(Index));
        }

        // GET: Disponibilidades/Excluir/1
        [HttpGet]
        public IActionResult Excluir(int id)
        {
            var disponibilidade =
                _disponibilidadeRepository.BuscarPorId(id);

            if (disponibilidade == null)
            {
                return NotFound();
            }

            return View(disponibilidade);
        }

        // POST: Disponibilidades/Excluir
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExclusao(int id)
        {
            _disponibilidadeRepository.Excluir(id);

            return RedirectToAction(nameof(Index));
        }

        // Carrega os profissionais para o formulário
        private void CarregarProfissionais()
        {
            ViewBag.Profissionais =
                _profissionalRepository.Listar()
                    .Where(p => p.Ativo)
                    .ToList();
        }
    }
}

