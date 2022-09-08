using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for ProcessorWatchPage.xaml
    /// </summary>
    public partial class ProcessorWatchPage : Page
    {

        public ProcessorWatchPage()
        {
            InitializeComponent();
            DataContext = this;
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += OnTick;
            timer.Start();
            Unloaded += (_, __) => timer?.Stop();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var currentProcesses = Process.GetProcesses().Select(p => new
            {
                p.ProcessName,
                p.Responding,
                p.MachineName,
                ImagePath = Path.Combine(Environment.CurrentDirectory, "Resources", "logo.png")
            }).OrderBy(p => p.ProcessName);
            if (currentProcesses.Any(p => p.ProcessName.ToLower().Contains("shadowsocks")))
            {
                MessageBox.Show("Turn off shadowsocks");
            }
            ProcessesGrid.ItemsSource = ComboMultiBinding.ItemsSource = currentProcesses;
        }
    }
}
