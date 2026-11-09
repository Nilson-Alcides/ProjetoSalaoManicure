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
                Pagamento? BuscarPorAgendamentoId(int agendamentoId);

                Pagamento? BuscarPorOrderId(string orderId);

                void AtualizarDadosConfirmacao(
                    int pagamentoId,
                    string status,
                    string? pagamentoExternoId);

               void ConfirmarPagamento(
                    int pagamentoId,
                    string? pagamentoExternoId);    
            }
        }