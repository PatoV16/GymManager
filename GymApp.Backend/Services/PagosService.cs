using GymApp.Backend.Data;
using GymApp.Backend.Entity;
using GymApp.Backend.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymApp.Backend.Services;

public class PagosService : IPagosService
{
    private readonly GymDbContext _context;

    public PagosService(GymDbContext context)
    {
        _context = context;
    }

    public async Task<List<Pago>> ObtenerTodosAsync()
    {
        return await _context.Pagos
            .Include(p => p.Socio)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<List<Pago>> ObtenerPorSocioAsync(int socioId)
    {
        return await _context.Pagos
            .Where(p => p.SocioId == socioId)
            .OrderByDescending(p => p.FechaPago)
            .ToListAsync();
    }

    public async Task<Pago> CrearAsync(Pago pago)
    {
        if (pago.Monto <= 0)
            throw new InvalidOperationException("El monto debe ser mayor a cero");

        pago.FechaPago = DateTime.Now;
        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();
        return pago;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago == null) return false;

        _context.Pagos.Remove(pago);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> TotalIngresosAsync(DateTime desde, DateTime hasta)
    {
        // Traemos solo los montos a memoria y luego sumamos con LINQ to Objects
        var montos = await _context.Pagos
            .Where(p => p.FechaPago >= desde && p.FechaPago <= hasta)
            .Select(p => p.Monto)
            .ToListAsync();

        return montos.Sum();
    }
    public async Task<List<(string Mes, decimal Total)>> IngresosPorMesAsync(int meses)
    {
        var desde = DateTime.Today.AddMonths(-meses + 1);
        desde = new DateTime(desde.Year, desde.Month, 1);

        // Traer a memoria para agrupar (SQLite no soporta bien esto con decimal)
        var pagos = await _context.Pagos
            .Where(p => p.FechaPago >= desde)
            .Select(p => new { p.FechaPago, p.Monto })
            .ToListAsync();

        var resultado = pagos
            .GroupBy(p => new { p.FechaPago.Year, p.FechaPago.Month })
            .Select(g => new
            {
                Fecha = new DateTime(g.Key.Year, g.Key.Month, 1),
                Total = g.Sum(p => p.Monto)
            })
            .OrderBy(x => x.Fecha)
            .Select(x => (
                Mes: x.Fecha.ToString("MMM yyyy", new System.Globalization.CultureInfo("es-ES")),
                Total: x.Total
            ))
            .ToList();

        return resultado;
    }

    public async Task<List<(string NombreSocio, decimal Total)>> TopSociosAsync(int cantidad)
    {
        var pagos = await _context.Pagos
            .Include(p => p.Socio)
            .Select(p => new { p.SocioId, p.Monto, p.Socio!.Nombre, p.Socio.Apellido })
            .ToListAsync();

        var resultado = pagos
            .GroupBy(p => new { p.SocioId, p.Nombre, p.Apellido })
            .Select(g => new
            {
                Nombre = $"{g.Key.Nombre} {g.Key.Apellido}",
                Total = g.Sum(p => p.Monto)
            })
            .OrderByDescending(x => x.Total)
            .Take(cantidad)
            .Select(x => (NombreSocio: x.Nombre, Total: x.Total))
            .ToList();

        return resultado;
    }
}
