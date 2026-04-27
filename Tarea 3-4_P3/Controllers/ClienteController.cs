using Microsoft.AspNetCore.Mvc;
using Tarea_3_4.DTOs;
using Tarea_3_4.Models;
using Tarea_3_4.Services;

namespace Tarea_3_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;
        public ClienteController(IClienteService service) => _service = service;

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar() => Ok(await _service.Listar());

        [HttpPost("Agregar")]
        public async Task<IActionResult> Agregar([FromBody] ClienteDto dto)
        {
            var nuevoCliente = new Cliente
            {
                RncCedula = dto.RncCedula,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                Email = dto.Email
            };
            return Ok(await _service.Agregar(nuevoCliente));
        }

        [HttpPut("Modificar/{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] ClienteDto dto)
        {
            var cliente = await _service.Obtener(id);
            if (cliente == null) return NotFound("Cliente no encontrado.");

            cliente.RncCedula = dto.RncCedula;
            cliente.Nombre = dto.Nombre;
            cliente.Telefono = dto.Telefono;
            cliente.Direccion = dto.Direccion;
            cliente.Email = dto.Email;

            return Ok(await _service.Modificar(cliente));
        }

        [HttpDelete("Eliminar/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _service.Eliminar(id);
            if (resultado == 0) return NotFound();
            if (resultado == -1) return BadRequest("No se puede eliminar porque este cliente tiene facturas registradas.");
            return NoContent();
        }
    }
}