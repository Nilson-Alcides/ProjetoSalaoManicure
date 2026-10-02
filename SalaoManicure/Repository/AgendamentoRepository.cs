
using MySql.Data.MySqlClient;

using SalaoManicure.Models;

using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Repository
{
    public class AgendamentoRepository : IAgendamentoRepository
    {
        private readonly string _conexao;

        public AgendamentoRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception(
                    "ConnectionString não encontrada.");
        }

        // Cadastrar
        public void Cadastrar(Agendamento agendamento)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                INSERT INTO Agendamentos
                    (Data,
                     Horario,
                     Status,
                     ClienteId,
                     ProfissionalId,
                     ServicoId)
                VALUES
                    (@Data,
                     @Horario,
                     @Status,
                     @ClienteId,
                     @ProfissionalId,
                     @ServicoId);";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Data",
                agendamento.Data.Date);

            comando.Parameters.AddWithValue(
                "@Horario",
                agendamento.Horario);

            comando.Parameters.AddWithValue(
                "@Status",
                agendamento.Status);

            comando.Parameters.AddWithValue(
                "@ClienteId",
                agendamento.ClienteId);

            comando.Parameters.AddWithValue(
                "@ProfissionalId",
                agendamento.ProfissionalId);

            comando.Parameters.AddWithValue(
                "@ServicoId",
                agendamento.ServicoId);

            comando.ExecuteNonQuery();
        }

        // Atualizar
        public void Atualizar(Agendamento agendamento)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                UPDATE Agendamentos
                SET
                    Data = @Data,
                    Horario = @Horario,
                    Status = @Status,
                    ClienteId = @ClienteId,
                    ProfissionalId = @ProfissionalId,
                    ServicoId = @ServicoId
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Data",
                agendamento.Data.Date);

            comando.Parameters.AddWithValue(
                "@Horario",
                agendamento.Horario);

            comando.Parameters.AddWithValue(
                "@Status",
                agendamento.Status);

            comando.Parameters.AddWithValue(
                "@ClienteId",
                agendamento.ClienteId);

            comando.Parameters.AddWithValue(
                "@ProfissionalId",
                agendamento.ProfissionalId);

            comando.Parameters.AddWithValue(
                "@ServicoId",
                agendamento.ServicoId);

            comando.Parameters.AddWithValue(
                "@Id",
                agendamento.Id);

            comando.ExecuteNonQuery();
        }

        // Excluir
        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                DELETE FROM Agendamentos
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Id",
                id);

            comando.ExecuteNonQuery();
        }

        // Listar
        public List<Agendamento> Listar()
        {
            var agendamentos = new List<Agendamento>();

            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    a.Id,
                    a.Data,
                    a.Horario,
                    a.Status,

                    a.ClienteId,
                    c.Nome AS ClienteNome,

                    a.ProfissionalId,
                    p.Nome AS ProfissionalNome,

                    a.ServicoId,
                    s.Nome AS ServicoNome

                FROM Agendamentos a

                INNER JOIN Clientes c
                    ON c.Id = a.ClienteId

                INNER JOIN Profissionais p
                    ON p.Id = a.ProfissionalId

                INNER JOIN Servicos s
                    ON s.Id = a.ServicoId

                ORDER BY
                    a.Data,
                    a.Horario;";

            using var comando = new MySqlCommand(sql, conexao);

            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                var agendamento = new Agendamento
                {
                    Id = reader.GetInt32("Id"),

                    Data = reader.GetDateTime("Data"),

                    Horario = reader.GetTimeSpan("Horario"),

                    Status = reader.GetString("Status"),

                    ClienteId =
                        reader.GetInt32("ClienteId"),

                    Cliente = new Cliente
                    {
                        Id =
                            reader.GetInt32("ClienteId"),

                        Nome =
                            reader.GetString("ClienteNome")
                    },

                    ProfissionalId =
                        reader.GetInt32("ProfissionalId"),

                    Profissional = new Profissional
                    {
                        Id =
                            reader.GetInt32("ProfissionalId"),

                        Nome =
                            reader.GetString("ProfissionalNome")
                    },

                    ServicoId =
                        reader.GetInt32("ServicoId"),

                    Servico = new Servico
                    {
                        Id =
                            reader.GetInt32("ServicoId"),

                        Nome =
                            reader.GetString("ServicoNome")
                    }
                };

                agendamentos.Add(agendamento);
            }

            return agendamentos;
        }

        // Buscar por ID
        public Agendamento? BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    a.Id,
                    a.Data,
                    a.Horario,
                    a.Status,

                    a.ClienteId,
                    c.Nome AS ClienteNome,

                    a.ProfissionalId,
                    p.Nome AS ProfissionalNome,

                    a.ServicoId,
                    s.Nome AS ServicoNome

                FROM Agendamentos a

                INNER JOIN Clientes c
                    ON c.Id = a.ClienteId

                INNER JOIN Profissionais p
                    ON p.Id = a.ProfissionalId

                INNER JOIN Servicos s
                    ON s.Id = a.ServicoId

                WHERE a.Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Id",
                id);

            using var reader = comando.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Agendamento
            {
                Id = reader.GetInt32("Id"),

                Data = reader.GetDateTime("Data"),

                Horario = reader.GetTimeSpan("Horario"),

                Status = reader.GetString("Status"),

                ClienteId =
                    reader.GetInt32("ClienteId"),

                Cliente = new Cliente
                {
                    Id =
                        reader.GetInt32("ClienteId"),

                    Nome =
                        reader.GetString("ClienteNome")
                },

                ProfissionalId =
                    reader.GetInt32("ProfissionalId"),

                Profissional = new Profissional
                {
                    Id =
                        reader.GetInt32("ProfissionalId"),

                    Nome =
                        reader.GetString("ProfissionalNome")
                },

                ServicoId =
                    reader.GetInt32("ServicoId"),

                Servico = new Servico
                {
                    Id =
                        reader.GetInt32("ServicoId"),

                    Nome =
                        reader.GetString("ServicoNome")
                }
            };
        }
    }
}

