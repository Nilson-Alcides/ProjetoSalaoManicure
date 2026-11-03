
namespace SalaoManicure.Services
{
    public class ResultadoPagamento
    {
        public string OrderId { get; set; } = string.Empty;

        public string CheckoutUrl { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
