using GymApp.Backend.Entity;
using GymApp.Backend.Interfaces;
using GymApp.Frontend.Helpers;
using GymApp.Frontend.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GymApp.Frontend.ViewModels;

public class PagosViewModel : ViewModelBase
{
    private readonly IPagosService _pagosService;
    private readonly ISociosService _sociosService;

    public ObservableCollection<Pago> Pagos { get; } = new();

    private Pago? _pagoSeleccionado;
    public Pago? PagoSeleccionado
    {
        get => _pagoSeleccionado;
        set => SetProperty(ref _pagoSeleccionado, value);
    }

    private bool _cargando;
    public bool Cargando
    {
        get => _cargando;
        set => SetProperty(ref _cargando, value);
    }

    private decimal _totalMesActual;
    public decimal TotalMesActual
    {
        get => _totalMesActual;
        set => SetProperty(ref _totalMesActual, value);
    }

    private decimal _totalGeneral;
    public decimal TotalGeneral
    {
        get => _totalGeneral;
        set => SetProperty(ref _totalGeneral, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand EliminarCommand { get; }

    public PagosViewModel(IPagosService pagosService, ISociosService sociosService)
    {
        _pagosService = pagosService;
        _sociosService = sociosService;

        CargarCommand = new RelayCommand(async _ => await CargarAsync());
        NuevoCommand = new RelayCommand(async _ => await NuevoAsync());
        EliminarCommand = new RelayCommand(async _ => await EliminarAsync(), _ => PagoSeleccionado != null);

        _ = CargarAsync();
    }

    private async Task CargarAsync()
    {
        Cargando = true;
        try
        {
            Pagos.Clear();
            var lista = await _pagosService.ObtenerTodosAsync();
            foreach (var p in lista) Pagos.Add(p);

            // Calcular totales
            var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);

            TotalMesActual = await _pagosService.TotalIngresosAsync(inicioMes, finMes);
            TotalGeneral = await _pagosService.TotalIngresosAsync(DateTime.MinValue, DateTime.MaxValue);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar pagos: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Cargando = false;
        }
    }

    private async Task NuevoAsync()
    {
        var socios = await _sociosService.ObtenerTodosAsync();
        if (socios.Count == 0)
        {
            MessageBox.Show("Primero debes registrar al menos un socio.",
                "Sin socios", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var ventana = new PagoEditWindow(_pagosService, socios)
        {
            Owner = Application.Current.MainWindow
        };
        ventana.ShowDialog();

        if (ventana.GuardadoExitoso)
            await CargarAsync();
    }

    private async Task EliminarAsync()
    {
        if (PagoSeleccionado == null) return;

        var confirm = MessageBox.Show(
            $"¿Eliminar este pago?\n\nSocio: {PagoSeleccionado.Socio?.Nombre} {PagoSeleccionado.Socio?.Apellido}\nMonto: {PagoSeleccionado.Monto:C}",
            "Confirmar eliminación",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _pagosService.EliminarAsync(PagoSeleccionado.Id);
            await CargarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}