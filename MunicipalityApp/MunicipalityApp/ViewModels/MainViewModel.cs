using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Maps.MapControl.WPF;
using MunicipalityApp.Commands;
using MunicipalityApp.Webservices.WASP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Maps = Microsoft.Maps.MapControl.WPF;
using Resources = MunicipalityApp.Properties.Resources;

namespace MunicipalityApp
{
    public enum MainView
    {
        Login,
        Main,
        Progress
    }

    public class MainViewModel : BindableBase
    {
        /****************************************************************/
        // Variables
        /****************************************************************/
        #region Variables

        // General
        private MainView mainView;
        private RelayCommand<SelectionChangedEventArgs> tabControlSelectionChangedCommand;
        private ICommand windowLoadedCommand;

        // Progress
        private string progressMessage;

        // Login
        private string username;
        private string password;
        private ICommand loginCommand;        

        // Overview
        private IssuesOverviewFilter issueFilter;
        private List<Issue> issuesOverviewCollection;
        private ObservableCollection<IssueState> issueStates;
        private ObservableCollection<Category> categories;
        private ObservableCollection<Category> subCategories;
        private bool isBlocked;
        private RelayCommand<Category> categoryCheckedChangedCommand;
        private ICommand applyFilterCommand;

        // Reports
        private ObservableCollection<Issue> issuesWithReports;
        private ObservableCollection<Report> reports;
        private SimpleCommand<Issue> seeIssueDetailsCommand;
        private Issue selectedIssueWithReports;

        // Blocked
        private ObservableCollection<Citizen> blockedCitizens;
        private SimpleCommand<Citizen> unblockCitizenCommand;

        // Sign up
        private string newUsername;
        private string newPassword;
        private string newName;
        private ICommand signUpUserCommand;

        #endregion

        /****************************************************************/
        // Constructor
        /****************************************************************/
        #region Constructor

        public MainViewModel()
        {            
            MainView = MainView.Login;

            Username = "grete@aalborg.dk";
            Password = "12345678";

            Setup();
        }

        #endregion

        /****************************************************************/
        // Methods
        /****************************************************************/
        #region Methods

        /****************************************************/
        // General
        /****************************************************/
        #region General

        private void Setup()
        {
            /////////////////
            // Command - Window Loaded
            /////////////////
            WindowLoadedCommand = new RelayCommand<EventArgs>((args) =>
            {
                CurrentWindow.tbRoot.SelectedItem = CurrentWindow.tbRoot.Items[1];
            });
            /////////////////
            // Command - Tab control selection changed
            /////////////////
            TabControlSelectionChangedCommand = new RelayCommand<SelectionChangedEventArgs>((args) =>
            {
                var tabControl = args.Source as TabControl;
                if (tabControl == null)
                    return;

                int oldIndex = tabControl.Items.IndexOf(args.RemovedItems[0]);

                switch (tabControl.SelectedIndex)
                {
                    case 0:
                        tabControl.SelectedIndex = oldIndex;
                        if (MessageBox.Show(Resources.are_you_sure_you_want_to_log_out, Resources.log_out, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            LogOut();
                            tabControl.SelectedItem = CurrentWindow.tbRoot.Items[1];
                        }
                        break;
                    case 2:
                        GetReports();
                        break;
                    case 3:
                        GetBlockedCitizens();
                        break;
                }
                args.Handled = true;
            });

            SetupLogin();
            SetupOverview();
            SetupReports();
            SetupBlocked();
            SetupSignUp();
        }

        private void LogOut()
        {
            MainView = MainView.Login;
        }

        #endregion

        /****************************************************/
        // Login
        /****************************************************/
        #region Login

        private void SetupLogin()
        {
            /////////////////
            // Command - Log in
            /////////////////
            LogInCommand = new RelayCommand(() =>
            {
                Login();
            });
        }

        private void Login()
        {
            ShowProgress(Resources.logging_in);

            Task.Run(async () =>
            {
                var response = await App.WASPService.LogInMunicipality(new MunicipalityUserLogin()
                {
                    Email = Username,
                    Password = Password
                });
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Login);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                App.MunicipalityUser = response.WASPResponse.Result;
                IssueFilter.MunicipalityIds = new List<int> { App.MunicipalityUser.MunicipalityId };
                HideProgress(MainView.Main);
                GeneralUtil.RunOnUIThread(() =>
                {
                    ResetFilter();
                    GetListOfIssues(getCategories: true);
                });
            });
        }

        #endregion

        /****************************************************/
        // Overview
        /****************************************************/
        #region Overview

        private void SetupOverview()
        {
            ResetFilter();

            CategoryCheckedChangedCommand = new RelayCommand<Category>((category) =>
            {
                if (!category.IsSelected)
                {
                    SubCategories = SubCategories.Where(x => x.Id != category.Id).ToObservableCollection();
                }
                else
                {
                    category.SubCategories.ForEach(x => x.IsSelected = false);
                    SubCategories.Add(category);
                }
            });
            ApplyFilterCommand = new RelayCommand(() =>
            {
                var tempIssueStateIds = IssueStates.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                IssueFilter.IssueStateIds = tempIssueStateIds.Count == 0 ? null : tempIssueStateIds;
                var tempCategoryIds = Categories.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                IssueFilter.CategoryIds = tempCategoryIds.Count == 0 ? null : tempCategoryIds;
                var tempSubCategoryIds = SubCategories.SelectMany(x => x.SubCategories)
                                                      .Where(x => x.IsSelected == true)
                                                      .Select(x => x.Id)
                                                      .ToList();
                IssueFilter.CategoryIds = tempCategoryIds.Count == 0 ? null : tempCategoryIds;
                IssueFilter.SubCategoryIds = tempSubCategoryIds.Count == 0 ? null : tempSubCategoryIds;
                IssueFilter.IsBlocked = IsBlocked;
                GetListOfIssues();
            });
        }

        private void ResetFilter()
        {
            IsBlocked = false;
            IssueStates = new ObservableCollection<IssueState>()
            {
                new IssueState()
                {
                    Id = 1,
                    IsSelected = true,
                    Name = "Created"
                },
                new IssueState()
                {
                    Id = 2,
                    IsSelected = true,
                    Name = "Approved"
                },
                new IssueState()
                {
                    Id = 3,
                    IsSelected = false,
                    Name = "Resolved"
                },
                new IssueState()
                {
                    Id = 4,
                    IsSelected = false,
                    Name = "Not resolved"
                }
            };

            IssueFilter = new IssuesOverviewFilter()
            {
                IsBlocked = IsBlocked,
                IssueStateIds = IssueStates.Where(x => x.IsSelected).Select(x => x.Id).ToList()
            };
        }
        
        private void GetListOfIssues(bool getCategories = false)
        {
            ShowProgress(Resources.getting_issues);
            Task.Run(async () =>
            {
                var response = await App.WASPService.GetListOfIssues(IssueFilter);
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    IssuesOverviewCollection = response.WASPResponse.Result;
                    HideProgress(MainView.Main);
                    if (getCategories)
                        GetListOfCategories();
                });
            });
        }
        private void GetIssueDetails(Issue issue, bool isBlocked, Action closedAction)
        {
            ShowProgress(Resources.getting_issue_details);
            Task.Run(async () =>
            {
                var response = await App.WASPService.GetIssueDetails(issue.Id);
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    HideProgress(MainView.Main);
                    IssueDetailsWindow issueDetailsWindow = new IssueDetailsWindow(response.WASPResponse.Result, isBlocked);
                    issueDetailsWindow.Closed += (sender, args) =>
                    {
                        closedAction();
                    };
                    issueDetailsWindow.ShowDialog();
                });
            });
        }
        private void GetListOfCategories()
        {
            ShowProgress(Resources.getting_categories);
            Task.Run(async () =>
            {
                var response = await App.WASPService.GetListOfCategories();
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    Categories = response.WASPResponse.Result.ToObservableCollection();
                    SubCategories = new ObservableCollection<Category>();
                    HideProgress(MainView.Main);
                });
            });
        }

        #endregion

        /****************************************************/
        // Reports
        /****************************************************/
        #region Reports

        private void SetupReports()
        {
            SeeIssueDetailsCommand = new SimpleCommand<Issue>((issue) =>
            {
                GetIssueDetails(issue, issue.Citizen.IsBlocked, () =>
                {
                    GetReports();
                });
            });
        }
        private void GetReports()
        {
            ShowProgress("Getting reports...");
            Task.Run(async () =>
            {
                var response = await App.WASPService.GetListOfReports(App.MunicipalityUser.MunicipalityId);
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }

                GeneralUtil.RunOnUIThread(() =>
                {
                    Reports = null;
                    SelectedIssueWithReports = null;
                    IssuesWithReports = response.WASPResponse.Result.ToObservableCollection();
                    foreach (var item in IssuesWithReports)
                    {
                        item.SeeIssueDetailsCommand = SeeIssueDetailsCommand;
                    }
                    HideProgress(MainView.Main);                    
                });
            });
        }

        #endregion

        /****************************************************/
        // Blocked
        /****************************************************/
        #region Blocked

        private void SetupBlocked()
        {
            UnblockCitizenCommand = new SimpleCommand<Citizen>((citizen) =>
            {
                UnblockCitizen(citizen);
            });
        }

        private void GetBlockedCitizens()
        {
            ShowProgress("Getting blocked citizens...");
            Task.Run(async () =>
            {
                var response = await App.WASPService.GetListOfCitizens(App.MunicipalityUser.MunicipalityId, true);
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    BlockedCitizens = response.WASPResponse.Result.ToObservableCollection();
                    foreach(var item in BlockedCitizens)
                    {
                        item.UnblockCitizenCommand = UnblockCitizenCommand;
                    }                    
                    HideProgress(MainView.Main);
                });
            });
        }

        private void UnblockCitizen(Citizen citizen)
        {
            if (MessageBox.Show("Are you sure you want to unblock this citizen?", "Unblock citizen", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            ShowProgress("Unblocking citizen...");
            Task.Run(async () =>
            {
                var response = await App.WASPService.UnblockCitizen(citizen.Id);
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    HideProgress(MainView.Main);
                    GetBlockedCitizens();     
                });
            });
        }

        #endregion

        /****************************************************/
        // Sign up
        /****************************************************/
        #region Sign up

        private void SetupSignUp()
        {
            SignUpUserCommand = new SimpleCommand(() =>
            {
                SignUpMunicipality();
            });
        }

        private void SignUpMunicipality()
        {
            if (string.IsNullOrEmpty(NewUsername) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(NewName))
            {
                GeneralUtil.ShowMessage("Please fill out all fields");
                return;
            }

            ShowProgress("Signing up new municipality user...");
            Task.Run(async () =>
            {
                var response = await App.WASPService.SignUpMunicipalityUser(new MunicipalityUserSignUp()
                {
                    Email = NewUsername,
                    Name = NewName,
                    MunicipalityId = App.MunicipalityUser.MunicipalityId,
                    Password = NewPassword
                });
                if (!response.IsSuccess)
                {
                    HideProgress(MainView.Main);
                    GeneralUtil.ShowMessage(response.ErrorMessage);
                    return;
                }
                GeneralUtil.RunOnUIThread(() =>
                {
                    GeneralUtil.ShowMessage("New municipality user signed up");
                    NewUsername = string.Empty;
                    NewPassword = string.Empty;
                    NewName = string.Empty;
                    HideProgress(MainView.Main);
                });
            });
        }

        #endregion

        /****************************************************/
        // Progress
        /****************************************************/
        #region Progress

        private void ShowProgress(string progressMessage)
        {
            MainView = MainView.Progress;
            ProgressMessage = progressMessage;
        }
        private void HideProgress(MainView mainView)
        {
            MainView = mainView;
        }

        #endregion

        #endregion

        /****************************************************************/
        // Properties
        /****************************************************************/
        #region Properties

        /****************************************************/
        // General
        /****************************************************/
        #region General

        public MainWindow CurrentWindow { get; set; }        
        public Visibility MainViewVisibility => MainView == MainView.Main ? Visibility.Visible : Visibility.Hidden;
        public Visibility LoginViewVisibility => MainView == MainView.Login ? Visibility.Visible : Visibility.Hidden;
        public RelayCommand<SelectionChangedEventArgs> TabControlSelectionChangedCommand
        {
            get => tabControlSelectionChangedCommand;
            set => SetValue(ref tabControlSelectionChangedCommand, value);
        }
        public MainView MainView
        {
            get => mainView; set
            {
                mainView = value;
                OnPropertyChanged(nameof(LoginViewVisibility));
                OnPropertyChanged(nameof(MainViewVisibility));
                OnPropertyChanged(nameof(ProgressVisibility));
            }
        }
        public ICommand WindowLoadedCommand { get => windowLoadedCommand; set => SetValue(ref windowLoadedCommand, value); }

        #endregion

        /****************************************************/
        // Login
        /****************************************************/
        #region Login

        public ICommand LogInCommand { get => loginCommand; set => SetValue(ref loginCommand, value); }
        public string Username { get => username; set => SetValue(ref username, value); }
        public string Password
        {
            get => password; set
            {
                if (CurrentWindow != null && CurrentWindow.pbLogin.Password != value)
                    CurrentWindow.pbLogin.Password = value;
                SetValue(ref password, value);
            }
        }

        #endregion

        /****************************************************/
        // Overview
        /****************************************************/
        #region Overview

        public Map Map => CurrentWindow.mapControl;
        public IssuesOverviewFilter IssueFilter { get => issueFilter; set => SetValue(ref issueFilter, value); }
        public List<Issue> IssuesOverviewCollection
        {
            get => issuesOverviewCollection;
            set
            {
                issuesOverviewCollection = value;

                Map.Children.Clear();

                foreach (var issue in IssuesOverviewCollection)
                {
                    var pushPin = new Pushpin();
                    var location = issue.Location;
                    pushPin.Location = new Maps.Location(location.Latitude, location.Longitude);
                    pushPin.Background = issue.IssueState.StateColor;
                    pushPin.MouseLeftButtonDown += (sender, args) =>
                    {
                        GetIssueDetails(issue, IsBlocked, () => GetListOfIssues());
                    };
                    Map.Children.Add(pushPin);
                }
            }
        }
        public ObservableCollection<IssueState> IssueStates { get => issueStates; set => SetValue(ref issueStates, value); }
        public ObservableCollection<Category> Categories
        {
            get => categories; set
            {
                var temp = value;
                foreach (var item in temp)
                {
                    item.CheckedChangedCommand = CategoryCheckedChangedCommand;
                }
                SetValue(ref categories, value);
            }
        }
        public ObservableCollection<Category> SubCategories { get => subCategories; set => SetValue(ref subCategories, value); }
        public RelayCommand<Category> CategoryCheckedChangedCommand { get => categoryCheckedChangedCommand; set => SetValue(ref categoryCheckedChangedCommand, value); }
        public ICommand ApplyFilterCommand { get => applyFilterCommand; set => SetValue(ref applyFilterCommand, value); }
        public bool IsBlocked { get => isBlocked; set => SetValue(ref isBlocked, value); }

        #endregion

        /****************************************************/
        // Reports
        /****************************************************/
        #region Reports

        public ObservableCollection<Issue> IssuesWithReports { get => issuesWithReports; set => SetValue(ref issuesWithReports, value); }
        public ObservableCollection<Report> Reports { get => reports; set => SetValue(ref reports, value); }
        public SimpleCommand<Issue> SeeIssueDetailsCommand { get => seeIssueDetailsCommand; set => SetValue(ref seeIssueDetailsCommand, value); }
        public Issue SelectedIssueWithReports { get => selectedIssueWithReports; set
            {
                SetValue(ref selectedIssueWithReports, value);
                if (SelectedIssueWithReports != null)
                {
                    Reports = SelectedIssueWithReports.Reports.ToObservableCollection();
                }
            }
        }

        #endregion

        /****************************************************/
        // Blocked
        /****************************************************/
        #region Blocked

        public ObservableCollection<Citizen> BlockedCitizens { get => blockedCitizens; set => SetValue(ref blockedCitizens, value); }
        public SimpleCommand<Citizen> UnblockCitizenCommand { get => unblockCitizenCommand; set => SetValue(ref unblockCitizenCommand, value); }

        #endregion

        /****************************************************/
        // Sign up
        /****************************************************/
        #region Sign up

        public ICommand SignUpUserCommand { get => signUpUserCommand; set => SetValue(ref signUpUserCommand, value); }
        public string NewUsername { get => newUsername; set => SetValue(ref newUsername, value); }
        public string NewPassword { get => newPassword; set => SetValue(ref newPassword, value); }
        public string NewName { get => newName; set => SetValue(ref newName, value); }

        #endregion

        /****************************************************/
        // Progress
        /****************************************************/
        #region Progress

        public Visibility ProgressVisibility => MainView == MainView.Progress ? Visibility.Visible : Visibility.Hidden;
        public string ProgressMessage { get => progressMessage; set => SetValue(ref progressMessage, value); }

        #endregion

        #endregion

    }
}
