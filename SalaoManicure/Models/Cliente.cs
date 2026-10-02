namespace SalaoManicure.Models;

public class Cliente
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Telefone { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; }

    public List<Agendamento> Agendamentos { get; set; } = new();

}
