using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971project.Models
{
    public class PerformanceAssessment : AssessmentBase
    {
        public PerformanceAssessment(string title, DateTime startDate, DateTime endDate)
            : base(title, startDate, endDate)
        {
        }

        public override string AssessmentType => "Performance";

        public override string GetSummary()
        {
            return $"[Performance Task] {Title} scheduled from {StartDate:d} to {EndDate:d}";
        }
    }
}