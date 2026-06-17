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

public partial class TermDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _db;

    [ObservableProperty] private Term? term;
    public ObservableCollection<Course> Courses { get; } = new();

    public TermDetailViewModel(DatabaseService db) => _db = db;

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Term", out var obj) && obj is Term t)
        {
            Term = t;
        }
    }

    [RelayCommand]
    public async Task EditTermAsync()
    {
        if (Term is null) return;
        await Shell.Current.GoToAsync(nameof(Views.TermEditPage),
            new Dictionary<string, object> { ["Term"] = Term });
    }

   

    [RelayCommand]
    public async Task AddCourseAsync()
    {
        if (Term is null) return;

        if (Courses.Count >= 6)
        {
            await Shell.Current.DisplayAlert("Limit reached", "Each term can have up to 6 courses.", "OK");
            return;
        }

        await Shell.Current.GoToAsync(nameof(Views.CourseEditPage),
            new Dictionary<string, object>
            {
                ["TermId"] = Term.TermId
            });
    }

    [RelayCommand]
    public async Task OpenCourseAsync(Course course)
    {
        if (course is null) return;

        await Shell.Current.GoToAsync(nameof(Views.CourseEditPage),
            new Dictionary<string, object>
            {
                ["Course"] = course
            });
    }

    private bool _isLoadingCourses;

    [RelayCommand]
    public async Task LoadCoursesAsync()
    {
        if (Term is null) return;
        if (_isLoadingCourses) return;

        try
        {
            _isLoadingCourses = true;

            var items = await _db.GetCoursesByTermAsync(Term.TermId);

            Courses.Clear(); 
            foreach (var c in items)
                Courses.Add(c);
        }
        finally
        {
            _isLoadingCourses = false;
        }
    }

}
