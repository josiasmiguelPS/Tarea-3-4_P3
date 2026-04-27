namespace Tarea_3_4.Models;

public partial class DetalleFactura
{
    public int Id { get; set; }
    public int FacturaId { get; set; }
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal SubTotal { get; set; }

    public virtual Factura Factura { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}