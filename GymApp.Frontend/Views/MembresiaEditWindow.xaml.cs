using GymApp.Backend.Entity;
using GymApp.Backend.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GymApp.Frontend.Views;

public partial class MembresiaEditWindow : Window
{
    private readonly IMembresiasService _membresiasService;
    private readonly Membresia? _membresiaExistente;
    private readonly Dictionary<string, decimal> _precios = new()
    {
        { "Mensual", 30m },
        { "Trimestral", 80m },
        { "Semestral", 150m },
        { "Anual", 280m }
    };

    public bool GuardadoExitoso { get; private set; }

    // Constructor para NUEVA membresía
    public MembresiaEditWindow(IMembresiasService membresiasService, List<Socio> socios)
    {
        InitializeComponent();
        _membresiasService = membresiasService;
        _membresiaExistente = null;

        TituloVentana.Text = "➕ Nueva Membresía";
        Title = "Nueva Membresía";

        CmbSocio.ItemsSource = socios.Select(s => new
        {
            s.Id,
            NombreCompleto = $"{s.Nombre} {s.Apellido} ({s.Cedula})"
        }).ToList();

        if (CmbSocio.Items.Count > 0)
            CmbSocio.SelectedIndex = 0;

        CmbTipo.SelectedIndex = 0;
        DpFechaInicio.SelectedDate = DateTime.Today;
        DpFechaFin.SelectedDate = DateTime.Today.AddMonths(1);
    }

    // Constructor para EDITAR membresía existente
    public MembresiaEditWindow(IMembresiasService membresiasService, List<Socio> socios, Membresia membresia)
        : this(membresiasService, socios)
    {
        _membresiaExistente = membresia;
        TituloVentana.Text = "✏️ Editar Membresía";
        Title = "Editar Membresía";

        // Seleccionar el socio correspondiente
        for (int i = 0; i < CmbSocio.Items.Count; i++)
        {
            dynamic item = CmbSocio.Items[i];
            if (item.Id == membresia.SocioId)
            {
                CmbSocio.SelectedIndex = i;
                break;
            }
        }

        // Seleccionar tipo
        foreach (ComboBoxItem item in CmbTipo.Items)
        {
            if (item.Content.ToString() == membresia.Tipo)
            {
                CmbTipo.SelectedItem = item;
                break;
            }
        }

        DpFechaInicio.SelectedDate = membresia.FechaInicio;
        DpFechaFin.SelectedDate = membresia.FechaFin;
        TxtPrecio.Text = membresia.Precio.ToString("F2");
        ChkActiva.IsChecked = membresia.Activa;
    }

    private void CmbTipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbTipo.SelectedItem is ComboBoxItem item)
        {
            var tipo = item.Content.ToString();
            if (tipo != null && _precios.TryGetValue(tipo, out var precio))
            {
                TxtPrecio.Text = precio.ToString("F2");

                // Actualizar fecha fin sugerida
                if (DpFechaInicio.SelectedDate.HasValue)
                {
                    var inicio = DpFechaInicio.SelectedDate.Value;
                    DpFechaFin.SelectedDate = tipo switch
                    {
                        "Mensual" => inicio.AddMonths(1),
                        "Trimestral" => inicio.AddMonths(3),
                        "Semestral" => inicio.AddMonths(6),
                        "Anual" => inicio.AddYears(1),
                        _ => inicio.AddMonths(1)
                    };
                }
            }
        }
    }

    private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        // Validaciones
        if (CmbSocio.SelectedItem == null)
        {
            MessageBox.Show("Debes seleccionar un socio.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CmbTipo.SelectedItem == null)
        {
            MessageBox.Show("Debes seleccionar un tipo.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (DpFechaInicio.SelectedDate == null || DpFechaFin.SelectedDate == null)
        {
            MessageBox.Show("Las fechas son obligatorias.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (DpFechaFin.SelectedDate <= DpFechaInicio.SelectedDate)
        {
            MessageBox.Show("La fecha fin debe ser posterior a la fecha inicio.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(TxtPrecio.Text, out var precio) || precio <= 0)
        {
            MessageBox.Show("El precio debe ser un número mayor a cero.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            dynamic socioSeleccionado = CmbSocio.SelectedItem;
            int socioId = socioSeleccionado.Id;
            var tipo = ((ComboBoxItem)CmbTipo.SelectedItem).Content.ToString() ?? "Mensual";

            if (_membresiaExistente == null)
            {
                // Crear
                var nueva = new Membresia
                {
                    SocioId = socioId,
                    Tipo = tipo,
                    FechaInicio = DpFechaInicio.SelectedDate.Value,
                    FechaFin = DpFechaFin.SelectedDate.Value,
                    Precio = precio,
                    Activa = ChkActiva.IsChecked ?? true
                };
                await _membresiasService.CrearAsync(nueva);
            }
            else
            {
                // Editar
                _membresiaExistente.SocioId = socioId;
                _membresiaExistente.Tipo = tipo;
                _membresiaExistente.FechaInicio = DpFechaInicio.SelectedDate.Value;
                _membresiaExistente.FechaFin = DpFechaFin.SelectedDate.Value;
                _membresiaExistente.Precio = precio;
                _membresiaExistente.Activa = ChkActiva.IsChecked ?? true;
                await _membresiasService.ActualizarAsync(_membresiaExistente);
            }

            GuardadoExitoso = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        GuardadoExitoso = false;
        Close();
    }
}