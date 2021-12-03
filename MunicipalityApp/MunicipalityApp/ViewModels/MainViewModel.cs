using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Maps.MapControl.WPF;
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
        private MunicipalityUser municipalityUser;

        // Overview
        private IssuesOverviewFilter issueFilter;
        private List<Issue> issuesOverviewCollection;
        private ObservableCollection<IssueState> issueStates;
        private ObservableCollection<Category> categories;
        private ObservableCollection<Category> subCategories;
        private RelayCommand<Category> categoryCheckedChangedCommand;
        private ICommand applyFilterCommand;

        // Reports


        // Blocked


        // Sign up


        #endregion

        #region Constructor

        public MainViewModel()
        {            
            MainView = MainView.Login;

            Setup();


            Categories = new ObservableCollection<Category>()
            {
                new Category()
                {
                    Id = 1,
                    Name = "Test",
                    IsSelected = true,
                    SubCategories = new List<SubCategory>()
                    {
                        new SubCategory()
                        {
                            Id = 1,
                            CategoryId = 1,
                            Name = "Test",
                            IsSelected = false
                        },
                        new SubCategory()
                        {
                            Id = 1,
                            CategoryId = 1,
                            Name = "Test",
                            IsSelected = true
                        }
                    }
                },
                new Category()
                {
                    Id = 2,
                    Name = "Test",
                    IsSelected = false,
                    SubCategories = new List<SubCategory>()
                    {
                        new SubCategory()
                        {
                            Id = 1,
                            CategoryId = 1,
                            Name = "Test",
                            IsSelected = false
                        },
                        new SubCategory()
                        {
                            Id = 1,
                            CategoryId = 1,
                            Name = "Test",
                            IsSelected = true
                        }
                    }
                }
            };

            Username = "grete@aalborg.dk";
            Password = "12345678";
            ProgressMessage = Resources.logging_in;
        }

        #endregion

        #region Methods

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
                }
                args.Handled = true;
            });

            SetupLogin();
            SetupOverview();
        }

        private void LogOut()
        {
            MainView = MainView.Login;
        }

        #endregion

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
                MunicipalityUser = response.WASPResponse.Result;
                HideProgress(MainView.Main);
                GeneralUtil.RunOnUIThread(() =>
                {
                    GetListOfIssues(getCategories: true); ;
                });
            });
        }

        #endregion

        #region Overview

        private void SetupOverview()
        {
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
                IsBlocked = false,
                IssueStateIds = IssueStates.Where(x => x.IsSelected).Select(x => x.Id).ToList()
            };

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
                                                      .Where(x => x.IsSelected = true)
                                                      .Select(x => x.Id)
                                                      .ToList();
                IssueFilter.CategoryIds = tempCategoryIds.Count == 0 ? null : tempCategoryIds;
                IssueFilter.SubCategoryIds = tempSubCategoryIds.Count == 0 ? null : tempSubCategoryIds;
                GetListOfIssues();
            });
        }

        private void PushPin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Pushpin pin = sender as Pushpin;

            GeneralUtil.ShowMessage(pin.Location.ToString());
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

        #region Properties

        #region General

        public MunicipalityUser MunicipalityUser
        {
            get => municipalityUser; set
            {
                municipalityUser = value;
                IssueFilter.MunicipalityIds = new List<int> { municipalityUser.MunicipalityId };
            }
        }
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
                    pushPin.MouseLeftButtonDown += PushPin_MouseDown;
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

        #endregion

        #region Progress

        public Visibility ProgressVisibility => MainView == MainView.Progress ? Visibility.Visible : Visibility.Hidden;
        public string ProgressMessage { get => progressMessage; set => SetValue(ref progressMessage, value); }

        #endregion

        #endregion

    }
}
