
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
                        INSERT INTO disponibilidades
                            (Data, Horario, Disponivel)
                        VALUES
                            (@Data, @Horario, @Disponivel);";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Data",disponibilidade.Data.Date);
            comando.Parameters.AddWithValue("@Horario",disponibilidade.Horario);
            comando.Parameters.AddWithValue("@Disponivel",disponibilidade.Disponivel);

            comando.ExecuteNonQuery();
        }


        // ============================================================
        // ATUALIZAR
        // ============================================================

        public void Atualizar(Disponibilidade disponibilidade)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"UPDATE Disponibilidades
                            SET
                                Id = @Id, Data = @Data,
                                Horario = @Horario, Disponivel = @Disponivel
                            WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Id",disponibilidade.Id);
            comando.Parameters.AddWithValue("@Data",disponibilidade.Data.Date);
            comando.Parameters.AddWithValue("@Horario",disponibilidade.Horario);
            comando.Parameters.AddWithValue("@Disponivel",disponibilidade.Disponivel);

            comando.ExecuteNonQuery();
        }

        // ============================================================
        // EXCLUIR
        // ============================================================

        public void Excluir(int id)
        {
            using var conexao = new MySqlConnection(_conexao);

            conexao.Open();

            string sql = @"DELETE FROM Disponibilidades
                             WHERE Id = @Id;";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Id",id);

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
               SELECT d.Id, d.Data, d.Horario, d.Disponivel
                FROM Disponibilidades d
               WHERE d.Disponivel = 1
                  AND d.Data >= CURDATE()  
                  ORDER BY
                     d.Data,
                     d.Horario;";

            using var comando = new MySqlCommand(sql, conexao);

            using var reader = comando.ExecuteReader();

            while (reader.Read())
            {
                var disponibilidade =
                    new Disponibilidade
                    {
                        Id = reader.GetInt32("Id"), 
                        Data = reader.GetDateTime("Data"),
                        Horario = reader.GetTimeSpan("Horario"),
                        Disponivel = reader.GetBoolean("Disponivel")
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
                SELECT  d.Id, d.Data, d.Horario, d.Disponivel  FROM Disponibilidades d 
                 WHERE d.Disponivel = 1 AND d.Data >= CURDATE()  AND d.Id = 25
                 ORDER BY 
                   d.Data, d.Horario; ";

            using var comando = new MySqlCommand(sql, conexao);

            comando.Parameters.AddWithValue("@Id",id);

            using var reader = comando.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            return new Disponibilidade
            {
                Id = reader.GetInt32("Id"), 
                Data = reader.GetDateTime("Data"),
                Horario = reader.GetTimeSpan("Horario"),
                Disponivel = reader.GetBoolean("Disponivel")
            };
        }
    }
}
