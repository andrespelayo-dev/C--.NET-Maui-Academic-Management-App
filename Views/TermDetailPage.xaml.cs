using C971project.ViewModels;

namespace C971project.Views;

public partial class TermDetailPage : ContentPage
{
    private readonly TermDetailViewModel _vm;

    public TermDetailPage(TermDetailViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCoursesAsync();
    }
}
