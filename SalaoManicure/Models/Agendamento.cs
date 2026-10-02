
using System.ComponentModel.DataAnnotations;

namespace SalaoManicure.Models
{
    public class Agendamento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe a data.")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Informe o horário.")]
        public TimeSpan Horario { get; set; }

        [Required(ErrorMessage = "Informe o status.")]
        public string Status { get; set; } = "Agendado";


        // ============================================================
        // CLIENTE
        // ============================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Selecione um cliente.")]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }


        // ============================================================
        // PROFISSIONAL
        // ============================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Selecione um profissional.")]
        public int ProfissionalId { get; set; }

        public Profissional? Profissional { get; set; }


        // ============================================================
        // SERVIÇO
        // ============================================================

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Selecione um serviço.")]
        public int ServicoId { get; set; }

        public Servico? Servico { get; set; }
    }
}

