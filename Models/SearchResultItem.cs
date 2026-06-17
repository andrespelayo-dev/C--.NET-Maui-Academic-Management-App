using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971project.Models
{
    public class SearchResultItem
    {
        public string ResultType { get; set; } = string.Empty;   
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string DetailLine1 { get; set; } = string.Empty;
        public string DetailLine2 { get; set; } = string.Empty;
    }
}
