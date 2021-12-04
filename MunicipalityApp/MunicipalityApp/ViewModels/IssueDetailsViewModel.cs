using GalaSoft.MvvmLight.CommandWpf;
using MunicipalityApp.Commands;
using MunicipalityApp.Webservices.WASP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MunicipalityApp
{
    public enum IssueDetailsView
    {
        Main,
        Progress
    }

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

        private ICommand saveAllChangesCommand;
        private ICommand addResponseCommand;
        private ICommand blockCitizenCommand;
        private SimpleCommand<MunicipalityResponse> deleteResponseCommand;

        private ObservableCollection<IssueState> newIssueStates;
        private IssueState selectedNewIssueState;

        private IssueDetailsView view;
        private string progressMessage;

        private bool isBlocked;

        #endregion

        #region Constructor

        public IssueDetailsViewModel()
        {
            IssueState = new IssueState()
            {
                Id = 1,
                Name = "Created"
            };
            DateCreated = "2021-12-01 10:00";
            DateEdited = "2021-12-01 10:01";
            Category = "Test";
            SubCategory = "Hest";
            IsBlocked = false;

            MunicipalityResponses = new ObservableCollection<MunicipalityResponse>()
            {
                new MunicipalityResponse()
                {
                    Id = 1,
                    IsInEditMode = false,
                    DateCreated = DateTime.Now,
                    DateEdited = DateTime.Parse("2021-10-10 10:00:01"),
                    Response = "This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long This is a long test, very long"
                }
            };

            Description = "Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test Dette er en test";

            AddResponseCommand = new RelayCommand(() =>
            {
                AddResponse();
            });
            SaveAllChangesCommand = new RelayCommand(() =>
            {
                SaveAllChanges();
            });
            BlockCitizenCommand = new RelayCommand(() =>
            {
                if (IsBlocked)
                {
                    UnblockCitizen();
                }
                else
                {
                    BlockCitizen();
                }
            });
            DeleteResponseCommand = new SimpleCommand<MunicipalityResponse>((response) =>
            {
                MunicipalityResponses.Remove(response);
            });
        }

        #endregion

        #region Methods

        #region General

        public void Init(Issue issue, bool isBlocked)
        {
           
            Issue = issue;
            IsBlocked = isBlocked;
            LoadValues();
        }

        private void LoadValues()
        {
            IssueState = Issue.IssueState;

            var pictures = new List<BitmapImage>();
            if (!string.IsNullOrEmpty(Issue.Picture1))
                pictures.Add(GetBitmapImage(Issue.Picture1));
            if (!string.IsNullOrEmpty(Issue.Picture2))
                pictures.Add(GetBitmapImage(Issue.Picture2));
            if (!string.IsNullOrEmpty(Issue.Picture3))
                pictures.Add(GetBitmapImage(Issue.Picture3));
            Pictures = pictures;

            Description = Issue.Description;
            DateCreated = Issue.DateCreated.ToString("yyyy-MM-dd HH:mm");
            if (Issue.DateEdited != null)
                DateEdited = Issue.DateEdited.Value.ToString("yyyy-MM-dd HH:mm");
            else
                DateEdited = null;

            if (Issue.MunicipalityResponses != null)
            {
                MunicipalityResponses = Issue.MunicipalityResponses.Select(x => (MunicipalityResponse)x.Clone()).OrderByDescending(x => x.DateCreated).ToObservableCollection();
                foreach (var item in MunicipalityResponses)
                {
                    item.DeleteResponseCommand = DeleteResponseCommand;
                }
            }
            else
            {
                MunicipalityResponses = new ObservableCollection<MunicipalityResponse>();
            }

            Category = Issue.Category.Name;
            SubCategory = Issue.SubCategory.Name;

            LoadNewIssueStates();
        }

        private void LoadNewIssueStates()
        {
            var newIssueStates = new List<IssueState>();
            switch (Issue.IssueState.IssueStateValue)
            {
                case IssueStateValue.Created:
                    newIssueStates.AddRange(new List<IssueState> {
                        new IssueState()
                        {
                            IssueStateValue = IssueStateValue.Approved,
                            Name = "Approved"
                        },
                        new IssueState()
                        {
                            IssueStateValue = IssueStateValue.Resolved,
                            Name = "Resolved"
                        },
                        new IssueState()
                        {
                            IssueStateValue = IssueStateValue.NotResolved,
                            Name = "Not resolved"
                        }
                    });
                    break;
                case IssueStateValue.Approved:
                    newIssueStates.AddRange(new List<IssueState> {
                        new IssueState()
                        {
                            IssueStateValue = IssueStateValue.Resolved,
                            Name = "Resolved"
                        },
                        new IssueState()
                        {
                            IssueStateValue = IssueStateValue.NotResolved,
                            Name = "Not resolved"
                        }
                    });
                    break;
                case IssueStateValue.Resolved:
                    break;
                case IssueStateValue.NotResolved:
                    break;
            }
            NewIssueStates = newIssueStates.ToObservableCollection();
            SelectedNewIssueState = null;
        }

        private BitmapImage GetBitmapImage(string base64Picture)
        {
            byte[] binaryData = Convert.FromBase64String(base64Picture);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();
            return bi;
        }

        #endregion

        #region Main

        private void AddResponse()
        {
            MunicipalityResponses.Insert(0, new MunicipalityResponse()
            {
                DateCreated = DateTime.Now,
            });
        }

        private void UnblockCitizen()
        {
            if (MessageBox.Show("Are you sure you want to unblock this citizen?", "Unblock citizen", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            ShowProgress("Unblocking citizen...");
            Task.Run(async () =>
            {
                var response = await App.WASPService.UnblockCitizen(Issue.CitizenId);
                if (!response.IsSuccess)
                {
                    HideProgress(IssueDetailsView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    HideProgress(IssueDetailsView.Main);
                    CurrentWindow.Close();
                });
            });
        }

        private void BlockCitizen()
        {
            if (MessageBox.Show("Are you sure you want to block this citizen?", "Block citizen", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            ShowProgress("Blocking citizen...");
            Task.Run(async () =>
            {
                var response = await App.WASPService.BlockCitizen(Issue.CitizenId);
                if (!response.IsSuccess)
                {
                    HideProgress(IssueDetailsView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    HideProgress(IssueDetailsView.Main);
                    CurrentWindow.Close();
                });
            });
        }

        private void SaveAllChanges()
        {
            ShowProgress("Updating status...");
            Task.Run(async () =>
            {
                if (selectedNewIssueState != null && SelectedNewIssueState.IssueStateValue != Issue.IssueState.IssueStateValue)
                {
                    var response = await App.WASPService.UpdateIssueStatus(Issue.Id, SelectedNewIssueState.Id);
                    if (!response.IsSuccess)
                    {
                        HideProgress(IssueDetailsView.Main);
                        GeneralUtil.ShowMessage(response.ErrorMessage);
                        return;
                    }
                }
                ShowProgress("Creating responses...");
                var newResponses = MunicipalityResponses.Where(x => x.Id == 0 && !string.IsNullOrEmpty(x.Response)).ToList();
                if (newResponses.Count > 0)
                {
                    foreach (var item in newResponses)
                    {
                        var response = await App.WASPService.CreateMunicipalityResponse(new MunicipalityResponseCreate()
                        {
                            IssueId = Issue.Id,
                            MunicipalityUserId = App.MunicipalityUser.Id,
                            Response = item.Response
                        });
                        if (!response.IsSuccess)
                        {
                            HideProgress(IssueDetailsView.Main);
                            GeneralUtil.ShowMessage(response.ErrorMessage);
                            return;
                        }
                    }
                }
                ShowProgress("Checking response changes...");
                var editedResponses = new List<MunicipalityResponse>();
                var deletedResponses = new List<MunicipalityResponse>();
                foreach (var item in Issue.MunicipalityResponses)
                {
                    var temp = MunicipalityResponses.FirstOrDefault(x => x.Id == item.Id);
                    if (temp == null)
                        deletedResponses.Add(item);
                    else
                    {
                        if (temp.Response != item.Response)
                            editedResponses.Add(temp);
                    }
                }
                ShowProgress("Updating responses...");
                foreach (var item in editedResponses)
                {
                    var response = await App.WASPService.UpdateMunicipalityResponse(item.Id, new List<WASPUpdate>()
                    {
                        new WASPUpdate()
                        {
                            Name = "Response",
                            Value = item.Response
                        }
                    });
                    if (!response.IsSuccess)
                    {
                        HideProgress(IssueDetailsView.Main);
                        GeneralUtil.ShowMessage(response.ErrorMessage);
                        return;
                    }
                }
                ShowProgress("Deleting responses...");
                foreach (var item in deletedResponses)
                {
                    var response = await App.WASPService.DeleteMunicipalityResponse(item.Id);
                    if (!response.IsSuccess)
                    {
                        HideProgress(IssueDetailsView.Main);
                        GeneralUtil.ShowMessage(response.ErrorMessage);
                        return;
                    }
                }

                GeneralUtil.RunOnUIThread(() =>
                {
                    HideProgress(IssueDetailsView.Main);
                    CurrentWindow.Close();
                });
            });
        }

        #endregion

        #region Progress

        private void ShowProgress(string progressMessage)
        {
            View = IssueDetailsView.Progress;
            ProgressMessage = progressMessage;
        }
        private void HideProgress(IssueDetailsView view)
        {
            View = view;
        }

        #endregion

        #endregion

        #region Properties

        public IssueDetailsWindow CurrentWindow { get; set; }
        public IssueDetailsView View
        {
            get => view; set
            {
                SetValue(ref view, value);
                OnPropertyChanged(nameof(ProgressVisibility));
                OnPropertyChanged(nameof(MainVisibility));
            }
        }
        public bool IsBlocked { get => isBlocked; set => SetValue(ref isBlocked, value); }
        public string ProgressMessage { get => progressMessage; set => SetValue(ref progressMessage, value); }
        public Visibility ProgressVisibility => View == IssueDetailsView.Progress ? Visibility.Visible : Visibility.Hidden;
        public Visibility MainVisibility => View == IssueDetailsView.Main ? Visibility.Visible : Visibility.Hidden;
        public ObservableCollection<IssueState> NewIssueStates { get => newIssueStates; set => SetValue(ref newIssueStates, value); }
        public Issue Issue { get; set; }
        public List<BitmapImage> Pictures { get => pictures; set => SetValue(ref pictures, value); }
        public IssueState IssueState { get => issueState; set => SetValue(ref issueState, value); }
        public string DateCreated { get => dateCreated; set => SetValue(ref dateCreated, value); }
        public string DateEdited { get => dateEdited; set => SetValue(ref dateEdited, value); }
        public string Category { get => category; set => SetValue(ref category, value); }
        public string SubCategory { get => subCategory; set => SetValue(ref subCategory, value); }
        public string Description { get => description; set => SetValue(ref description, value); }
        public ObservableCollection<MunicipalityResponse> MunicipalityResponses { get => municipalityResponses; set => SetValue(ref municipalityResponses, value); }
        public ICommand SaveAllChangesCommand { get => saveAllChangesCommand; set => SetValue(ref saveAllChangesCommand, value); }
        public ICommand AddResponseCommand { get => addResponseCommand; set => SetValue(ref addResponseCommand, value); }
        public ICommand BlockCitizenCommand { get => blockCitizenCommand; set => SetValue(ref blockCitizenCommand, value); }
        public SimpleCommand<MunicipalityResponse> DeleteResponseCommand { get => deleteResponseCommand; set => SetValue(ref deleteResponseCommand, value); }
        public IssueState SelectedNewIssueState { get => selectedNewIssueState; set => SetValue(ref selectedNewIssueState, value); }

        #endregion
    }
}
