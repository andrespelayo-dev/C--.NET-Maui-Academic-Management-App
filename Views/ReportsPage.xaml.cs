using C971project.Services;

namespace C971project.Views;

public partial class ReportsPage : ContentPage
{
    private readonly DatabaseService _db;

    public ReportsPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        TimestampLabel.Text = $"Generated: {DateTime.Now:G}";
        ReportCollection.ItemsSource = await _db.GetCourseProgressReportAsync();
    }
}