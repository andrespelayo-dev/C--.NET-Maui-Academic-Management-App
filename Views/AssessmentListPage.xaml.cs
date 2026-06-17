using C971project.ViewModels;

namespace C971project.Views;

public partial class AssessmentListPage : ContentPage
{
    public AssessmentListPage(AssessmentListViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AssessmentListViewModel vm)
            await vm.LoadAsync();
    }

}
