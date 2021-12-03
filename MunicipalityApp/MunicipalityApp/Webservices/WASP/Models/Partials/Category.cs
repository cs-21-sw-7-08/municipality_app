using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class Category : BindableBase
    {
        private bool isSelected;
        private RelayCommand<Category> checkedChangedCommand;

        public bool IsSelected
        {
            get => isSelected; set
            {
                if (SetValue(ref isSelected, value))
                    CheckedChangedCommand?.Execute(this);
            }
        }
        public RelayCommand<Category> CheckedChangedCommand
        {
            get => checkedChangedCommand;
            set => SetValue(ref checkedChangedCommand, value);
        }

    }
}
