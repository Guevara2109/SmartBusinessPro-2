# Casos de análisis: Creación de contextos y cadenas de conexión en Entity Framework Core
**Caso:** SmartBusinessPro  |  **Herramienta:** Visual Studio 2022, C#, Windows Forms (.NET 8), SQL Server

**Repositorio GitHub:** _(pega aquí tu enlace)_

---

## 1. Código fuente

### 1.1 Entidades
**Categoria.cs**
```csharp
public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<Producto> Productos { get; set; } = new();
}
```
**Producto.cs**
```csharp
public class Producto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    [Column(TypeName = "decimal(18,2)")]
    public decimal Precio { get; set; }
    public int Existencia { get; set; }
    public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
}
```

### 1.2 Contexto de datos (SmartBusinessContext.cs)
```csharp
public class SmartBusinessContext : DbContext
{
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        string? cadena = ConfigurationManager
            .ConnectionStrings["SmartBusinessPro"]?.ConnectionString;

        if (string.IsNullOrWhiteSpace(cadena))
            throw new InvalidOperationException("No se encontró la cadena de conexión.");

        optionsBuilder.UseSqlServer(cadena);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId);
    }
}
```
(El resto del código —`MainForm.cs`, `Program.cs`, `SmartBusinessPro.sql`— está en el repositorio.)

---

## 2. Configuración: cadena de conexión

Definida en `App.config`:
```xml
<connectionStrings>
  <add name="SmartBusinessPro"
       connectionString="Server=(localdb)\MSSQLLocalDB;Database=SmartBusinessPro;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=5;"
       providerName="Microsoft.Data.SqlClient" />
</connectionStrings>
```

| Componente | Explicación |
|---|---|
| `Server` | Instancia de SQL Server a la que se conecta (`(localdb)\MSSQLLocalDB` es la instancia que instala Visual Studio). |
| `Database` | Base de datos a usar: `SmartBusinessPro`. |
| `Trusted_Connection=True` | Autenticación de Windows, sin usuario ni contraseña en el texto. |
| `TrustServerCertificate=True` | Acepta el certificado del servidor (necesario en entornos locales con el cifrado por defecto de Microsoft.Data.SqlClient). |
| `Connect Timeout=5` | Segundos de espera antes de fallar, para que el error aparezca rápido. |
| `name` | Nombre con el que el contexto busca la cadena en `ConfigurationManager`. |

**Proveedor:** `UseSqlServer()` del paquete `Microsoft.EntityFrameworkCore.SqlServer`.

---

## 3. Evidencias
_(Inserta aquí tus capturas)_
- Captura 1: formulario con el mensaje "Conexión exitosa a SmartBusinessPro".
- Captura 2: DataGridView mostrando producto, precio, existencia y categoría.
- Captura 3: tablas con datos en SSMS / Explorador de objetos de SQL Server de Visual Studio.

---

## 4. Pruebas

| # | Prueba | Cambio realizado | Resultado esperado | Resultado obtenido |
|---|---|---|---|---|
| 1 | Conexión válida | Ninguno | "Conexión exitosa" y se cargan 5 productos | _(completa)_ |
| 2 | Servidor incorrecto | `Server=(localdb)\SERVIDORX` | Mensaje: no se pudo encontrar el servidor SQL Server | _(completa)_ |
| 3 | Base de datos incorrecta | `Database=SmartBusinessPro123` | Mensaje: la base de datos no existe o no hay acceso (error 4060) | _(completa)_ |

**Errores encontrados y soluciones**
- *Error de certificado SSL ("la cadena de certificados fue emitida por una entidad no de confianza")* → se agregó `TrustServerCertificate=True`.
- *"No se encontró la cadena de conexión"* → el archivo debe llamarse exactamente `App.config` y estar en la raíz del proyecto; el nombre de la cadena debe coincidir.
- *Tabla no encontrada* → se ejecutó el script `SmartBusinessPro.sql` antes de correr la aplicación.

Los errores se gestionan con `try/catch` sobre `SqlException`, mostrando un mensaje claro según el número de error.

---

## 5. Enlace al repositorio
_(pega aquí el enlace de GitHub)_

## 6. Conclusión
Centralizar el acceso a datos en un contexto (`SmartBusinessContext`) permite que toda la aplicación use una sola configuración de conexión en lugar de repetirla en cada formulario, lo que reduce errores y facilita el mantenimiento. Mantener la cadena de conexión en `App.config` separa la configuración del código: se puede cambiar de servidor o base de datos sin recompilar, y no se exponen datos sensibles en el código fuente. Como documenta Microsoft, en aplicaciones Windows Forms `App.config` es la opción adecuada para almacenar esta configuración.

**Criterio de logro:** el contexto está configurado correctamente y la aplicación recupera y muestra los productos almacenados en SQL Server.
