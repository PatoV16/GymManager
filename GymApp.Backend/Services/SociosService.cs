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

public class SociosService : ISociosService
{
    private readonly GymDbContext _context;

    public SociosService(GymDbContext context)
    {
        _context = context;
    }

    public async Task<List<Socio>> ObtenerTodosAsync()
    {
        return await _context.Socios
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<Socio?> ObtenerPorIdAsync(int id)
    {
        return await _context.Socios
            .Include(s => s.Membresias)
            .Include(s => s.Pagos)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Socio> CrearAsync(Socio socio)
    {
        // Validación básica: cédula no repetida
        var existe = await _context.Socios.AnyAsync(s => s.Cedula == socio.Cedula);
        if (existe)
            throw new InvalidOperationException($"Ya existe un socio con la cédula {socio.Cedula}");

        socio.FechaRegistro = DateTime.Now;
        _context.Socios.Add(socio);
        await _context.SaveChangesAsync();
        return socio;
    }

    public async Task<bool> ActualizarAsync(Socio socio)
    {
        var existente = await _context.Socios.FindAsync(socio.Id);
        if (existente == null) return false;

        // Verificar que la cédula no esté en OTRO socio
        var cedulaDuplicada = await _context.Socios
            .AnyAsync(s => s.Cedula == socio.Cedula && s.Id != socio.Id);
        if (cedulaDuplicada)
            throw new InvalidOperationException($"La cédula {socio.Cedula} ya está en uso");

        existente.Nombre = socio.Nombre;
        existente.Apellido = socio.Apellido;
        existente.Cedula = socio.Cedula;
        existente.Telefono = socio.Telefono;
        existente.Email = socio.Email;
        existente.FechaNacimiento = socio.FechaNacimiento;
        existente.Activo = socio.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var socio = await _context.Socios.FindAsync(id);
        if (socio == null) return false;

        _context.Socios.Remove(socio);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Socio>> BuscarAsync(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return await ObtenerTodosAsync();

        texto = texto.ToLower();
        return await _context.Socios
            .Where(s => s.Nombre.ToLower().Contains(texto)
                     || s.Apellido.ToLower().Contains(texto)
                     || s.Cedula.Contains(texto))
            .OrderBy(s => s.Apellido)
            .ToListAsync();
    }
    public async Task<int> ContarActivosAsync()
    {
        return await _context.Socios.CountAsync(s => s.Activo);
    }

    public async Task<int> ContarTotalAsync()
    {
        return await _context.Socios.CountAsync();
    }
}