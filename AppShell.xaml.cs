using C971project.Views;

namespace C971project;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TermEditPage), typeof(TermEditPage));
        Routing.RegisterRoute(nameof(TermDetailPage), typeof(TermDetailPage));
        Routing.RegisterRoute(nameof(CourseEditPage), typeof(CourseEditPage));
        Routing.RegisterRoute(nameof(AssessmentListPage), typeof(AssessmentListPage));
        Routing.RegisterRoute(nameof(AssessmentEditPage), typeof(AssessmentEditPage));
        Routing.RegisterRoute("assessmentList", typeof(AssessmentListPage));

    }
}
