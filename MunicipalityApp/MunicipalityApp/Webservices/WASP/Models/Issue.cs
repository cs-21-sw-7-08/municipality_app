using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class Issue
    {
        public int Id { get; set; }
        public int CitizenId { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
        public Location Location { get; set; }
        public string Address { get; set; }
        public string Picture1 { get; set; }
        public string Picture2 { get; set; }
        public string Picture3 { get; set; }
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public Municipality Municipality { get; set; }
        public IssueState IssueState { get; set; }
        public List<MunicipalityResponse> MunicipalityResponses { get; set; }
        public List<int> IssueVerificationCitizenIds { get; set; }
        public List<Report> Reports { get; set; }
        public Citizen Citizen { get; set; }
    }
}
