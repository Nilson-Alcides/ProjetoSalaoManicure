
using SalaoManicure.Models;

namespace SalaoManicure.Repository.Contract
{
    public interface IAgendamentoRepository
    {
        int Cadastrar(Agendamento agendamento);

        void Atualizar(Agendamento agendamento);

        void Excluir(int id);

        List<Agendamento> Listar();

        Agendamento? BuscarPorId(int id);

        List<Agendamento> ListarPorCliente(int clienteId);
    }
}
