using Tarea_3_4.Models;

namespace Tarea_3_4.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> ListaProductos();
        Task<Producto?> ObtenerProducto(int id);
        Task<int> AgregarProducto(Producto modelo);
        Task<int> ActualizarProducto(Producto modelo);
        Task<int> EliminarProducto(int id);
    }
}