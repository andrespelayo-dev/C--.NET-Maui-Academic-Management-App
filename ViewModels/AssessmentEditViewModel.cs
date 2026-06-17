using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C971project.Models;
using C971project.Services;
using Plugin.LocalNotification;
using System.Text.RegularExpressions;
namespace C971project.ViewModels;

[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssessmentId), "assessmentId")]
public partial class AssessmentEditViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    public AssessmentEditViewModel(DatabaseService db)
    {
        _db = db;
    }

    [ObservableProperty] private int courseId;
    [ObservableProperty] private int assessmentId;

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string type = "Objective";
    [ObservableProperty] private DateTime startDate = DateTime.Today;
    [ObservableProperty] private DateTime endDate = DateTime.Today.AddDays(7);
    [ObservableProperty] private bool notifyStart;
    [ObservableProperty] private bool notifyEnd;
    [ObservableProperty] private int startNotificationId;
    [ObservableProperty] private int endNotificationId;


    public async Task LoadAsync()
    {
        if (AssessmentId == 0)
        {
            Title = "";
            Type = "Objective";
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(7);
            return;
        }

        var a = await _db.GetAssessmentAsync(AssessmentId);
        if (a is null) return;

        Title = a.Title;
        Type = a.Type;
        StartDate = a.StartDate;
        EndDate = a.EndDate;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Missing info", "Assessment title is required.", "OK");
            return;
        }

        if (EndDate < StartDate)
        {
            await Shell.Current.DisplayAlert("Invalid dates", "End date must be after start date.", "OK");
            return;
        }

        // Enforce max one Objective + one Performance per course
        var existing = await _db.GetAssessmentsByCourseAsync(CourseId);
        var normalizedType = (Type ?? "").Trim();

        bool isObj = normalizedType.Equals("Objective", StringComparison.OrdinalIgnoreCase);
        bool isPerf = normalizedType.Equals("Performance", StringComparison.OrdinalIgnoreCase);

        if (!isObj && !isPerf)
        {
            await Shell.Current.DisplayAlert("Invalid type", "Type must be Objective or Performance.", "OK");
            return;
        }

        bool wouldDuplicate =
            existing.Any(a =>
                a.AssessmentId != AssessmentId &&
                a.Type.Equals(normalizedType, StringComparison.OrdinalIgnoreCase));

        if (wouldDuplicate)
        {
            await Shell.Current.DisplayAlert(
                "Limit reached",
                $"Only one {normalizedType} assessment is allowed per course.",
                "OK");
            return;
        }

        var assessment = new Assessment
        {
            AssessmentId = AssessmentId,
            CourseId = CourseId,
            Title = Title.Trim(),
            Type = isObj ? "Objective" : "Performance",
            StartDate = StartDate,
            EndDate = EndDate,

            NotifyStart = NotifyStart,
            NotifyEnd = NotifyEnd,
            StartNotificationId = StartNotificationId,
            EndNotificationId = EndNotificationId
        };

        if (NotifyStart)
        {
            CancelNotification(assessment.StartNotificationId);

            assessment.StartNotificationId = await ScheduleNotificationAsync(
                "Assessment starts",
                $"{assessment.Title} starts today.",
                assessment.StartDate
            );
        }
        else
        {
            CancelNotification(assessment.StartNotificationId);
            assessment.StartNotificationId = 0;
        }

        if (NotifyEnd)
        {
            CancelNotification(assessment.EndNotificationId);

            assessment.EndNotificationId = await ScheduleNotificationAsync(
                "Assessment ends",
                $"{assessment.Title} ends today.",
                assessment.EndDate
            );
        }
        else
        {
            CancelNotification(assessment.EndNotificationId);
            assessment.EndNotificationId = 0;
        }

        await _db.SaveAssessmentAsync(assessment);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (AssessmentId == 0)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        bool ok = await Shell.Current.DisplayAlert("Confirm", "Delete this assessment?", "Yes", "No");
        if (!ok) return;

        var existing = await _db.GetAssessmentAsync(AssessmentId);
        if (existing != null)
        {
            CancelNotification(existing.StartNotificationId);
            CancelNotification(existing.EndNotificationId);

            await _db.DeleteAssessmentAsync(existing);
        }
        await Shell.Current.GoToAsync("..");

    }

    private static DateTime NormalizeTo6Am(DateTime date)
    {
        return new DateTime(date.Year, date.Month, date.Day, 6, 0, 0);
    }

    private async Task<int> ScheduleNotificationAsync(string title, string body, DateTime date)
    {
        var request = new NotificationRequest
        {
            NotificationId = new Random().Next(100000, 999999),
            Title = title,
            Description = body,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = NormalizeTo6Am(date)
            }
        };

        await LocalNotificationCenter.Current.Show(request);
        return request.NotificationId;
    }

    private void CancelNotification(int id)
    {
        if (id > 0)
            LocalNotificationCenter.Current.Cancel(id);
    }

}
