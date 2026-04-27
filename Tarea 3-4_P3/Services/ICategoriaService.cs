using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public interface ICategoriaService
    {
        Task<List<Categoria>> Listar();
        Task<Categoria?> Obtener(int id);
        Task<int> Agregar(Categoria modelo);
        Task<int> Modificar(Categoria modelo);
        Task<int> Eliminar(int id);
    }

    public interface IClienteService
    {
        Task<List<Cliente>> Listar();
        Task<Cliente?> Obtener(int id);
        Task<int> Agregar(Cliente modelo);
        Task<int> Modificar(Cliente modelo);
        Task<int> Eliminar(int id);
    }
}