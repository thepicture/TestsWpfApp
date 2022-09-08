using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for ProcessesListPage.xaml
    /// </summary>
    public partial class ProcessesListPage : Page
    {
        public ProcessesListPage()
        {
            InitializeComponent();
            var currentProcesses = Process.GetProcesses().Select(p => new
            {
                p.ProcessName,
                p.Responding,
                p.MachineName
            }).OrderBy(p => p.ProcessName);
            ProcessesList.ItemsSource = currentProcesses;
        }
    }
}
