using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;


namespace SalaoManicure.Repository
{
    public class ProfissionalRepository : IProfissionalRepository
    {
        private readonly string _conexao;

        public ProfissionalRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception("ConnectionString não encontrada.");
        }

        public void Cadastrar(Profissional profissional)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                INSERT INTO Profissionais
                    (Nome, Email, Telefone, Ativo)
                VALUES
                    (@Nome, @Email, @Telefone, @Ativo);
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Nome", profissional.Nome);
            cmd.Parameters.AddWithValue("@Email", profissional.Email);
            cmd.Parameters.AddWithValue("@Telefone", profissional.Telefone);
            cmd.Parameters.AddWithValue("@Ativo", profissional.Ativo);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Profissional profissional)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                UPDATE Profissionais
                SET
                    Nome = @Nome,
                    Email = @Email,
                    Telefone = @Telefone,
                    Ativo = @Ativo
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", profissional.Id);
            cmd.Parameters.AddWithValue("@Nome", profissional.Nome);
            cmd.Parameters.AddWithValue("@Email", profissional.Email);
            cmd.Parameters.AddWithValue("@Telefone", profissional.Telefone);
            cmd.Parameters.AddWithValue("@Ativo", profissional.Ativo);

            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                DELETE FROM Profissionais
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Profissional> Listar()
        {
            var profissionais = new List<Profissional>();

            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                SELECT
                    Id,
                    Nome,
                    Email,
                    Telefone,
                    Ativo
                FROM Profissionais
                ORDER BY Nome;
            ";

            using var cmd = new MySqlCommand(sql, conexao);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                profissionais.Add(new Profissional
                {
                    Id = reader.GetInt32("Id"),

                    Nome = reader.GetString("Nome"),

                    Email = reader.GetString("Email"),

                    Telefone = reader.IsDBNull(
                        reader.GetOrdinal("Telefone"))
                        ? string.Empty
                        : reader.GetString("Telefone"),

                    Ativo = reader.GetBoolean("Ativo")
                });
            }

            return profissionais;
        }

        public Profissional? BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                SELECT
                    Id,
                    Nome,
                    Email,
                    Telefone,
                    Ativo
                FROM Profissionais
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Profissional
                {
                    Id = reader.GetInt32("Id"),

                    Nome = reader.GetString("Nome"),

                    Email = reader.GetString("Email"),

                    Telefone = reader.IsDBNull(
                        reader.GetOrdinal("Telefone"))
                        ? string.Empty
                        : reader.GetString("Telefone"),

                    Ativo = reader.GetBoolean("Ativo")
                };
            }

            return null;
        }
    }
}


