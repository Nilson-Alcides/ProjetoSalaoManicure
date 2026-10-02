using SalaoManicure.Models;


namespace SalaoManicure.Repository.Contract
{
    public interface IServicoRepository
    {
        void Cadastrar(Servico servico);

        void Atualizar(Servico servico);

        void Excluir(int id);

        List<Servico> Listar();

        Servico? BuscarPorId(int id);
    }
}

