using GymApp.Backend.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Interfaces;

public interface IPagosService
{
    Task<List<Pago>> ObtenerTodosAsync();
    Task<List<Pago>> ObtenerPorSocioAsync(int socioId);
    Task<Pago> CrearAsync(Pago pago);
    Task<bool> EliminarAsync(int id);
    Task<decimal> TotalIngresosAsync(DateTime desde, DateTime hasta);
    Task<List<(string Mes, decimal Total)>> IngresosPorMesAsync(int meses);
    Task<List<(string NombreSocio, decimal Total)>> TopSociosAsync(int cantidad);
}
