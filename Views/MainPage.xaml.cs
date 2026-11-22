using IbgeNomes.ViewModels;

namespace IbgeNomes.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _vm;

    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_vm.Estados.Count == 0) await _vm.InicializarAsync();
    }

    private void Aba_Clicked(object? sender, EventArgs e)
    {
        if (sender is Button b && int.TryParse(b.ClassId, out var i)) _vm.AbaIndex = i;
    }
}
