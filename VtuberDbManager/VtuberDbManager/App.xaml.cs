using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.Xml;
using System.Windows;
using VtuberDbManager.Config;
using VtuberDbManager.Service;
using VtuberDbManager.View;
using VtuberDbManager.ViewModel;

namespace VtuberDbManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;
    protected override void OnStartup(StartupEventArgs e)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        services.Configure<TwitchApiSettings>(configuration.GetSection("TwitchApi"));

        var dbAdminApiKey = configuration["DBAdminApiKey"];
        services.AddHttpClient<IVtuberDbService, VtuberDbService>(v => 
        {
            //v.BaseAddress = new Uri("http://localhost:5240/");
            v.BaseAddress = new Uri("https://vindies.jp/");
            // バックエンドで認識させるための api キーをヘッダーに付加しておく
            v.DefaultRequestHeaders.Add("X-DB-Api-Key", dbAdminApiKey);
        });
        services.AddHttpClient<ITwitchIdService, TwitchIdService>();

        services.AddTransient<MainWindow>();
        services.AddTransient<MainiWindowViewModel>();
        services.AddTransient<VtubersWindow>();
        services.AddTransient<VtubersWindowViewModel>();
        services.AddTransient<GroupsWindow>();
        services.AddTransient<GroupsWindowViewModel>();
        services.AddTransient<PlatformWindow>();
        services.AddTransient<PlatformWindowViewModel>();
        services.AddTransient<VisitorCountWindow>();
        services.AddTransient<VisitorCountWindowViewModel>();

        services.AddSingleton<IWindowService, WindowService>();
        services.AddSingleton<IMessageBoxService, MessageBoxService>();

        ServiceProvider = services.BuildServiceProvider();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        mainWindow.Show();
    }
}
