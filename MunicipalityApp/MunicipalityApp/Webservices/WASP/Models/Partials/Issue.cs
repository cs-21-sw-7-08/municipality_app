using MunicipalityApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class Issue : BindableBase
    {
        private SimpleCommand<Issue> seeIssueDetailsCommand;

        public SimpleCommand<Issue> SeeIssueDetailsCommand { get => seeIssueDetailsCommand; set => SetValue(ref seeIssueDetailsCommand, value); }

    }
}
