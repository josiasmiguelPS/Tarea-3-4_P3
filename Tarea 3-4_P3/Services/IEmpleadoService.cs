using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public interface IEmpleadoService
    {
        Task<List<Empleado>> AllUsers();
        Task<int> AddUser(Empleado modelo);
        Task<int> UpdateUser(Empleado modelo);
        Task<int> DeleteUser(int id);
    }
}
