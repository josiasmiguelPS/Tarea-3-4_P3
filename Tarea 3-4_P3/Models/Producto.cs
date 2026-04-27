using System.Collections.Generic;

namespace Tarea_3_4.Models;

public partial class Producto
{
    public int Id { get; set; }
    public string? CodigoBarras { get; set; }
    public string Nombre { get; set; } = null!;
    public int CategoriaId { get; set; }
    public decimal Costo { get; set; }
    public decimal Precio { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public bool SeVendePorCaja { get; set; }
    public int UnidadesPorCaja { get; set; }

    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
}