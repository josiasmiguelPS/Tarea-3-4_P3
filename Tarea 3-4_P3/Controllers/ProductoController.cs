using Microsoft.AspNetCore.Mvc;
using Tarea_3_4.DTOs;
using Tarea_3_4.Models;
using Tarea_3_4.Services;

namespace Tarea_3_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _Service;

        public ProductoController(IProductoService service)
        {
            _Service = service;
        }

        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> Listar()
        {
            var productos = await _Service.ListaProductos();
            return Ok(productos);
        }

        [HttpGet]
        [Route("Obtener/{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var producto = await _Service.ObtenerProducto(id);
            if (producto == null)
            {
                return NotFound("El producto no existe.");
            }
            return Ok(producto);
        }

        [HttpPost]
        [Route("Agregar")]
        public async Task<IActionResult> Agregar([FromBody] ProductoDto dto)
        {
            // Mapeo manual de DTO a Entidad
            var nuevoProducto = new Producto
            {
                CodigoBarras = dto.CodigoBarras,
                Nombre = dto.Nombre,
                CategoriaId = dto.CategoriaId,
                Costo = dto.Costo,
                Precio = dto.Precio,
                StockActual = dto.StockActual,
                StockMinimo = dto.StockMinimo,
                SeVendePorCaja = dto.SeVendePorCaja,
                UnidadesPorCaja = dto.UnidadesPorCaja
            };

            var filas = await _Service.AgregarProducto(nuevoProducto);
            return Ok(filas);
        }

        [HttpPut]
        [Route("Modificar/{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] ProductoDto dto)
        {
            // Primero buscamos si el producto existe
            var productoExistente = await _Service.ObtenerProducto(id);
            if (productoExistente == null) return NotFound("Producto no encontrado");

            // Actualizamos solo los valores permitidos
            productoExistente.CodigoBarras = dto.CodigoBarras;
            productoExistente.Nombre = dto.Nombre;
            productoExistente.CategoriaId = dto.CategoriaId;
            productoExistente.Costo = dto.Costo;
            productoExistente.Precio = dto.Precio;
            productoExistente.StockActual = dto.StockActual;
            productoExistente.StockMinimo = dto.StockMinimo;
            productoExistente.SeVendePorCaja = dto.SeVendePorCaja;
            productoExistente.UnidadesPorCaja = dto.UnidadesPorCaja;

            var filas = await _Service.ActualizarProducto(productoExistente);
            return Ok(filas);
        }

        [HttpDelete]
        [Route("Eliminar")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var filas = await _Service.EliminarProducto(id);

            if (filas == 0)
            {
                return NotFound("Producto no encontrado.");
            }

            if (filas == -1)
            {
                // Este error ocurre si intentas borrar algo que ya está en una factura
                return BadRequest("Seguridad de Integridad: No se puede eliminar este producto porque ya tiene ventas registradas en el historial.");
            }

            return NoContent();
        }
    }
}