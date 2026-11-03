namespace SalaoManicure.Services.Contract
{
    public interface IMercadoPagoService
    {
        Task<SalaoManicure.Services.Contract.ResultadoPagamento>
            CriarOrderAsync(
                int agendamentoId,
                string nomeServico,
                decimal valor,
                string emailCliente,
                string nomeCliente,
                string urlSucesso,
                string urlFalha,
                string urlPendente);
    }

    public class ResultadoPagamento
    {
        public string OrderId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}