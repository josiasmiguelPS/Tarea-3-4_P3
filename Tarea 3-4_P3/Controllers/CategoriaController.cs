using Microsoft.AspNetCore.Mvc;
using Tarea_3_4.DTOs;
using Tarea_3_4.Models;
using Tarea_3_4.Services;

namespace Tarea_3_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;
        public CategoriaController(ICategoriaService service) => _service = service;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar() => Ok(await _service.Listar());

        [HttpPost("Agregar")]
        public async Task<IActionResult> Agregar([FromBody] CategoriaDto dto)
        {
            var nuevaCategoria = new Categoria { Nombre = dto.Nombre, Descripcion = dto.Descripcion };
            return Ok(await _service.Agregar(nuevaCategoria));
        }

        [HttpPut("Modificar/{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] CategoriaDto dto)
        {
            var categoria = await _service.Obtener(id);
            if (categoria == null) return NotFound("Categoría no encontrada.");

            categoria.Nombre = dto.Nombre;
            categoria.Descripcion = dto.Descripcion;

            return Ok(await _service.Modificar(categoria));
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _service.Eliminar(id);
            if (resultado == 0) return NotFound();
            if (resultado == -1) return BadRequest("No se puede eliminar porque tiene productos asignados.");
            return NoContent();
        }
    }
}