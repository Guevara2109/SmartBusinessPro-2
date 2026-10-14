using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBusinessPro.Models;

public class Producto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }

    public int Existencia { get; set; }

    // Llave foránea
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
