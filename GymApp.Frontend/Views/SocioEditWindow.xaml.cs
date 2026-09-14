using GymApp.Backend.Entity;
using GymApp.Backend.Interfaces;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GymApp.Frontend.Views;

public partial class SocioEditWindow : Window
{
    private readonly ISociosService _sociosService;
    private readonly Socio? _socioExistente;

    public bool GuardadoExitoso { get; private set; }

    // Constructor para NUEVO socio
    public SocioEditWindow(ISociosService sociosService)
    {
        InitializeComponent();
        _sociosService = sociosService;
        _socioExistente = null;
        TituloVentana.Text = "➕ Nuevo Socio";
        Title = "Nuevo Socio";
        DpFechaNacimiento.SelectedDate = DateTime.Today.AddYears(-25);
    }

    // Constructor para EDITAR socio existente
    public SocioEditWindow(ISociosService sociosService, Socio socio) : this(sociosService)
    {
        _socioExistente = socio;
        TituloVentana.Text = "✏️ Editar Socio";
        Title = "Editar Socio";

        // Cargar datos en el formulario
        TxtNombre.Text = socio.Nombre;
        TxtApellido.Text = socio.Apellido;
        TxtCedula.Text = socio.Cedula;
        TxtTelefono.Text = socio.Telefono ?? "";
        TxtEmail.Text = socio.Email ?? "";
        DpFechaNacimiento.SelectedDate = socio.FechaNacimiento;
        ChkActivo.IsChecked = socio.Activo;
    }

    private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        // ===== Validaciones =====
        if (string.IsNullOrWhiteSpace(TxtNombre.Text))
        {
            MessageBox.Show("El nombre es obligatorio.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtNombre.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(TxtApellido.Text))
        {
            MessageBox.Show("El apellido es obligatorio.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtApellido.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(TxtCedula.Text))
        {
            MessageBox.Show("La cédula es obligatoria.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtCedula.Focus();
            return;
        }

        if (DpFechaNacimiento.SelectedDate == null)
        {
            MessageBox.Show("La fecha de nacimiento es obligatoria.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            DpFechaNacimiento.Focus();
            return;
        }

        if (DpFechaNacimiento.SelectedDate > DateTime.Today)
        {
            MessageBox.Show("La fecha de nacimiento no puede ser futura.", "Validación",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (_socioExistente == null)
            {
                // ===== CREAR =====
                var nuevo = new Socio
                {
                    Nombre = TxtNombre.Text.Trim(),
                    Apellido = TxtApellido.Text.Trim(),
                    Cedula = TxtCedula.Text.Trim(),
                    Telefono = string.IsNullOrWhiteSpace(TxtTelefono.Text) ? null : TxtTelefono.Text.Trim(),
                    Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim(),
                    FechaNacimiento = DpFechaNacimiento.SelectedDate.Value,
                    Activo = ChkActivo.IsChecked ?? true,
                    FechaRegistro = DateTime.Now
                };

                await _sociosService.CrearAsync(nuevo);
            }
            else
            {
                // ===== EDITAR =====
                _socioExistente.Nombre = TxtNombre.Text.Trim();
                _socioExistente.Apellido = TxtApellido.Text.Trim();
                _socioExistente.Cedula = TxtCedula.Text.Trim();
                _socioExistente.Telefono = string.IsNullOrWhiteSpace(TxtTelefono.Text) ? null : TxtTelefono.Text.Trim();
                _socioExistente.Email = string.IsNullOrWhiteSpace(TxtEmail.Text) ? null : TxtEmail.Text.Trim();
                _socioExistente.FechaNacimiento = DpFechaNacimiento.SelectedDate.Value;
                _socioExistente.Activo = ChkActivo.IsChecked ?? true;

                await _sociosService.ActualizarAsync(_socioExistente);
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
