using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971project.Models
{
    public class ObjectiveAssessment : AssessmentBase
    {
        public ObjectiveAssessment(string title, DateTime startDate, DateTime endDate)
            : base(title, startDate, endDate)
        {
        }

        public override string AssessmentType => "Objective";

        public override string GetSummary()
        {
            return $"[Objective Exam] {Title} scheduled from {StartDate:d} to {EndDate:d}";
        }
    }
}
