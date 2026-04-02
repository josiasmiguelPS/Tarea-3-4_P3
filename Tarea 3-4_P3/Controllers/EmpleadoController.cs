using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Reflection.Emit;
using Tarea_3_4.Models;
using Tarea_3_4.Services;

namespace Tarea_3_4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly IEmpleadoService _Service;
        
        public EmpleadoController(IEmpleadoService service)
        {
            _Service = service;
        }
        [HttpGet]
        [Route("Listar")]
        public async Task<IActionResult> ListarUsuarios()
        { 
            var usuario = await _Service.AllUsers();
            return Ok(usuario);
        }

        [HttpPost]
        [Route("Agregar")]
        public async Task<IActionResult> Agregar([FromBody]Empleado modelo) 
        {
            var filas = await _Service.AddUser(modelo);
            return Ok(filas);
        }

        [HttpPut]
        [Route("Modificar")]
        public async Task<IActionResult> Modificar([FromBody]Empleado modelo)
        {
            var filas = await _Service.UpdateUser(modelo);
            return Ok(filas);
        }
        [HttpDelete]
        [Route("Eliminar")]
        public async Task<IActionResult> Eliminar(int id) 
        {
            var filas = await _Service.DeleteUser(id);
            if (filas == 0)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
