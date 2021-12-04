using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MunicipalityApp.Webservices.WASP
{
    public enum IssueStateValue
    {
        Created,
        Approved,
        Resolved,
        NotResolved
    }

    public partial class IssueState : BindableBase
    {
        private bool isSelected;

        public bool IsSelected { get => isSelected; set => SetValue(ref isSelected, value); }
        public Brush StateColor
        {
            get
            {
                switch (Id)
                {
                    case 1:
                        return App.BrushIssueStateCreated;
                    case 2:
                        return App.BrushIssueStateApproved;
                    case 3:
                        return App.BrushIssueStateResolved;
                    case 4:
                        return App.BrushIssueStateNotResolved;
                }
                return Brushes.White;
            }
        }
        public IssueStateValue IssueStateValue
        {
            get
            {
                switch (Id)
                {
                    case 1:
                        return IssueStateValue.Created;
                    case 2:
                        return IssueStateValue.Approved;
                    case 3:
                        return IssueStateValue.Resolved;
                    case 4:
                        return IssueStateValue.NotResolved;
                }
                return IssueStateValue.Created;
            }
            set
            {
                switch (value)
                {
                    case IssueStateValue.Created:
                        Id = 1;
                        break;
                    case IssueStateValue.Approved:
                        Id = 2;
                        break;
                    case IssueStateValue.Resolved:
                        Id = 3;
                        break;
                    case IssueStateValue.NotResolved:
                        Id = 4;
                        break;
                }
            }
        }
    }
}
