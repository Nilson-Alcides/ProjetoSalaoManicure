
using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Repository
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly string _conexao;

        public PagamentoRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception(
                    "ConnectionString não encontrada.");
        }

        // Cadastrar pagamento
        public int Cadastrar(Pagamento pagamento)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                INSERT INTO pagamentos
                (
                    AgendamentoId,
                    Valor,
                    Percentual,
                    Gateway,
                    Status
                )
                VALUES
                (
                    @AgendamentoId,
                    @Valor,
                    @Percentual,
                    @Gateway,
                    @Status
                );";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@AgendamentoId", pagamento.AgendamentoId);

            comando.Parameters.AddWithValue(
                "@Valor", pagamento.Valor);

            comando.Parameters.AddWithValue(
                "@Percentual", pagamento.Percentual);

            comando.Parameters.AddWithValue(
                "@Gateway", pagamento.Gateway);

            comando.Parameters.AddWithValue(
                "@Status", pagamento.Status);

            comando.ExecuteNonQuery();

            return (int)comando.LastInsertedId;
        }

        // Atualizar dados retornados pelo Mercado Pago
        public void AtualizarCheckout(
            int pagamentoId,
            string mercadoPagoOrderId,
            string checkoutUrl,
            string status)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                UPDATE pagamentos
                SET
                    MercadoPagoOrderId = @MercadoPagoOrderId,
                    CheckoutUrl = @CheckoutUrl,
                    Status = @Status,
                    DataAtualizacao = NOW()
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@MercadoPagoOrderId", mercadoPagoOrderId);

            comando.Parameters.AddWithValue(
                "@CheckoutUrl", checkoutUrl);

            comando.Parameters.AddWithValue(
                "@Status", status);

            comando.Parameters.AddWithValue(
                "@Id", pagamentoId);

            comando.ExecuteNonQuery();
        }

        public void AtualizarStatus(int pagamentoId, string status)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            const string sql = @"
        UPDATE pagamentos
        SET Status = @Status,
            DataAtualizacao = NOW()
        WHERE Id = @Id";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Status", status);
            comando.Parameters.AddWithValue("@Id", pagamentoId);

            comando.ExecuteNonQuery();
        }
    }
}
