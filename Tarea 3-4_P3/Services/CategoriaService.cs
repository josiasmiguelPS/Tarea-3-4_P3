using Microsoft.EntityFrameworkCore;
using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly Dbcrudcore01Context _context;
        public CategoriaService(Dbcrudcore01Context context) => _context = context;

        public async Task<List<Categoria>> Listar() => await _context.Categorias.ToListAsync();
        public async Task<Categoria?> Obtener(int id) => await _context.Categorias.FindAsync(id);

        public async Task<int> Agregar(Categoria modelo)
        {
            await _context.Categorias.AddAsync(modelo);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Modificar(Categoria modelo)
        {
            _context.Categorias.Update(modelo);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Eliminar(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return 0;

            try
            {
                _context.Categorias.Remove(categoria);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return -1; // Retorna -1 si la categoría ya tiene productos asignados
            }
        }
    }
}