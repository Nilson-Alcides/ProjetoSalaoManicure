
using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;



namespace SalaoManicure.Repository
{
    public class DisponibilidadeRepository : IDisponibilidadeRepository
    {
        private readonly string _conexao;

        public DisponibilidadeRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception(
                    "ConnectionString não encontrada.");
        }

        // ============================================================
        // CADASTRAR
        // ============================================================

        public void Cadastrar(Disponibilidade disponibilidade)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                INSERT INTO Disponibilidades
                    (ProfissionalId, Data, Horario, Ativo)
                VALUES
                    (@ProfissionalId, @Data, @Horario, @Ativo);";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@ProfissionalId",
                disponibilidade.ProfissionalId);

            comando.Parameters.AddWithValue(
                "@Data",
                disponibilidade.Data.Date);

            comando.Parameters.AddWithValue(
                "@Horario",
                disponibilidade.Horario);

            comando.Parameters.AddWithValue(
                "@Ativo",
                disponibilidade.Ativo);

            comando.ExecuteNonQuery();
        }

        // ============================================================
        // ATUALIZAR
        // ============================================================

        public void Atualizar(Disponibilidade disponibilidade)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                UPDATE Disponibilidades
                SET
                    ProfissionalId = @ProfissionalId,
                    Data = @Data,
                    Horario = @Horario,
                    Ativo = @Ativo
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Id",
                disponibilidade.Id);

            comando.Parameters.AddWithValue(
                "@ProfissionalId",
                disponibilidade.ProfissionalId);

            comando.Parameters.AddWithValue(
                "@Data",
                disponibilidade.Data.Date);

            comando.Parameters.AddWithValue(
                "@Horario",
                disponibilidade.Horario);

            comando.Parameters.AddWithValue(
                "@Ativo",
                disponibilidade.Ativo);

            comando.ExecuteNonQuery();
        }

        // ============================================================
        // EXCLUIR
        // ============================================================

        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                DELETE FROM Disponibilidades
                WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Id",
                id);

            comando.ExecuteNonQuery();
        }

        // ============================================================
        // LISTAR
        // ============================================================

        public List<Disponibilidade> Listar()
        {
            var disponibilidades =
                new List<Disponibilidade>();

            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    d.Id,
                    d.ProfissionalId,
                    p.Nome AS ProfissionalNome,
                    d.Data,
                    d.Horario,
                    d.Ativo
                FROM Disponibilidades d

                INNER JOIN Profissionais p
                    ON p.Id = d.ProfissionalId

                ORDER BY
                    d.Data,
                    d.Horario,
                    p.Nome;";

            using var comando = new MySqlCommand(sql, conexao);

            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                var disponibilidade =
                    new Disponibilidade
                    {
                        Id = reader.GetInt32("Id"),

                        ProfissionalId =
                            reader.GetInt32("ProfissionalId"),

                        Profissional = new Profissional
                        {
                            Id = reader.GetInt32("ProfissionalId"),

                            Nome =
                                reader.GetString(
                                    "ProfissionalNome")
                        },

                        Data =
                            reader.GetDateTime("Data"),

                        Horario =
                            reader.GetTimeSpan("Horario"),

                        Ativo =
                            reader.GetBoolean("Ativo")
                    };

                disponibilidades.Add(disponibilidade);
            }

            return disponibilidades;
        }

        // ============================================================
        // BUSCAR POR ID
        // ============================================================

        public Disponibilidade? BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    d.Id,
                    d.ProfissionalId,
                    p.Nome AS ProfissionalNome,
                    d.Data,
                    d.Horario,
                    d.Ativo
                FROM Disponibilidades d

                INNER JOIN Profissionais p
                    ON p.Id = d.ProfissionalId

                WHERE d.Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue(
                "@Id",
                id);

            using var reader = comando.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Disponibilidade
            {
                Id = reader.GetInt32("Id"),

                ProfissionalId =
                    reader.GetInt32("ProfissionalId"),

                Profissional = new Profissional
                {
                    Id =
                        reader.GetInt32("ProfissionalId"),

                    Nome =
                        reader.GetString(
                            "ProfissionalNome")
                },

                Data =
                    reader.GetDateTime("Data"),

                Horario =
                    reader.GetTimeSpan("Horario"),

                Ativo =
                    reader.GetBoolean("Ativo")
            };
        }
    }
}
