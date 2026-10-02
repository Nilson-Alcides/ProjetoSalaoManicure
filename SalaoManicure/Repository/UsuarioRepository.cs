
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Generators;
using SalaoManicure.Models;
using SalaoManicure.Models.Constants;

namespace SalaoManicure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly string _conexao;

        public UsuarioRepository(IConfiguration configuration)
        {
            _conexao = configuration.GetConnectionString("ConexaoMySQL")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' não encontrada."); 
        }

        // =========================================================
        // LOGIN
        // =========================================================
        public Usuario? Login(string email, string senha)
        {
            using var connection = new MySqlConnection(_conexao);

            connection.Open();

            const string sql = @"
                SELECT
                    id,
                    nome,
                    email,
                    senhaHash,
                    perfil,
                    ativo,
                    dataCadastro
                FROM usuarios
                WHERE email = @email
                AND ativo = 1
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@email", email);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            string senhaHash = reader["senhaHash"]?.ToString() ?? string.Empty;

            // Validação da senha
            if (!BCrypt.Net.BCrypt.Verify(senha, senhaHash))
                return null;

            return new Usuario
            {
                Id = Convert.ToInt32(reader["id"]),
                Nome = reader["nome"]?.ToString() ?? string.Empty,
                Email = reader["email"]?.ToString() ?? string.Empty,
                SenhaHash = senhaHash,
                Perfil = reader["perfil"]?.ToString() ?? string.Empty,
                Ativo = Convert.ToBoolean(reader["ativo"]),
                DataCadastro = Convert.ToDateTime(reader["dataCadastro"])
            };
        }


        // =========================================================
        // BUSCAR POR EMAIL
        // =========================================================
        public Usuario? BuscarPorEmail(string email)
        {
            using var connection = new MySqlConnection(_conexao);

            connection.Open();

            const string sql = @"
                SELECT
                    id,
                    nome,
                    email,
                    senhaHash,
                    perfil,
                    ativo,
                    dataCadastro
                FROM usuarios
                WHERE email = @email
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@email", email);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Usuario
            {
                Id = Convert.ToInt32(reader["id"]),
                Nome = reader["nome"]?.ToString() ?? string.Empty,
                Email = reader["email"]?.ToString() ?? string.Empty,
                SenhaHash = reader["senhaHash"]?.ToString() ?? string.Empty,
                Perfil = reader["perfil"]?.ToString() ?? string.Empty,
                Ativo = Convert.ToBoolean(reader["ativo"]),
                DataCadastro = Convert.ToDateTime(reader["dataCadastro"])
            };
        }


        // =========================================================
        // CADASTRAR
        // =========================================================
        public void Cadastrar(Usuario usuario)
        {
            using var connection = new MySqlConnection(_conexao);

            connection.Open();

            // O perfil NÃO vem do formulário.
            // O cadastro público sempre será Cliente.
            usuario.Perfil = PerfisUsuario.Cliente;
            usuario.Ativo = true;
            usuario.DataCadastro = DateTime.Now;

            const string sql = @"
                INSERT INTO usuarios
                (
                    nome,
                    email,
                    senhaHash,
                    perfil,
                    ativo,
                    dataCadastro
                )
                VALUES
                (
                    @nome,
                    @email,
                    @senhaHash,
                    @perfil,
                    @ativo,
                    @dataCadastro
                )";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@nome", usuario.Nome);
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@senhaHash", usuario.SenhaHash);
            command.Parameters.AddWithValue("@perfil", usuario.Perfil);
            command.Parameters.AddWithValue("@ativo", usuario.Ativo);
            command.Parameters.AddWithValue("@dataCadastro", usuario.DataCadastro);

            command.ExecuteNonQuery();

            usuario.Id = (int)command.LastInsertedId;
        }


        // =========================================================
        // BUSCAR POR ID
        // =========================================================
        public Usuario? BuscarPorId(int id)
        {
            using var connection = new MySqlConnection(_conexao);

            connection.Open();

            const string sql = @"
                SELECT
                    id,
                    nome,
                    email,
                    senhaHash,
                    perfil,
                    ativo,
                    dataCadastro
                FROM usuarios
                WHERE id = @id
                LIMIT 1";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", id);

            using var reader = command.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Usuario
            {
                Id = Convert.ToInt32(reader["id"]),
                Nome = reader["nome"]?.ToString() ?? string.Empty,
                Email = reader["email"]?.ToString() ?? string.Empty,
                SenhaHash = reader["senhaHash"]?.ToString() ?? string.Empty,
                Perfil = reader["perfil"]?.ToString() ?? string.Empty,
                Ativo = Convert.ToBoolean(reader["ativo"]),
                DataCadastro = Convert.ToDateTime(reader["dataCadastro"])
            };
        }


        // =========================================================
        // ATUALIZAR
        // =========================================================
        public void Atualizar(Usuario usuario)
        {
            using var connection = new MySqlConnection(_conexao);

            connection.Open();

            const string sql = @"
                UPDATE usuarios
                SET
                    nome = @nome,
                    email = @email,
                    perfil = @perfil,
                    ativo = @ativo
                WHERE id = @id";

            using var command = new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@id", usuario.Id);
            command.Parameters.AddWithValue("@nome", usuario.Nome);
            command.Parameters.AddWithValue("@email", usuario.Email);
            command.Parameters.AddWithValue("@perfil", usuario.Perfil);
            command.Parameters.AddWithValue("@ativo", usuario.Ativo);

            command.ExecuteNonQuery();
        }
    }
}

