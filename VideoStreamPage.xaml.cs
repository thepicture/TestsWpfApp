using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Point = System.Drawing.Point;

namespace TestsWpfApp
{
    /// <summary>
    /// Interaction logic for VideoStreamPage.xaml
    /// </summary>
    public partial class VideoStreamPage : Page
    {
        public VideoStreamPage()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(50) };
            timer.Tick += UpdateVideoStream;
            timer.Start();
        }

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr ptr);

        private void UpdateVideoStream(object sender, EventArgs e)
        {
            using (Bitmap bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(Point.Empty, Point.Empty, bitmap.Size);
                }
                var pointer = bitmap.GetHbitmap();
                VideoStream.Source = Imaging.CreateBitmapSourceFromHBitmap(pointer, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(pointer);
            }
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
        }

        private Bitmap ByteArrayToBitmap(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
                return new Bitmap(stream);
        }

        private BitmapImage ByteArrayToImageStream(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                return image;
            }
        }
    }
}
