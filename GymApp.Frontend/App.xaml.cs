using GymApp.Backend.Data;
using GymApp.Backend.Interfaces;
using GymApp.Backend.Services;
using GymApp.Frontend.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

namespace GymApp.Frontend;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // ===== Base de datos SQLite =====
        var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gym.db");
        services.AddDbContext<GymDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // ===== Servicios del backend =====
        services.AddScoped<ISociosService, SociosService>();
        services.AddScoped<IMembresiasService, MembresiasService>();
        services.AddScoped<IPagosService, PagosService>();

        // ===== ViewModels =====
        services.AddTransient<SociosViewModel>();
        services.AddTransient<MembresiasViewModel>();
        services.AddTransient<PagosViewModel>();
        services.AddTransient<ReportesViewModel>();

        // ===== Ventana principal =====
        services.AddSingleton<MainWindow>();

        Services = services.BuildServiceProvider();

        // ===== Crear la base de datos si no existe =====
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            db.Database.EnsureCreated();
        }

        // ===== Mostrar MainWindow =====
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}