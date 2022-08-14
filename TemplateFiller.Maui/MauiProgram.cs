using TemplateFiller.Core;
using TemplateFiller.Maui.Services;
using TemplateFiller.Maui.ViewModels;
using TemplateFiller.Maui.Views;

namespace TemplateFiller.Maui;

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

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<MainPage>();

        builder.Services.AddSingleton<MainViewModel>();

        builder.Services.AddSingleton<IEmailSender, EmailSender>();
        builder.Services.AddSingleton<IFileProcessor, LocalFileProcessor>();

        return builder.Build();
    }
}
