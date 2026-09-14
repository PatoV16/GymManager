using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GymApp.Backend.Entity;
using GymApp.Backend.Interfaces;
using GymApp.Frontend.Helpers;

namespace GymApp.Frontend.ViewModels;

public class ReportesViewModel : ViewModelBase
{
    private readonly ISociosService _sociosService;
    private readonly IMembresiasService _membresiasService;
    private readonly IPagosService _pagosService;

    // Métricas
    private int _sociosActivos;
    public int SociosActivos
    {
        get => _sociosActivos;
        set => SetProperty(ref _sociosActivos, value);
    }

    private int _sociosTotales;
    public int SociosTotales
    {
        get => _sociosTotales;
        set => SetProperty(ref _sociosTotales, value);
    }

    private int _membresiasActivas;
    public int MembresiasActivas
    {
        get => _membresiasActivas;
        set => SetProperty(ref _membresiasActivas, value);
    }

    private decimal _ingresosMes;
    public decimal IngresosMes
    {
        get => _ingresosMes;
        set => SetProperty(ref _ingresosMes, value);
    }

    // Listas
    public ObservableCollection<dynamic> MembresiasPorVencer { get; } = new();
    public ObservableCollection<dynamic> IngresosMensuales { get; } = new();
    public ObservableCollection<dynamic> TopSocios { get; } = new();

    public ICommand CargarCommand { get; }

    public ReportesViewModel(
        ISociosService sociosService,
        IMembresiasService membresiasService,
        IPagosService pagosService)
    {
        _sociosService = sociosService;
        _membresiasService = membresiasService;
        _pagosService = pagosService;

        CargarCommand = new RelayCommand(async _ => await CargarAsync());

        _ = CargarAsync();
    }

    private async Task CargarAsync()
    {
        try
        {
            // Métricas
            SociosActivos = await _sociosService.ContarActivosAsync();
            SociosTotales = await _sociosService.ContarTotalAsync();
            MembresiasActivas = await _membresiasService.ContarActivasAsync();

            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);
            IngresosMes = await _pagosService.TotalIngresosAsync(inicioMes, finMes);

            // Membresías por vencer (próximos 7 días)
            var porVencer = await _membresiasService.ObtenerPorVencerAsync(7);
            MembresiasPorVencer.Clear();
            foreach (var m in porVencer)
            {
                MembresiasPorVencer.Add(new
                {
                    Socio = $"{m.Socio?.Nombre} {m.Socio?.Apellido}",
                    Tipo = m.Tipo,
                    FechaFin = m.FechaFin.ToString("dd/MM/yyyy"),
                    DiasRestantes = (m.FechaFin - DateTime.Today).Days
                });
            }

            // Ingresos por mes (últimos 6)
            var ingresos = await _pagosService.IngresosPorMesAsync(6);
            IngresosMensuales.Clear();
            foreach (var (mes, total) in ingresos)
            {
                IngresosMensuales.Add(new { Mes = mes, Total = total });
            }

            // Top 5 socios
            var top = await _pagosService.TopSociosAsync(5);
            TopSocios.Clear();
            foreach (var (nombre, total) in top)
            {
                TopSocios.Add(new { Nombre = nombre, Total = total });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar reportes: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}