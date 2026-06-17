using C971project.Services;

namespace C971project.Views;

public partial class SearchPage : ContentPage
{
    private readonly DatabaseService _db;

    public SearchPage(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        var query = SearchEntry.Text ?? string.Empty;

        var results = await _db.SearchAllAsync(query);

        ResultsCollection.ItemsSource = results;
        ResultsLabel.IsVisible = true;
        NoResultsLabel.IsVisible = results.Count == 0;
    }
}