using GalaSoft.MvvmLight.CommandWpf;
using MunicipalityApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public partial class MunicipalityResponse : BindableBase, ICloneable
    {        
        private SimpleCommand<MunicipalityResponse> deleteResponseCommand;
        private bool isInEditMode;

        public bool IsInEditMode { get => isInEditMode; set => SetValue(ref isInEditMode, value); }
        public string Date => DateEdited == null ? DateCreated.ToString("yyyy-MM-dd HH:mm") : DateEdited.Value.ToString("yyyy-MM-dd HH:mm");        
        public SimpleCommand<MunicipalityResponse> DeleteResponseCommand { get => deleteResponseCommand; set => SetValue(ref deleteResponseCommand, value); }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
