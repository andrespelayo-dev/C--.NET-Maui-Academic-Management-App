using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using C971project.Models;
using C971project.Services;
using System.Text.RegularExpressions;
using Plugin.LocalNotification;
using Microsoft.Maui.ApplicationModel.DataTransfer;



namespace C971project.ViewModels;

public partial class CourseEditViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService _db;

    [ObservableProperty] private int courseId;
    [ObservableProperty] private int termId;

    [ObservableProperty] private string title = "";
    [ObservableProperty] private DateTime startDate = DateTime.Today;
    [ObservableProperty] private DateTime endDate = DateTime.Today.AddMonths(1);

    [ObservableProperty] private string status = "In Progress";

    [ObservableProperty] private string instructorName = "";
    [ObservableProperty] private string instructorPhone = "";
    [ObservableProperty] private string instructorEmail = "";

    [ObservableProperty] private string notes = "";

    [ObservableProperty] private bool notifyStart;
    [ObservableProperty] private bool notifyEnd;

    public CourseEditViewModel(DatabaseService db) => _db = db;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Course", out var obj) && obj is Course c)
        {
            CourseId = c.CourseId;
            TermId = c.TermId;

            Title = c.Title;
            StartDate = c.StartDate;
            EndDate = c.EndDate;

            Status = string.IsNullOrWhiteSpace(c.Status) ? "In Progress" : c.Status;

            InstructorName = c.InstructorName ?? "";
            InstructorPhone = c.InstructorPhone ?? "";
            InstructorEmail = c.InstructorEmail ?? "";
            Notes = c.Notes ?? "";

            NotifyStart = c.NotifyStart;
            NotifyEnd = c.NotifyEnd;
        }
        else
        {
            CourseId = 0;
            if (query.TryGetValue("TermId", out var tid) && tid is int i) TermId = i;

            Title = "";
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddMonths(1);

            Status = "In Progress";

            InstructorName = "";
            InstructorPhone = "";
            InstructorEmail = "";
            Notes = "";

            NotifyStart = false;
            NotifyEnd = false;
        }
    }



    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            await Shell.Current.DisplayAlert("Missing info", "Course title is required.", "OK");
            return;
        }

        if (EndDate < StartDate)
        {
            await Shell.Current.DisplayAlert("Invalid dates", "End date must be after start date.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(InstructorName) ||
            string.IsNullOrWhiteSpace(InstructorPhone) ||
            string.IsNullOrWhiteSpace(InstructorEmail))
        {
            await Shell.Current.DisplayAlert("Missing info", "Instructor name, phone, and email are required.", "OK");
            return;
        }

        if (!Regex.IsMatch(InstructorEmail.Trim(), @"^\S+@\S+\.\S+$"))
        {
            await Shell.Current.DisplayAlert("Invalid email", "Enter a valid email address.", "OK");
            return;
        }

        var course = new Course
        {
            CourseId = CourseId,
            TermId = TermId,

            Title = Title.Trim(),
            StartDate = StartDate,
            EndDate = EndDate,

            Status = Status,

            InstructorName = InstructorName.Trim(),
            InstructorPhone = InstructorPhone.Trim(),
            InstructorEmail = InstructorEmail.Trim(),

            Notes = Notes ?? "",

            NotifyStart = NotifyStart,
            NotifyEnd = NotifyEnd
        };
        if (NotifyStart)
        {
            CancelNotification(course.StartNotificationId);

            course.StartNotificationId = await ScheduleNotificationAsync(
                "Course starts",
                $"{course.Title} starts today.",
                course.StartDate
            );
        }
        else
        {
            CancelNotification(course.StartNotificationId);
            course.StartNotificationId = 0;
        }

        if (NotifyEnd)
        {
            CancelNotification(course.EndNotificationId);

            course.EndNotificationId = await ScheduleNotificationAsync(
                "Course ends",
                $"{course.Title} ends today.",
                course.EndDate
            );
        }
        else
        {
            CancelNotification(course.EndNotificationId);
            course.EndNotificationId = 0;
        }
        await _db.SaveCourseAsync(course);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (CourseId == 0)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        bool ok = await Shell.Current.DisplayAlert("Delete course?", "This will delete its assessments.", "Delete", "Cancel");
        if (!ok) return;

        var existing = await _db.GetCourseAsync(CourseId);
        if (existing != null) await _db.DeleteCourseAsync(existing);

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
                NotifyTime =  NormalizeTo6Am(date)

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

    [RelayCommand]
    private async Task OpenAssessmentsAsync()
    {
        if (CourseId == 0)
        {
            await Shell.Current.DisplayAlert("Save first", "Please save the course before adding assessments.", "OK");
            return;
        }

        await Shell.Current.GoToAsync($"assessmentList?courseId={CourseId}");
    }

    [RelayCommand]
    private async Task ShareNotesAsync()
    {
        if (string.IsNullOrWhiteSpace(Notes))
        {
            await Shell.Current.DisplayAlert(
                "Nothing to share",
                "Please add notes before sharing.",
                "OK");
            return;
        }

        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Share Course Notes",
            Text = Notes
        });
    }

}
