
using SalaoManicure.Models;
using System.ComponentModel.DataAnnotations;

namespace SalaoManicure.Models
{
    public class Bloqueio
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecione um profissional.")]
        public int ProfissionalId { get; set; }

        public Profissional? Profissional { get; set; }

        [Required(ErrorMessage = "Informe a data.")]
        public DateTime Data { get; set; }

        public TimeSpan? Horario { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
    }
}

