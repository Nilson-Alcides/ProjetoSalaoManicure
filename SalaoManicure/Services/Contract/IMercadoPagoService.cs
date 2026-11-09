namespace SalaoManicure.Services.Contract
{
    public interface IMercadoPagoService
    {
        Task<ResultadoPagamento> CriarOrderAsync(
            int agendamentoId,
            string nomeServico,
            decimal valor,
            string emailCliente,
            string nomeCliente,
            string urlSucesso,
            string urlFalha,
            string urlPendente);

        Task<ResultadoConsultaOrder> ConsultarOrderAsync(
            string orderId);
    }

    public class ResultadoPagamento
    {
        public string OrderId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class ResultadoConsultaOrder
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusDetail { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;
        public string? PaymentId { get; set; }
    }
}