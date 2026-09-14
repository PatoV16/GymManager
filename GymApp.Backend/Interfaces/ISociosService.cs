using GymApp.Backend.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Interfaces;

public interface ISociosService
{
    Task<List<Socio>> ObtenerTodosAsync();
    Task<Socio?> ObtenerPorIdAsync(int id);
    Task<Socio> CrearAsync(Socio socio);
    Task<bool> ActualizarAsync(Socio socio);
    Task<bool> EliminarAsync(int id);
    Task<List<Socio>> BuscarAsync(string texto);
    Task<int> ContarActivosAsync();
    Task<int> ContarTotalAsync();
}
