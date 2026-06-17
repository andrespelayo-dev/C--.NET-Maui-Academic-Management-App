using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C971project.Models;
using C971project.Services;
using System.Collections.ObjectModel;

namespace C971project.ViewModels;

public partial class TermListViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public ObservableCollection<Term> Terms { get; } = new();

    [ObservableProperty] private bool isBusy;

    public TermListViewModel(DatabaseService db) => _db = db;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Terms.Clear();

            var items = await _db.GetTermsAsync();
            foreach (var t in items) Terms.Add(t);
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    public async Task AddTermAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.TermEditPage));
    }

    [RelayCommand]
    public async Task OpenTermAsync(Term term)
    {
        if (term is null) return;

        await Shell.Current.GoToAsync(nameof(Views.TermDetailPage),
            new Dictionary<string, object> { ["Term"] = term });
    }
}
