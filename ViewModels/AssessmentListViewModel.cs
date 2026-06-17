using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C971project.Models;
using C971project.Services;
using C971project.Views;

namespace C971project.ViewModels;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class AssessmentListViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public AssessmentListViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty] private int courseId;

    public ObservableCollection<Assessment> Assessments { get; } = new();

    public async Task LoadAsync()
    {
        Assessments.Clear();
        var list = await _db.GetAssessmentsByCourseAsync(CourseId);
        foreach (var a in list)
            Assessments.Add(a);
    }

    [RelayCommand]
    private async Task AddAssessmentAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(AssessmentEditPage)}?courseId={CourseId}&assessmentId=0");
    }

    [RelayCommand]
    private async Task OpenAssessmentAsync(Assessment assessment)
    {
        if (assessment is null) return;
        await Shell.Current.GoToAsync($"{nameof(AssessmentEditPage)}?courseId={CourseId}&assessmentId={assessment.AssessmentId}");
    }

    public override string ToString() => base.ToString();
}
