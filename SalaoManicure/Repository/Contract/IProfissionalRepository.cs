using SalaoManicure.Models;

namespace SalaoManicure.Repository.Contract
{
    public interface IProfissionalRepository
    {
        void Cadastrar(Profissional profissional); 
        void Atualizar(Profissional profissional); 
        void Excluir(int id); List<Profissional> 
        Listar(); Profissional? BuscarPorId(int id);
    }
}
