using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MunicipalityApp.Webservices.WASP
{
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
    }
}
