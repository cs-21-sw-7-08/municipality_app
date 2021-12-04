using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public class MunicipalityUserSignUp
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int MunicipalityId { get; set; }
    }
}
