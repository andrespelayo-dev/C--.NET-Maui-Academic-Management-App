using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971project.Models
{
    public class CourseReportItem
    {
        public string CourseTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StartDateText { get; set; } = string.Empty;
        public string EndDateText { get; set; } = string.Empty;
        public string AssessmentCountText { get; set; } = string.Empty;
    }
}