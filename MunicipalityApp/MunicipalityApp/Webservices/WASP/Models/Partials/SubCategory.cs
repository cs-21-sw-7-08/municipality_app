using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class SubCategory : BindableBase
    {
        private bool isSelected;

        public bool IsSelected { get => isSelected; set => SetValue(ref isSelected, value); }
    }
}
