using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using GymApp.Frontend.ViewModels;
using GymApp.Frontend.Views;
using Microsoft.Extensions.DependencyInjection;

namespace GymApp.Frontend;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BtnSocios_Click(object sender, RoutedEventArgs e)
    {
        var vm = App.Services.GetRequiredService<SociosViewModel>();
        var view = new SociosView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void BtnMembresias_Click(object sender, RoutedEventArgs e)
    {
        var vm = App.Services.GetRequiredService<MembresiasViewModel>();
        var view = new MembresiasView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void BtnPagos_Click(object sender, RoutedEventArgs e)
    {
        var vm = App.Services.GetRequiredService<PagosViewModel>();
        var view = new PagosView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }

    private void BtnReportes_Click(object sender, RoutedEventArgs e)
    {
        var vm = App.Services.GetRequiredService<ReportesViewModel>();
        var view = new ReportesView { DataContext = vm };
        ContenidoPrincipal.Content = view;
    }
}