using System.ComponentModel.DataAnnotations;

namespace SalaoManicure.Models
{
    public class Disponibilidade
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a data.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Informe o horário.")]
        public TimeSpan Horario { get; set; }
        public Profissional? Profissional { get; set; }
        public bool Disponivel { get; set; } = true;
    }
}   