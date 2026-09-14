
using GymApp.Backend.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Interfaces;

public interface IMembresiasService
{
    Task<List<Membresia>> ObtenerTodasAsync();
    Task<List<Membresia>> ObtenerPorSocioAsync(int socioId);
    Task<Membresia> CrearAsync(Membresia membresia);
    Task<bool> ActualizarAsync(Membresia membresia);
    Task<bool> EliminarAsync(int id);
    Task<int> ContarActivasAsync();
    Task<List<Membresia>> ObtenerPorVencerAsync(int dias);
}
