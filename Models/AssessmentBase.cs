using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971project.Models
{
    public abstract class AssessmentBase
    {
        public string Title { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }

        protected AssessmentBase(string title, DateTime startDate, DateTime endDate)
        {
            Title = title;
            StartDate = startDate;
            EndDate = endDate;
        }

        public abstract string AssessmentType { get; }

        public virtual string GetSummary()
        {
            return $"{AssessmentType}: {Title} | {StartDate:d} - {EndDate:d}";
        }

        public bool IsDateRangeValid()
        {
            return StartDate <= EndDate;
        }
    }
}