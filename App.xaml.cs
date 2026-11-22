using IbgeNomes.Views;

namespace IbgeNomes;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = AppTheme.Light; // paleta pensada para tema claro
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // A página é criada aqui, quando os recursos do App.xaml já foram carregados.
        var page = IPlatformApplication.Current!.Services.GetRequiredService<MainPage>();
        return new Window(new NavigationPage(page));
    }
}
