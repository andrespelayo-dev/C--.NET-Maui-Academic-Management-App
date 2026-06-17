using C971project.ViewModels;

namespace C971project.Views;

public partial class CourseEditPage : ContentPage
{
    public CourseEditPage(CourseEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
