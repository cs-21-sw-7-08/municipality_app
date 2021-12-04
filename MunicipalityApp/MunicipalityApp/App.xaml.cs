using MunicipalityApp.Webservices.WASP;
using MunicipalityApp.Webservices.WASP.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MunicipalityApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static SolidColorBrush BrushIssueStateCreated { get; set; }
        public static SolidColorBrush BrushIssueStateApproved { get; set; }
        public static SolidColorBrush BrushIssueStateResolved { get; set; }
        public static SolidColorBrush BrushIssueStateNotResolved { get; set; }

        public static void Setup()
        {
            BrushIssueStateCreated = Current.Resources["brushIssueStateCreated"] as SolidColorBrush;
            BrushIssueStateApproved = Current.Resources["brushIssueStateApproved"] as SolidColorBrush;
            BrushIssueStateResolved = Current.Resources["brushIssueStateResolved"] as SolidColorBrush;
            BrushIssueStateNotResolved = Current.Resources["brushIssueStateNotResolved"] as SolidColorBrush;
        }

        public static List<IssueState> IssueStates => new List<IssueState>()
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

        public static IWASPServiceFunctions WASPService { get; private set; }
        public static MunicipalityUser MunicipalityUser { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            WASPService = new WASPService("https://localhost:5001");

            // Get resources
            Setup();
        }
    }
}
