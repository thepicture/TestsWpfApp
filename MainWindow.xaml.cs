using System.Windows;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigationFrame.Navigate(new ProcessorWatchPage());
            Common.Navigator = NavigationFrame;
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            NavigationFrame.GoBack();
        }

        private void NavigateToSerialPortPage(object sender, RoutedEventArgs e)
        {
            NavigationFrame.Navigate(new SerialPortPage());
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
    }
}
