
using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Repository
{
    public class ServicoRepository : IServicoRepository
    {
        private readonly string _conexao;

        public ServicoRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception("ConnectionString não encontrada.");
        }

        public void Cadastrar(Servico servico)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                INSERT INTO Servicos
                    (Nome, Descricao, Preco, DuracaoMinutos, Ativo)
                VALUES
                    (@Nome, @Descricao, @Preco, @DuracaoMinutos, @Ativo);
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Nome", servico.Nome);
            cmd.Parameters.AddWithValue("@Descricao", servico.Descricao);
            cmd.Parameters.AddWithValue("@Preco", servico.Preco);
            cmd.Parameters.AddWithValue(
                "@DuracaoMinutos",
                servico.DuracaoMinutos);
            cmd.Parameters.AddWithValue("@Ativo", servico.Ativo);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Servico servico)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                UPDATE Servicos
                SET
                    Nome = @Nome,
                    Descricao = @Descricao,
                    Preco = @Preco,
                    DuracaoMinutos = @DuracaoMinutos,
                    Ativo = @Ativo
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", servico.Id);
            cmd.Parameters.AddWithValue("@Nome", servico.Nome);
            cmd.Parameters.AddWithValue("@Descricao", servico.Descricao);
            cmd.Parameters.AddWithValue("@Preco", servico.Preco);
            cmd.Parameters.AddWithValue(
                "@DuracaoMinutos",
                servico.DuracaoMinutos);
            cmd.Parameters.AddWithValue("@Ativo", servico.Ativo);

            cmd.ExecuteNonQuery();
        }

        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                DELETE FROM Servicos
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        public List<Servico> Listar()
        {
            var servicos = new List<Servico>();

            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                SELECT
                    Id,
                    Nome,
                    Descricao,
                    Preco,
                    DuracaoMinutos,
                    Ativo
                FROM Servicos
                ORDER BY Nome;
            ";

            using var cmd = new MySqlCommand(sql, conexao);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                servicos.Add(new Servico
                {
                    Id = reader.GetInt32("Id"),

                    Nome = reader.GetString("Nome"),

                    Descricao = reader.IsDBNull(
                        reader.GetOrdinal("Descricao"))
                        ? string.Empty
                        : reader.GetString("Descricao"),

                    Preco = reader.GetDecimal("Preco"),

                    DuracaoMinutos =
                        reader.GetInt32("DuracaoMinutos"),

                    Ativo = reader.GetBoolean("Ativo")
                });
            }

            return servicos;
        }

        public Servico? BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_conexao);
            conexao.Open();

            string sql = @"
                SELECT
                    Id,
                    Nome,
                    Descricao,
                    Preco,
                    DuracaoMinutos,
                    Ativo
                FROM Servicos
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Servico
                {
                    Id = reader.GetInt32("Id"),

                    Nome = reader.GetString("Nome"),

                    Descricao = reader.IsDBNull(
                        reader.GetOrdinal("Descricao"))
                        ? string.Empty
                        : reader.GetString("Descricao"),

                    Preco = reader.GetDecimal("Preco"),

                    DuracaoMinutos =
                        reader.GetInt32("DuracaoMinutos"),

                    Ativo = reader.GetBoolean("Ativo")
                };
            }

            return null;
        }
    }
}
