using C971project.ViewModels;

namespace C971project.Views;

public partial class AssessmentEditPage : ContentPage
{
    public AssessmentEditPage(AssessmentEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AssessmentEditViewModel vm)
            await vm.LoadAsync();
    }
}
