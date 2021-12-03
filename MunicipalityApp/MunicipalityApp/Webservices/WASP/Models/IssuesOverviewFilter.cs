using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public class IssuesOverviewFilter
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public List<int> MunicipalityIds { get; set; }
        public List<int> IssueStateIds { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> SubCategoryIds { get; set; }
        public List<int> CitizenIds { get; set; }
        public bool IsBlocked { get; set; }
    }
}
