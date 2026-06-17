using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using C971project.Models;
using System.Globalization;

namespace C971project.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "termtracker.db3");
        _db = new SQLiteAsyncConnection(dbPath);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _db.CreateTableAsync<Term>();
        await _db.CreateTableAsync<Course>();
        await _db.CreateTableAsync<Assessment>();

        await SeedEvaluationDataAsync();

    }

    //  TERM 
    public Task<List<Term>> GetTermsAsync() =>
        _db.Table<Term>().OrderBy(t => t.StartDate).ToListAsync();

    public Task<Term?> GetTermAsync(int termId) =>
        _db.Table<Term>().Where(t => t.TermId == termId).FirstOrDefaultAsync();

    public Task<int> SaveTermAsync(Term term) =>
        term.TermId == 0 ? _db.InsertAsync(term) : _db.UpdateAsync(term);

    public async Task<int> DeleteTermAsync(Term term)
    {
        var courses = await GetCoursesByTermAsync(term.TermId);
        foreach (var c in courses)
            await DeleteCourseAsync(c);

        return await _db.DeleteAsync(term);
    }

    //  COURSE 
    public Task<List<Course>> GetCoursesByTermAsync(int termId) =>
        _db.Table<Course>().Where(c => c.TermId == termId)
           .OrderBy(c => c.StartDate).ToListAsync();

    public Task<Course?> GetCourseAsync(int courseId) =>
        _db.Table<Course>().Where(c => c.CourseId == courseId).FirstOrDefaultAsync();

    public Task<int> SaveCourseAsync(Course course) =>
        course.CourseId == 0 ? _db.InsertAsync(course) : _db.UpdateAsync(course);

    public async Task<int> DeleteCourseAsync(Course course)
    {
        var assessments = await GetAssessmentsByCourseAsync(course.CourseId);
        foreach (var a in assessments)
            await _db.DeleteAsync(a);

        return await _db.DeleteAsync(course);
    }

    //  ASSESSMENT 
    public Task<List<Assessment>> GetAssessmentsByCourseAsync(int courseId) =>
        _db.Table<Assessment>().Where(a => a.CourseId == courseId)
           .OrderBy(a => a.StartDate).ToListAsync();

    public Task<Assessment?> GetAssessmentAsync(int assessmentId) =>
        _db.Table<Assessment>().Where(a => a.AssessmentId == assessmentId).FirstOrDefaultAsync();

    public Task<int> SaveAssessmentAsync(Assessment assessment) =>
        assessment.AssessmentId == 0 ? _db.InsertAsync(assessment) : _db.UpdateAsync(assessment);

    public Task<int> DeleteAssessmentAsync(Assessment assessment) =>
        _db.DeleteAsync(assessment);

    public async Task SeedEvaluationDataAsync()
    {

        var termCount = await _db.Table<Term>().CountAsync();
        if (termCount > 0)
            return;

        var term = new Term
        {
            Title = "Evaluation Term",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(6)
        };

        await _db.InsertAsync(term);


        var course = new Course
        {
            TermId = term.TermId,
            Title = "Mobile Application Development",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(30),
            Status = "In Progress",

            InstructorName = "Anika Patel",
            InstructorPhone = "555-123-4567",
            InstructorEmail = "anika.patel@strimeuniversity.edu",

            Notes = "course notes.",


            NotifyStart = false,
            NotifyEnd = false,
            StartNotificationId = 0,
            EndNotificationId = 0
        };

        await _db.InsertAsync(course);

        var objective = new Assessment
        {
            CourseId = course.CourseId,
            Title = "Objective Assessment",
            Type = "Objective",
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(10),

            NotifyStart = false,
            NotifyEnd = false,
            StartNotificationId = 0,
            EndNotificationId = 0
        };

        var performance = new Assessment
        {
            CourseId = course.CourseId,
            Title = "Performance Assessment",
            Type = "Performance",
            StartDate = DateTime.Today.AddDays(5),
            EndDate = DateTime.Today.AddDays(20),

            NotifyStart = false,
            NotifyEnd = false,
            StartNotificationId = 0,
            EndNotificationId = 0
        };

        await _db.InsertAsync(objective);
        await _db.InsertAsync(performance);
    }

    public static C971project.Models.AssessmentBase ConvertToAssessmentBase(C971project.Models.Assessment assessment)
    {
        if (assessment.Type.Equals("Objective", StringComparison.OrdinalIgnoreCase))
        {
            return new C971project.Models.ObjectiveAssessment(
                assessment.Title,
                assessment.StartDate,
                assessment.EndDate);
        }

        return new C971project.Models.PerformanceAssessment(
            assessment.Title,
            assessment.StartDate,
            assessment.EndDate);
    }
    public async Task<List<string>> GetAssessmentSummariesAsync(int courseId)
    {
        var assessments = await GetAssessmentsByCourseAsync(courseId);
        var summaries = new List<string>();

        foreach (var assessment in assessments)
        {
            var oopAssessment = ConvertToAssessmentBase(assessment);
            summaries.Add(oopAssessment.GetSummary());
        }

        return summaries;
    }
    public async Task<List<SearchResultItem>> SearchAllAsync(string query)
    {
        var results = new List<SearchResultItem>();

        if (string.IsNullOrWhiteSpace(query))
            return results;

        query = query.Trim();

        var courses = await _db.Table<Course>().ToListAsync();
        var assessments = await _db.Table<Assessment>().ToListAsync();

        var matchingCourses = courses
            .Where(c => !string.IsNullOrWhiteSpace(c.Title) &&
                        c.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var course in matchingCourses)
        {
            var courseAssessments = assessments
                .Where(a => a.CourseId == course.CourseId)
                .ToList();

            results.Add(new SearchResultItem
            {
                ResultType = "Course",
                Title = course.Title,
                Subtitle = $"Status: {course.Status}",
                DetailLine1 = $"Dates: {course.StartDate:d} - {course.EndDate:d}",
                DetailLine2 = $"Assessments: {courseAssessments.Count}"
            });
        }

        var matchingAssessments = assessments
          .Where(a =>
          {
              var relatedCourse = courses.FirstOrDefault(c => c.CourseId == a.CourseId);
              var courseTitle = relatedCourse?.Title ?? string.Empty;

              return
                  (!string.IsNullOrWhiteSpace(a.Title) &&
                   a.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                  ||
                  (!string.IsNullOrWhiteSpace(a.Type) &&
                   a.Type.Contains(query, StringComparison.OrdinalIgnoreCase))
                  ||
                  ("assessment".Contains(query, StringComparison.OrdinalIgnoreCase))
                  ||
                  (!string.IsNullOrWhiteSpace(courseTitle) &&
                   courseTitle.Contains(query, StringComparison.OrdinalIgnoreCase));
          })
          .ToList();

        foreach (var assessment in matchingAssessments)
        {
            var relatedCourse = courses.FirstOrDefault(c => c.CourseId == assessment.CourseId);
            var courseTitle = relatedCourse?.Title ?? "Unknown Course";

            results.Add(new SearchResultItem
            {
                ResultType = "Assessment",
                Title = assessment.Title,
                Subtitle = $"Type: {assessment.Type}",
                DetailLine1 = $"Course: {courseTitle}",
                DetailLine2 = $"Due: {assessment.EndDate:d}"
            });
        }

        return results;
    }
    public async Task<List<CourseReportItem>> GetCourseProgressReportAsync()
    {
        var courses = await _db.Table<Course>().ToListAsync();
        var assessments = await _db.Table<Assessment>().ToListAsync();

        var reportItems = courses
            .Select(course =>
            {
                var assessmentCount = assessments.Count(a => a.CourseId == course.CourseId);

                return new CourseReportItem
                {
                    CourseTitle = course.Title,
                    Status = course.Status,
                    StartDateText = course.StartDate.ToString("MM/dd/yy"),
                    EndDateText = course.EndDate.ToString("MM/dd/yy"),
                    AssessmentCountText = assessmentCount.ToString()
                };
            })
            .OrderBy(c => c.CourseTitle)
            .ToList();

        return reportItems;
    }
}
