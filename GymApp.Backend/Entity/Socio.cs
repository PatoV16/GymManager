using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Entity;

public class Socio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;

    // Propiedades de navegación (relaciones)
    public ICollection<Membresia> Membresias { get; set; } = new List<Membresia>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
