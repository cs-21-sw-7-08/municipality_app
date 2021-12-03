using MunicipalityApp.Webservices.WASP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MunicipalityApp
{
    public class IssueDetailsViewModel : BindableBase
    {
        #region Variables
        
        private List<BitmapImage> pictures;
        private IssueState issueState;
        private string dateCreated;
        private string dateEdited;
        private string category;
        private string subCategory;
        private string description;
        private ObservableCollection<MunicipalityResponse> municipalityResponses;

        #endregion

        #region Constructor

        public IssueDetailsViewModel()
        {

        }

        #endregion

        #region Methods

        public void Init(Issue issue)
        {
            Issue = issue;
        }

        private void LoadValues()
        {

        }

        #endregion

        #region Properties

        public Issue Issue { get; set; }
        public List<BitmapImage> Pictures { get => pictures; set => SetValue(ref pictures, value); }
        public IssueState IssueState { get => issueState; set => SetValue(ref issueState, value); }
        public string DateCreated { get => dateCreated; set => SetValue(ref dateCreated, value); }
        public string DateEdited { get => dateEdited; set => SetValue(ref dateEdited, value); }
        public string Category { get => category; set => SetValue(ref category, value); }
        public string SubCategory { get => subCategory; set => SetValue(ref subCategory, value); }
        public string Description { get => description; set => SetValue(ref description, value); }
        public ObservableCollection<MunicipalityResponse> MunicipalityResponses { get => municipalityResponses; set => SetValue(ref municipalityResponses, value); }

        #endregion
    }
}
