using C971project.ViewModels;

namespace C971project.Views;

public partial class TermEditPage : ContentPage
{
    public TermEditPage(TermEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
