using C971project.ViewModels;
using C971project.Views;

namespace C971project.Views;

public partial class TermListPage : ContentPage
{
    private readonly TermListViewModel _vm;

    public TermListPage(TermListViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as TermListViewModel)!.LoadAsync();
    }
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        if (Handler?.MauiContext?.Services is null)
            return;

        var searchPage = Handler.MauiContext.Services.GetService<SearchPage>();
        if (searchPage is not null)
            await Navigation.PushAsync(searchPage);
    }
    private async void OnReportsClicked(object sender, EventArgs e)
    {
        if (Handler?.MauiContext?.Services is null)
            return;

        var reportsPage = Handler.MauiContext.Services.GetService<ReportsPage>();
        if (reportsPage is not null)
            await Navigation.PushAsync(reportsPage);
    }
}
