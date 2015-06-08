using System.Windows;
using CandidateAssessment.ViewModels;

namespace CandidateAssessment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Lazy I know, but didn't think app warranted spinning up DI
            DataContext = new ShellViewModel();
        }
    }
}
