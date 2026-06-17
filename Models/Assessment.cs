using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971project.Models;

public class Assessment
{
    [PrimaryKey, AutoIncrement]
    public int AssessmentId { get; set; }

    [Indexed]
    public int CourseId { get; set; }

    [NotNull]
    public string Title { get; set; } = string.Empty;

    [NotNull]
    public string Type { get; set; } = "Objective";

    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(7);

    public bool NotifyStart { get; set; }
    public bool NotifyEnd { get; set; }

    public int StartNotificationId { get; set; }
    public int EndNotificationId { get; set; }
}
