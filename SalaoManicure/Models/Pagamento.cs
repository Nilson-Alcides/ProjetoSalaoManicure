
namespace SalaoManicure.Models
{
    public class Pagamento
    {
        public int Id { get; set; }

        public int AgendamentoId { get; set; }

        public decimal Valor { get; set; }

        public decimal Percentual { get; set; } = 30.00m;

        public string Gateway { get; set; } = "MercadoPago";

        public string Status { get; set; } = "Pendente";

        public string? MercadoPagoOrderId { get; set; }

        public string? PagamentoExternoId { get; set; }

        public string? CheckoutUrl { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }


    }
}
