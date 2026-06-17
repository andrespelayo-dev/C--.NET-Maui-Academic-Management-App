using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C971project.Models;
using C971project.Services;

namespace C971project.ViewModels;

public partial class TermEditViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _db;

    [ObservableProperty] private int termId;
    [ObservableProperty] private string title = "";
    [ObservableProperty] private DateTime startDate = DateTime.Today;
    [ObservableProperty] private DateTime endDate = DateTime.Today.AddMonths(6);

    public TermEditViewModel(DatabaseService db) => _db = db;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Term", out var obj) && obj is Term t)
        {
            TermId = t.TermId;
            Title = t.Title;
            StartDate = t.StartDate;
            EndDate = t.EndDate;
        }
        else
        {
            TermId = 0;
            Title = "";
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddMonths(6);
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Missing info", "Term title is required.", "OK");
            return;
        }

        if (EndDate < StartDate)
        {
            await Shell.Current.DisplayAlert("Invalid dates", "End date must be after start date.", "OK");
            return;
        }

        var term = new Term
        {
            TermId = TermId,
            Title = Title.Trim(),
            StartDate = StartDate,
            EndDate = EndDate
        };

        await _db.SaveTermAsync(term);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task DeleteAsync()
    {
        if (TermId == 0)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        bool ok = await Shell.Current.DisplayAlert("Delete term?", "This will delete its courses and assessments.", "Delete", "Cancel");
        if (!ok) return;

        var existing = await _db.GetTermAsync(TermId);
        if (existing != null) await _db.DeleteTermAsync(existing);

        await Shell.Current.GoToAsync("..");
    }
}
