using MunicipalityApp.Webservices.WASP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MunicipalityApp
{
    /// <summary>
    /// Interaction logic for IssueDetailsWindow.xaml
    /// </summary>
    public partial class IssueDetailsWindow : Window
    {
        public IssueDetailsWindow(Issue issue, bool isBlocked)
        {
            InitializeComponent();

            ViewModel.CurrentWindow = this;
            ViewModel.Init(issue, isBlocked);
        }

        public IssueDetailsViewModel ViewModel { get => DataContext as IssueDetailsViewModel; set => DataContext = value; }
    }
}
