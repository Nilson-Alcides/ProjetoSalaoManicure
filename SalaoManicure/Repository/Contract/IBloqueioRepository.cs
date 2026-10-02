
using SalaoManicure.Models;


namespace SalaoManicure.Repository.Contract
{
    public interface IBloqueioRepository
    {
        void Cadastrar(Bloqueio bloqueio);

        void Atualizar(Bloqueio bloqueio);

        void Excluir(int id);

        List<Bloqueio> Listar();

        Bloqueio? BuscarPorId(int id);
    }
}

