
using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Repository
{
    public class BloqueioRepository : IBloqueioRepository
    {
        private readonly string _conexao;

        public BloqueioRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception("ConnectionString não encontrada.");
        }

        // Cadastrar bloqueio
        public void Cadastrar(Bloqueio bloqueio)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                INSERT INTO Bloqueios
                    (ProfissionalId, Data, Horario, Motivo, Ativo)
                VALUES
                    (@ProfissionalId, @Data, @Horario, @Motivo, @Ativo);";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@ProfissionalId",
                bloqueio.ProfissionalId);

            comando.Parameters.AddWithValue(
                "@Data",
                bloqueio.Data.Date);

            comando.Parameters.AddWithValue(
                "@Horario",
                bloqueio.Horario.HasValue
                    ? bloqueio.Horario.Value
                    : DBNull.Value);

            comando.Parameters.AddWithValue(
                "@Motivo",
                bloqueio.Motivo);

            comando.Parameters.AddWithValue(
                "@Ativo",
                bloqueio.Ativo);

            comando.ExecuteNonQuery();
        }

        // Atualizar bloqueio
        public void Atualizar(Bloqueio bloqueio)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                UPDATE Bloqueios
                SET
                    ProfissionalId = @ProfissionalId,
                    Data = @Data,
                    Horario = @Horario,
                    Motivo = @Motivo,
                    Ativo = @Ativo
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Id",
                bloqueio.Id);

            comando.Parameters.AddWithValue(
                "@ProfissionalId",
                bloqueio.ProfissionalId);

            comando.Parameters.AddWithValue(
                "@Data",
                bloqueio.Data.Date);

            comando.Parameters.AddWithValue(
                "@Horario",
                bloqueio.Horario.HasValue
                    ? bloqueio.Horario.Value
                    : DBNull.Value);

            comando.Parameters.AddWithValue(
                "@Motivo",
                bloqueio.Motivo);

            comando.Parameters.AddWithValue(
                "@Ativo",
                bloqueio.Ativo);

            comando.ExecuteNonQuery();
        }

        // Excluir bloqueio
        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                DELETE FROM Bloqueios
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Id", id);

            comando.ExecuteNonQuery();
        }

        // Listar todos os bloqueios
        public List<Bloqueio> Listar()
        {
            var bloqueios = new List<Bloqueio>();

            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    b.Id,
                    b.ProfissionalId,
                    p.Nome AS ProfissionalNome,
                    b.Data,
                    b.Horario,
                    b.Motivo,
                    b.Ativo
                FROM Bloqueios b
                INNER JOIN Profissionais p
                    ON p.Id = b.ProfissionalId
                ORDER BY
                    b.Data,
                    b.Horario,
                    p.Nome;";

            using var comando = new MySqlCommand(sql, conexao);

            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                var bloqueio = new Bloqueio
                {
                    Id = reader.GetInt32("Id"),

                    ProfissionalId =
                        reader.GetInt32("ProfissionalId"),

                    Profissional = new Profissional
                    {
                        Id = reader.GetInt32("ProfissionalId"),
                        Nome = reader.GetString("ProfissionalNome")
                    },

                    Data = reader.GetDateTime("Data"),

                    Horario = reader.IsDBNull(
                        reader.GetOrdinal("Horario"))
                        ? null
                        : reader.GetTimeSpan("Horario"),

                    Motivo = reader.IsDBNull(
                        reader.GetOrdinal("Motivo"))
                        ? string.Empty
                        : reader.GetString("Motivo"),

                    Ativo = reader.GetBoolean("Ativo")
                };

                bloqueios.Add(bloqueio);
            }

            return bloqueios;
        }

        // Buscar bloqueio por ID
        public Bloqueio? BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    b.Id,
                    b.ProfissionalId,
                    p.Nome AS ProfissionalNome,
                    b.Data,
                    b.Horario,
                    b.Motivo,
                    b.Ativo
                FROM Bloqueios b
                INNER JOIN Profissionais p
                    ON p.Id = b.ProfissionalId
                WHERE b.Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Id", id);

            using var reader = comando.ExecuteReader();

            if (reader.Read())
            {
                return new Bloqueio
                {
                    Id = reader.GetInt32("Id"),

                    ProfissionalId =
                        reader.GetInt32("ProfissionalId"),

                    Profissional = new Profissional
                    {
                        Id = reader.GetInt32("ProfissionalId"),
                        Nome = reader.GetString("ProfissionalNome")
                    },

                    Data = reader.GetDateTime("Data"),

                    Horario = reader.IsDBNull(
                        reader.GetOrdinal("Horario"))
                        ? null
                        : reader.GetTimeSpan("Horario"),

                    Motivo = reader.IsDBNull(
                        reader.GetOrdinal("Motivo"))
                        ? string.Empty
                        : reader.GetString("Motivo"),

                    Ativo = reader.GetBoolean("Ativo")
                };
            }

            return null;
        }
    }
}

