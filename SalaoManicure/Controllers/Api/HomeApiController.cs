
using Microsoft.AspNetCore.Mvc;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeApiController : ControllerBase
    {
        private readonly IDisponibilidadeRepository
            _disponibilidadeRepository;

        public HomeApiController(
            IDisponibilidadeRepository disponibilidadeRepository)
        {
            _disponibilidadeRepository =
                disponibilidadeRepository;
        }

        // GET: api/Home
        [HttpGet]
        public IActionResult Index()
        {
            var disponibilidades =
                _disponibilidadeRepository
                    .Listar()
                    .Where(x =>
                        x.Data.Date >= DateTime.Today &&
                        x.Disponivel)
                    .OrderBy(x => x.Data)
                    .ThenBy(x => x.Horario)
                    .Select(x => new
                    {
                        x.Id,
                        Data = x.Data.ToString("yyyy-MM-dd"),
                        Horario = x.Horario.ToString(@"hh\:mm"),
                        x.Disponivel
                    })
                    .ToList();

            return Ok(disponibilidades);
        }
    }
}
