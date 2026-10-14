using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartBusinessPro.Data;

namespace SmartBusinessPro;

public class MainForm : Form
{
    private readonly DataGridView dgvProductos = new();
    private readonly Button btnProbar = new();
    private readonly Button btnCargar = new();
    private readonly Label lblEstado = new();

    public MainForm()
    {
        Text = "SmartBusinessPro - Inventario";
        Width = 800;
        Height = 500;
        StartPosition = FormStartPosition.CenterScreen;

        btnProbar.Text = "Probar conexión";
        btnProbar.SetBounds(12, 12, 150, 32);
        btnProbar.Click += BtnProbar_Click;

        btnCargar.Text = "Cargar productos";
        btnCargar.SetBounds(172, 12, 150, 32);
        btnCargar.Click += BtnCargar_Click;

        lblEstado.SetBounds(335, 12, 440, 32);
        lblEstado.TextAlign = ContentAlignment.MiddleLeft;
        lblEstado.Text = "Estado: sin probar";

        dgvProductos.SetBounds(12, 55, 760, 395);
        dgvProductos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvProductos.ReadOnly = true;
        dgvProductos.AllowUserToAddRows = false;
        dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        Controls.AddRange(new Control[] { btnProbar, btnCargar, lblEstado, dgvProductos });
    }

    private async void BtnProbar_Click(object? sender, EventArgs e)
    {
        try
        {
            using var db = new SmartBusinessContext();
            bool ok = await db.Database.CanConnectAsync();

            lblEstado.ForeColor = ok ? Color.Green : Color.Red;
            lblEstado.Text = ok ? "Conexión exitosa a SmartBusinessPro"
                                : "No se pudo conectar a la base de datos";
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    private async void BtnCargar_Click(object? sender, EventArgs e)
    {
        try
        {
            using var db = new SmartBusinessContext();

            var lista = await db.Productos
                .Include(p => p.Categoria)
                .Select(p => new
                {
                    Producto = p.Nombre,
                    Precio = p.Precio,
                    Existencia = p.Existencia,
                    Categoria = p.Categoria!.Nombre
                })
                .ToListAsync();

            dgvProductos.DataSource = lista;
            dgvProductos.Columns["Precio"].DefaultCellStyle.Format = "C2";

            lblEstado.ForeColor = Color.Green;
            lblEstado.Text = $"Se cargaron {lista.Count} productos";
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
    }

    private void MostrarError(Exception ex)
    {
        // Gestión de errores: servidor no disponible, BD incorrecta, etc.
        var sql = ex as SqlException ?? ex.InnerException as SqlException;

        string mensaje = sql switch
        {
            { Number: 4060 } => "La base de datos indicada no existe o no hay acceso a ella.",
            { Number: 53 or 40 or -1 or 2 } => "No se pudo encontrar el servidor SQL Server. Verifique que esté encendido y que el nombre sea correcto.",
            { Number: 18456 } => "Error de autenticación: usuario o contraseña incorrectos.",
            not null => $"Error de SQL Server ({sql.Number}): {sql.Message}",
            _ => $"Error inesperado: {ex.Message}"
        };

        lblEstado.ForeColor = Color.Red;
        lblEstado.Text = "Error de conexión";
        MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
