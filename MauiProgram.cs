using IbgeNomes.Services;
using IbgeNomes.ViewModels;
using IbgeNomes.Views;

namespace IbgeNomes;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton(new HttpClient { Timeout = TimeSpan.FromSeconds(30) });
        builder.Services.AddSingleton<IbgeLocalidadesService>();
        builder.Services.AddSingleton<IbgeNomesService>();
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
