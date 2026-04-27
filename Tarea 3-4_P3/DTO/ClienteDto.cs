namespace Tarea_3_4.DTOs
{
    public class ClienteDto
    {
        public string? RncCedula { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Email { get; set; }
    }
}