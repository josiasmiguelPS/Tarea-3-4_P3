using Microsoft.EntityFrameworkCore;
using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly Dbcrudcore01Context _context;
        private readonly DbSet<Empleado> _dbSet;

        public EmpleadoService(Dbcrudcore01Context context)
        {
            _context = context;
            _dbSet = context.Set<Empleado>();
        }

        public async Task<int> AddUser(Empleado modelo)
        {
            await _context.AddAsync(modelo);
            int filasAfectadas = await _context.SaveChangesAsync();
            return filasAfectadas;
        }

        
    }
}
