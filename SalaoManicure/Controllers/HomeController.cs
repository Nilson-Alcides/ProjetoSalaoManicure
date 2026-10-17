using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDisponibilidadeRepository _disponibilidadeRepository;

        public HomeController(
            IDisponibilidadeRepository disponibilidadeRepository)
        {
            _disponibilidadeRepository = disponibilidadeRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var disponibilidades =
                _disponibilidadeRepository
                    .Listar()
                    .Where(x => x.Data.Date >= DateTime.Today)
                    .OrderBy(x => x.Data)
                    .ThenBy(x => x.Horario)
                    .ToList();

            return View(disponibilidades);
        }
    }
}