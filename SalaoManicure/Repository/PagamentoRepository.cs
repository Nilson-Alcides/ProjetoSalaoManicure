
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

        public Pagamento? BuscarPorAgendamentoId(int agendamentoId)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            const string sql = @"
        SELECT
            Id,
            AgendamentoId,
            Valor,
            Percentual,
            Gateway,
            Status,
            MercadoPagoOrderId,
            PagamentoExternoId,
            CheckoutUrl,
            DataCadastro,
            DataAtualizacao
        FROM pagamentos
        WHERE AgendamentoId = @AgendamentoId
        LIMIT 1;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@AgendamentoId", agendamentoId);

            using var reader = comando.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Pagamento
            {
                Id = Convert.ToInt32(reader["Id"]),
                AgendamentoId = Convert.ToInt32(reader["AgendamentoId"]),
                Valor = Convert.ToDecimal(reader["Valor"]),
                Percentual = Convert.ToDecimal(reader["Percentual"]),
                Gateway = reader["Gateway"].ToString()!,
                Status = reader["Status"].ToString()!,
                MercadoPagoOrderId = reader["MercadoPagoOrderId"] as string,
                PagamentoExternoId = reader["PagamentoExternoId"] as string,
                CheckoutUrl = reader["CheckoutUrl"] as string,
                DataCadastro = Convert.ToDateTime(reader["DataCadastro"]),
                DataAtualizacao = reader["DataAtualizacao"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["DataAtualizacao"])
            };
        }

        // Buscar pagamento pelo ID da Order do Mercado Pago
        public Pagamento? BuscarPorOrderId(string orderId)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            const string sql = @"
        SELECT
            Id,
            AgendamentoId,
            Valor,
            Percentual,
            Gateway,
            Status,
            MercadoPagoOrderId,
            PagamentoExternoId,
            CheckoutUrl,
            DataCadastro,
            DataAtualizacao
        FROM pagamentos
        WHERE MercadoPagoOrderId = @OrderId
        LIMIT 1;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@OrderId", orderId);

            using var reader = comando.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Pagamento
            {
                Id = Convert.ToInt32(reader["Id"]),
                AgendamentoId = Convert.ToInt32(reader["AgendamentoId"]),
                Valor = Convert.ToDecimal(reader["Valor"]),
                Percentual = Convert.ToDecimal(reader["Percentual"]),
                Gateway = reader["Gateway"].ToString()!,
                Status = reader["Status"].ToString()!,
                MercadoPagoOrderId = reader["MercadoPagoOrderId"] as string,
                PagamentoExternoId = reader["PagamentoExternoId"] as string,
                CheckoutUrl = reader["CheckoutUrl"] as string,
                DataCadastro = Convert.ToDateTime(reader["DataCadastro"]),
                DataAtualizacao = reader["DataAtualizacao"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(reader["DataAtualizacao"])
            };
        }

        // Atualizar dados da confirmação do pagamento
        public void AtualizarDadosConfirmacao(
            int pagamentoId,
            string status,
            string? pagamentoExternoId)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            const string sql = @"
        UPDATE pagamentos
        SET
            Status = @Status,
            PagamentoExternoId = @PagamentoExternoId,
            DataAtualizacao = NOW()
        WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Status", status);

            comando.Parameters.AddWithValue(
                "@PagamentoExternoId",
                (object?)pagamentoExternoId ?? DBNull.Value);

            comando.Parameters.AddWithValue("@Id", pagamentoId);

            comando.ExecuteNonQuery();
        }


        public void ConfirmarPagamento(
            int pagamentoId,
            string? pagamentoExternoId)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            using var transacao = conexao.BeginTransaction();

            try
            {
                const string sqlPagamento = @"
            UPDATE pagamentos
            SET
                Status = 'Aprovado',
                PagamentoExternoId = @PagamentoExternoId,
                DataAtualizacao = NOW()
            WHERE Id = @Id;";

                using (var comando = new MySqlCommand(
                    sqlPagamento, conexao, transacao))
                {
                    comando.Parameters.AddWithValue("@Id", pagamentoId);

                    comando.Parameters.AddWithValue(
                        "@PagamentoExternoId",
                        (object?)pagamentoExternoId ?? DBNull.Value);

                    int linhasAfetadas = comando.ExecuteNonQuery();

                    if (linhasAfetadas == 0)
                    {
                        throw new InvalidOperationException(
                            "Pagamento não encontrado para confirmação.");
                    }
                }

                const string sqlAgendamento = @"
            UPDATE agendamentos a
            INNER JOIN pagamentos p
                ON p.AgendamentoId = a.Id
            SET a.Status = 'Agendado'
            WHERE p.Id = @PagamentoId
              AND p.Status = 'Aprovado';";

                using (var comando = new MySqlCommand(
                    sqlAgendamento, conexao, transacao))
                {
                    comando.Parameters.AddWithValue(
                        "@PagamentoId", pagamentoId);

                    int linhasAfetadas = comando.ExecuteNonQuery();

                    if (linhasAfetadas == 0)
                    {
                        throw new InvalidOperationException(
                            "Agendamento não encontrado para confirmação.");
                    }
                }

                transacao.Commit();
            }
            catch
            {
                transacao.Rollback();
                throw;
            }
        }

    }
}
