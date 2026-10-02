using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

namespace SalaoManicure.Repository
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly string _conexao;

        public ClienteRepository(IConfiguration config)
        {
            _conexao = config.GetConnectionString("ConexaoMySQL")
                ?? throw new Exception("ConnectionString não encontrada.");
        }

        // ==========================================
        // CADASTRAR
        // ==========================================

        public void Cadastrar(Cliente cliente)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"INSERT INTO Clientes
                    (Nome, Email, Telefone, DataCadastro)
                VALUES
                    (@Nome, @Email, @Telefone, @DataCadastro);
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@Email", cliente.Email);
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
            cmd.Parameters.AddWithValue("@DataCadastro", cliente.DataCadastro);

            cmd.ExecuteNonQuery();
        }

        // ==========================================
        // ATUALIZAR
        // ==========================================

        public void Atualizar(Cliente cliente)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"UPDATE Clientes
                        SET
                            Nome = @Nome,
                            Email = @Email,
                            Telefone = @Telefone
                        WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", cliente.Id);
            cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@Email", cliente.Email);
            cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);

            cmd.ExecuteNonQuery();
        }

        // ==========================================
        // EXCLUIR
        // ==========================================

        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                DELETE FROM Clientes
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", id);

            cmd.ExecuteNonQuery();
        }

        // ==========================================
        // LISTAR
        // ==========================================

        public List<Cliente> Listar()
        {
            var clientes = new List<Cliente>();

            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"SELECT
                        Id,
                        Nome,
                        Email,
                        Telefone,
                        DataCadastro
                    FROM Clientes
                    ORDER BY Nome;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                clientes.Add(new Cliente
                {
                    Id = reader.GetInt32("Id"),
                    Nome = reader.GetString("Nome"),
                    Email = reader.GetString("Email"),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("Telefone"))
                        ? string.Empty
                        : reader.GetString("Telefone"),
                    DataCadastro = reader.GetDateTime("DataCadastro")
                });
            }

            return clientes;
        }

        // ==========================================
        // BUSCAR POR ID
        // ==========================================

        public Cliente? BuscarPorId(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"
                SELECT
                    Id,
                    Nome,
                    Email,
                    Telefone,
                    DataCadastro
                FROM Clientes
                WHERE Id = @Id;
            ";

            using var cmd = new MySqlCommand(sql, conexao);

            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Cliente
                {
                    Id = reader.GetInt32("Id"),
                    Nome = reader.GetString("Nome"),
                    Email = reader.GetString("Email"),
                    Telefone = reader.IsDBNull(reader.GetOrdinal("Telefone"))
                        ? string.Empty
                        : reader.GetString("Telefone"),
                    DataCadastro = reader.GetDateTime("DataCadastro")
                };
            }

            return null;
        }
    }
}
