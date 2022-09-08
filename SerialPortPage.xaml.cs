using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for SerialPortPage.xaml
    /// </summary>
    public partial class SerialPortPage : Page, INotifyPropertyChanged
    {
        private ObservableCollection<string> messages;
        DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(20) };

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Messages
        {
            get => messages; set
            {
                messages = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Messages)));
            }
        }
        public SerialPortPage()
        {
            InitializeComponent();
            DataContext = this;
            //UpdateComPorts();
            DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(20) };
            _timer.Tick += OnUpdateVideoStream;
            _timer.Start();
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr ptr);

        private void OnUpdateVideoStream(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }
            IntPtr pointer = bitmap.GetHbitmap();
            Dispatcher.Invoke(() =>
            {
                return VideoStream.Source = Imaging.CreateBitmapSourceFromHBitmap(pointer, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            });

            DeleteObject(pointer);
        }

        private void UpdateComPorts()
        {
            SerialPort port = new SerialPort("COM1") { ReadTimeout = 500 };
            port.DataReceived += Port_DataReceived;
            port.Open();
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Dispatcher.Invoke(() => Messages.Add((sender as SerialPort).ReadLine()));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                UpdateComPorts();
            else
                _timer.Stop();
        }

        private void OnSendComMessage(object sender, RoutedEventArgs e)
        {
            new SerialPort("COM1").WriteLine("hello");
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            _timer?.Stop();
        }
    }
}
