using GymApp.Backend.Entity;
using GymApp.Backend.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GymApp.Frontend.Views;

public partial class PagoEditWindow : Window
{
    private readonly IPagosService _pagosService;

    public bool GuardadoExitoso { get; private set; }

    public PagoEditWindow(IPagosService pagosService, List<Socio> socios)
    {
        InitializeComponent();
        _pagosService = pagosService;

        CmbSocio.ItemsSource = socios.Select(s => new
        {
            s.Id,
            NombreCompleto = $"{s.Nombre} {s.Apellido} ({s.Cedula})"
        }).ToList();

        if (CmbSocio.Items.Count > 0)
            CmbSocio.SelectedIndex = 0;
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

        if (!decimal.TryParse(TxtMonto.Text, out var monto) || monto <= 0)
        {
            MessageBox.Show("El monto debe ser un número mayor a cero.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtMonto.Focus();
            return;
        }

        if (CmbMetodo.SelectedItem == null)
        {
            MessageBox.Show("Debes seleccionar un método de pago.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            dynamic socioSeleccionado = CmbSocio.SelectedItem;
            var metodo = ((ComboBoxItem)CmbMetodo.SelectedItem).Content.ToString() ?? "Efectivo";

            var nuevo = new Pago
            {
                SocioId = socioSeleccionado.Id,
                Monto = monto,
                FechaPago = DateTime.Now,
                MetodoPago = metodo,
                Observaciones = string.IsNullOrWhiteSpace(TxtObservaciones.Text)
                    ? null
                    : TxtObservaciones.Text.Trim()
            };

            await _pagosService.CrearAsync(nuevo);

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