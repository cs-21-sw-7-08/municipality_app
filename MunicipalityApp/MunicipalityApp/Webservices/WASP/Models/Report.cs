using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public class Report
    {
        public int Id { get; set; }
        public int IssueId { get; set; }
        public int ReportCategoryId { get; set; }
        public int TypeCounter { get; set; }
        public ReportCategory ReportCategory { get; set; }

    }
}
