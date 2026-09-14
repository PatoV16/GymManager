using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Entity;


public class Pago
{
    public int Id { get; set; }
    public int SocioId { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.Now;
    public string MetodoPago { get; set; } = "Efectivo"; // Efectivo, Tarjeta, Transferencia
    public string? Observaciones { get; set; }

    // Navegación
    public Socio? Socio { get; set; }
}
