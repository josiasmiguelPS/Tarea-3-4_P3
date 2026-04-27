using System.Collections.Generic;

namespace Tarea_3_4.Models;

public partial class Cliente
{
    public int Id { get; set; }
    public string? RncCedula { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string? Email { get; set; }

    public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
}