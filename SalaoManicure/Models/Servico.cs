namespace SalaoManicure.Models
{
    public class Servico
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public decimal Preco { get; set; }

        public int DuracaoMinutos { get; set; }

        public bool Ativo { get; set; } = true;

        public List<Agendamento> Agendamentos { get; set; } = new();
    }
}
