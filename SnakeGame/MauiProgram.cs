using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SnakeGame.DataAccess;
using SnakeGame.ViewModels;

namespace SnakeGame;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        string? connectionString = Environment.GetEnvironmentVariable("STORAGE_CS");
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        }, ServiceLifetime.Scoped);

        builder.Services.AddScoped<IUserRepo, UserRepo>();

        // Регистриране на ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        // Регистриране на Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug().SetMinimumLevel(LogLevel.Debug);
#endif

        return builder.Build();
    }
}
