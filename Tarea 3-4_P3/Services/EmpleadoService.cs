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

        public async Task<List<Empleado>> AllUsers()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<int> DeleteUser(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) 
            {
                return 0;
            }
            _context.Empleados.Remove(empleado);
            return await _context.SaveChangesAsync();
        }

        
    }
}
