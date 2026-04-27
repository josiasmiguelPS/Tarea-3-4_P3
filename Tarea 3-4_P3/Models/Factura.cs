using System;
using System.Collections.Generic;

namespace Tarea_3_4.Models;

public partial class Factura
{
    public int Id { get; set; }
    public string NumeroFactura { get; set; } = null!;
    public string? NCF { get; set; }
    public string? TipoNCF { get; set; }
    public DateTime Fecha { get; set; }
    public int? ClienteId { get; set; }
    public int EmpleadoId { get; set; }
    public decimal Total { get; set; }
    public string MetodoPago { get; set; } = null!;

    public virtual Cliente? Cliente { get; set; }
    public virtual Empleado Empleado { get; set; } = null!;
    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
}