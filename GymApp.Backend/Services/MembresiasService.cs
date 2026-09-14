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

public class MembresiasService : IMembresiasService
{
    private readonly GymDbContext _context;

    public MembresiasService(GymDbContext context)
    {
        _context = context;
    }

    public async Task<List<Membresia>> ObtenerTodasAsync()
    {
        return await _context.Membresias
            .Include(m => m.Socio)
            .OrderByDescending(m => m.FechaInicio)
            .ToListAsync();
    }

    public async Task<List<Membresia>> ObtenerPorSocioAsync(int socioId)
    {
        return await _context.Membresias
            .Where(m => m.SocioId == socioId)
            .OrderByDescending(m => m.FechaInicio)
            .ToListAsync();
    }

    public async Task<Membresia> CrearAsync(Membresia membresia)
    {
        if (membresia.FechaFin <= membresia.FechaInicio)
            throw new InvalidOperationException("La fecha fin debe ser posterior a la fecha inicio");

        _context.Membresias.Add(membresia);
        await _context.SaveChangesAsync();
        return membresia;
    }

    public async Task<bool> ActualizarAsync(Membresia membresia)
    {
        var existente = await _context.Membresias.FindAsync(membresia.Id);
        if (existente == null) return false;

        existente.Tipo = membresia.Tipo;
        existente.FechaInicio = membresia.FechaInicio;
        existente.FechaFin = membresia.FechaFin;
        existente.Precio = membresia.Precio;
        existente.Activa = membresia.Activa;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var membresia = await _context.Membresias.FindAsync(id);
        if (membresia == null) return false;

        _context.Membresias.Remove(membresia);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> ContarActivasAsync()
    {
        var hoy = DateTime.Today;
        return await _context.Membresias
            .CountAsync(m => m.Activa && m.FechaFin >= hoy);
    }

    public async Task<List<Membresia>> ObtenerPorVencerAsync(int dias)
    {
        var hoy = DateTime.Today;
        var limite = hoy.AddDays(dias);
        return await _context.Membresias
            .Include(m => m.Socio)
            .Where(m => m.Activa && m.FechaFin >= hoy && m.FechaFin <= limite)
            .OrderBy(m => m.FechaFin)
            .ToListAsync();
    }

}