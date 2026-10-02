using SalaoManicure.Models;

namespace SalaoManicure.Repository.Contract
{
    public interface IClienteRepository
    {
        void Cadastrar(Cliente cliente);

        void Atualizar(Cliente cliente);

        void Excluir(int id);

        List<Cliente> Listar();

        Cliente? BuscarPorId(int id);
    }
}
