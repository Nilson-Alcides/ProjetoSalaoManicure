using SalaoManicure.Models;

namespace SalaoManicure.Repository.Contract
{
    public interface IPagamentoRepository
    {
        int Cadastrar(Pagamento pagamento);

        void AtualizarCheckout(
            int pagamentoId,
            string mercadoPagoOrderId,
            string checkoutUrl,
            string status);

        void AtualizarStatus(int pagamentoId, string status);
    }
}