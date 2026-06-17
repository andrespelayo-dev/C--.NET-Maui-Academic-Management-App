using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971project.Models;

public class Course
{
    [PrimaryKey, AutoIncrement]
    public int CourseId { get; set; }

    [Indexed]
    public int TermId { get; set; }

    [NotNull]
    public string Title { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

    [NotNull]
    public string Status { get; set; } = "Plan To Take";

    [NotNull]
    public string InstructorName { get; set; } = string.Empty;

    [NotNull]
    public string InstructorPhone { get; set; } = string.Empty;

    [NotNull]
    public string InstructorEmail { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public bool NotifyStart { get; set; }
    public bool NotifyEnd { get; set; }

    public int StartNotificationId { get; set; }
    public int EndNotificationId { get; set; }
}
