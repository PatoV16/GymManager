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

public class SociosViewModel : ViewModelBase
{
    private readonly ISociosService _sociosService;

    public ObservableCollection<Socio> Socios { get; } = new();

    private Socio? _socioSeleccionado;
    public Socio? SocioSeleccionado
    {
        get => _socioSeleccionado;
        set => SetProperty(ref _socioSeleccionado, value);
    }

    private string _textoBusqueda = string.Empty;
    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set
        {
            if (SetProperty(ref _textoBusqueda, value))
                _ = BuscarAsync();
        }
    }

    private bool _cargando;
    public bool Cargando
    {
        get => _cargando;
        set => SetProperty(ref _cargando, value);
    }

    public ICommand CargarCommand { get; }
    public ICommand NuevoCommand { get; }
    public ICommand EditarCommand { get; }
    public ICommand EliminarCommand { get; }

    public SociosViewModel(ISociosService sociosService)
    {
        _sociosService = sociosService;

        CargarCommand = new RelayCommand(async _ => await CargarAsync());
        NuevoCommand = new RelayCommand(async _ => await NuevoAsync());
        EditarCommand = new RelayCommand(async _ => await EditarAsync(), _ => SocioSeleccionado != null);
        EliminarCommand = new RelayCommand(async _ => await EliminarAsync(), _ => SocioSeleccionado != null);

        _ = CargarAsync();
    }

    private async Task CargarAsync()
    {
        Cargando = true;
        try
        {
            Socios.Clear();
            var lista = await _sociosService.ObtenerTodosAsync();
            foreach (var s in lista) Socios.Add(s);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar socios: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Cargando = false;
        }
    }

    private async Task BuscarAsync()
    {
        try
        {
            Socios.Clear();
            var lista = await _sociosService.BuscarAsync(TextoBusqueda);
            foreach (var s in lista) Socios.Add(s);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task NuevoAsync()
    {
        var ventana = new SocioEditWindow(_sociosService)
        {
            Owner = Application.Current.MainWindow
        };
        ventana.ShowDialog();

        if (ventana.GuardadoExitoso)
            await CargarAsync();
    }

    private async Task EditarAsync()
    {
        if (SocioSeleccionado == null) return;

        var ventana = new SocioEditWindow(_sociosService, SocioSeleccionado)
        {
            Owner = Application.Current.MainWindow
        };
        ventana.ShowDialog();

        if (ventana.GuardadoExitoso)
            await CargarAsync();
    }

    private async Task EliminarAsync()
    {
        if (SocioSeleccionado == null) return;

        var confirm = MessageBox.Show(
            $"¿Eliminar a {SocioSeleccionado.Nombre} {SocioSeleccionado.Apellido}?\n\n" +
            "Esta acción no se puede deshacer.",
            "Confirmar eliminación",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            await _sociosService.EliminarAsync(SocioSeleccionado.Id);
            await CargarAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}