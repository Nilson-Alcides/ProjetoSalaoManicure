
using MySql.Data.MySqlClient;
using SalaoManicure.Models;
using SalaoManicure.Repository.Contract;

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

        string sql = @"
            INSERT INTO Clientes
            (
                UsuarioId,
                Nome,
                Email,
                Telefone,
                DataCadastro
            )
            VALUES
            (
                @UsuarioId,
                @Nome,
                @Email,
                @Telefone,
                @DataCadastro
            );
        ";

        using var cmd = new MySqlCommand(sql, conexao);

        cmd.Parameters.AddWithValue("@UsuarioId", cliente.UsuarioId);
        cmd.Parameters.AddWithValue("@Nome", cliente.Nome);
        cmd.Parameters.AddWithValue("@Email", cliente.Email);
        cmd.Parameters.AddWithValue("@Telefone", cliente.Telefone);
        cmd.Parameters.AddWithValue("@DataCadastro", cliente.DataCadastro);

        cmd.ExecuteNonQuery();

        cliente.Id = (int)cmd.LastInsertedId;
    }

    // ==========================================
    // ATUALIZAR
    // ==========================================

    public void Atualizar(Cliente cliente)
    {
        using var conexao = new MySqlConnection(_conexao);

        conexao.Open();

        string sql = @"
            UPDATE Clientes
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

        string sql = @"
            SELECT
                Id,
                UsuarioId,
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
                UsuarioId = reader.GetInt32("UsuarioId"),

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
                UsuarioId,
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

                UsuarioId = reader.GetInt32("UsuarioId"),

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

    // ==========================================
    // BUSCAR POR USUARIO
    // ==========================================

    public Cliente? BuscarPorUsuarioId(int usuarioId)
    {
        using var conexao = new MySqlConnection(_conexao);

        conexao.Open();

        string sql = @"
            SELECT
                Id,
                UsuarioId,
                Nome,
                Email,
                Telefone,
                DataCadastro
            FROM Clientes
            WHERE UsuarioId = @UsuarioId
            LIMIT 1;
        ";

        using var cmd = new MySqlCommand(sql, conexao);

        cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Cliente
            {
                Id = reader.GetInt32("Id"),

                UsuarioId = reader.GetInt32("UsuarioId"),

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
