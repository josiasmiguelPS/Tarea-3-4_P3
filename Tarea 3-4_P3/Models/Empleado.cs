using System;
using System.Collections.Generic;

namespace Tarea_3_4.Models;

public partial class Empleado
{
    public int ID { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public int? Edad { get; set; }

    public string? Direccion { get; set; }

    public string? Numero { get; set; }

    public string? Email { get; set; }
}
