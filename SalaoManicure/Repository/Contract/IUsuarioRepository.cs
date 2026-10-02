using SalaoManicure.Models;

namespace SalaoManicure.Repositories
{
    public interface IUsuarioRepository
    {
        Usuario? Login(string email, string senha);

        Usuario? BuscarPorEmail(string email);

        void Cadastrar(Usuario usuario);

        Usuario? BuscarPorId(int id);

        void Atualizar(Usuario usuario);
    }
}

