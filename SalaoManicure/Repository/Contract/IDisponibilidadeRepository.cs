
using SalaoManicure.Models;


namespace SalaoManicure.Repository.Contract
{
    public interface IDisponibilidadeRepository
    {
        void Cadastrar(Disponibilidade disponibilidade);

        void Atualizar(Disponibilidade disponibilidade);

        void Excluir(int id);

        List<Disponibilidade> Listar();

        Disponibilidade? BuscarPorId(int id);
    }
}

