using Microsoft.EntityFrameworkCore;
using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public class ProductoService : IProductoService
    {
        private readonly Dbcrudcore01Context _context;

        public ProductoService(Dbcrudcore01Context context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ListaProductos()
        {
            // Usamos .Include(p => p.Categoria) para traer también el nombre de la categoría,
            // así en el frontend no sale solo un número (ej. CategoriaId: 1), sino "Cuadernos"
            return await _context.Productos
                                 .Include(p => p.Categoria)
                                 .ToListAsync();
        }

        public async Task<Producto?> ObtenerProducto(int id)
        {
            return await _context.Productos
                                 .Include(p => p.Categoria)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> AgregarProducto(Producto modelo)
        {
            await _context.Productos.AddAsync(modelo);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> ActualizarProducto(Producto modelo)
        {
            _context.Productos.Update(modelo);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return 0; // No se encontró el producto
            }

            try
            {
                _context.Productos.Remove(producto);
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // PROTECCIÓN DE DATOS: Si intentas borrar un producto que YA se vendió 
                // y está registrado en una "Factura", SQL Server bloqueará el borrado.
                // Atrapamos el error aquí devolviendo -1 para que la API no colapse.
                return -1;
            }
        }
    }
}