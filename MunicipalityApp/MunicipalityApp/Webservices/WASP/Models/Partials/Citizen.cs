using MunicipalityApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class Citizen : BindableBase
    {
        private SimpleCommand<Citizen> unblockCitizenCommand;

        public SimpleCommand<Citizen> UnblockCitizenCommand { get => unblockCitizenCommand; set => SetValue(ref unblockCitizenCommand, value); }
    }
}
