namespace SmartBusinessPro.Models;

public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // Relación 1:N -> una categoría tiene muchos productos
    public List<Producto> Productos { get; set; } = new();
}
