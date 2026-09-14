
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

public class MembresiasViewModel : ViewModelBase
{
    private readonly IMembresiasService _membresiasService;
    private readonly ISociosService _sociosService;

    public ObservableCollection<Membresia> Membresias { get; } = new();

    private Membresia? _membresiaSeleccionada;
    public Membresia? MembresiaSeleccionada
    {
        get => _membresiaSeleccionada;
        set => SetProperty(ref _membresiaSeleccionada, value);
    }

    private bool _cargando;
    public bool Cargando
    {
        get => _cargando;
        set => SetProperty(ref _cargando, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand NuevaCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }

    public MembresiasViewModel(IMembresiasService membresiasService, ISociosService sociosService)
    {
        _membresiasService = membresiasService;
        _sociosService = sociosService;

        CargarCommand = new RelayCommand(async _ => await CargarAsync());
        NuevaCommand = new RelayCommand(async _ => await NuevaAsync());
        EditarCommand = new RelayCommand(async _ => await EditarAsync(), _ => MembresiaSeleccionada != null);
        EliminarCommand = new RelayCommand(async _ => await EliminarAsync(), _ => MembresiaSeleccionada != null);

        _ = CargarAsync();
    }

    private async Task CargarAsync()
    {
        Cargando = true;
        try
        {
            Membresias.Clear();
            var lista = await _membresiasService.ObtenerTodasAsync();
            foreach (var m in lista) Membresias.Add(m);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar membresías: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Cargando = false;
        }
    }

    private async Task NuevaAsync()
    {
        var socios = await _sociosService.ObtenerTodosAsync();
        if (socios.Count == 0)
        {
            MessageBox.Show("Primero debes registrar al menos un socio.",
                "Sin socios", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var ventana = new MembresiaEditWindow(_membresiasService, socios)
        {
            Owner = Application.Current.MainWindow
        };
        ventana.ShowDialog();

        if (ventana.GuardadoExitoso)
            await CargarAsync();
    }

    private async Task EditarAsync()
    {
        if (MembresiaSeleccionada == null) return;

        var socios = await _sociosService.ObtenerTodosAsync();
        var ventana = new MembresiaEditWindow(_membresiasService, socios, MembresiaSeleccionada)
        {
            Owner = Application.Current.MainWindow
        };
        ventana.ShowDialog();

        if (ventana.GuardadoExitoso)
            await CargarAsync();
    }

    private async Task EliminarAsync()
    {
        if (MembresiaSeleccionada == null) return;

        var confirm = MessageBox.Show(
            $"¿Eliminar esta membresía?\n\nSocio: {MembresiaSeleccionada.Socio?.Nombre} {MembresiaSeleccionada.Socio?.Apellido}\nTipo: {MembresiaSeleccionada.Tipo}",
            "Confirmar eliminación",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _membresiasService.EliminarAsync(MembresiaSeleccionada.Id);
            await CargarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}