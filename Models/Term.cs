using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971project.Models;

public class Term
{
    [PrimaryKey, AutoIncrement]
    public int TermId { get; set; }

    [NotNull]
    public string Title { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(6);
}
