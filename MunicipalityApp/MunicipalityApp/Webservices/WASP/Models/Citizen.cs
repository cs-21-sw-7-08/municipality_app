using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class Citizen
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Name { get; set; }
        public bool IsBlocked { get; set; }
        public Municipality Municipality { get; set; }
        public int MunicipalityId { get; set; }
    }
}
