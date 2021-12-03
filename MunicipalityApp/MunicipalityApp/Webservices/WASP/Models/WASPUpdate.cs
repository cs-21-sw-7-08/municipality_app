using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public class WASPUpdate
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public WASPUpdate()
        {

        }

        public WASPUpdate(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
