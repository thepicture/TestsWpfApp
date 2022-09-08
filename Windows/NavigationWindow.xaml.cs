using System.Windows;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public NavigationWindow()
        {
            InitializeComponent();
            NavigationFrame.Navigate(new ProcessorWatchPage());
            Common.Navigator = NavigationFrame;
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            NavigationFrame.GoBack();
        }

        private void OnFrameRendered(object sender, System.EventArgs e)
        {
            if (NavigationFrame.CanGoBack)
                BackButton.Visibility = Visibility.Visible;
            else
                BackButton.Visibility = Visibility.Collapsed;
        }

        private void NavigateToVideoStreamPage(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new VideoStreamPage());
        }

        private void NavigateToHardwarePage(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new HardwarePage());
        }

        private void NavigateToProcessesPage(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new ProcessesListPage());
        }

        private void NavigateToProcessorWatchPage(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new ProcessorWatchPage());
        }
    }
}
