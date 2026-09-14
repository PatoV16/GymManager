using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Entity;


public class Membresia
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public string Tipo { get; set; } = "Mensual"; // Mensual, Trimestral, Anual
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal Precio { get; set; }
    public bool Activa { get; set; } = true;

    // Navegación: una membresía pertenece a un socio
    public Socio? Socio { get; set; }
}
