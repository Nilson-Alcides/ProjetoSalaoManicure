namespace SalaoManicure.Models
{
    public class Profissional
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public List<Agendamento> Agendamentos { get; set; } = new();
    }
}
