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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace MuseLabExhibitApplication
{
    /// <summary>
    /// Interaction logic for DancePage5.xaml
    /// </summary>
    public partial class DancePage5 : Page
    {
        public DancePage5()
        {
            InitializeComponent();
        }

       private GesturePak.GestureMatcher matcher;
        private WriteableBitmap _bitmap;
        private EventHandler<AllFramesReadyEventArgs> handler;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize Gestures

            string resetGestureFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Arms Crossed.xml";
            GesturePak.Gesture resetGesture = new GesturePak.Gesture(resetGestureFile);

            List<GesturePak.Gesture> gestures = new List<GesturePak.Gesture>();
            gestures.Add(resetGesture);

            matcher = new GesturePak.GestureMatcher(gestures);

            // Initialize Kinect
            Window mainWindow = Application.Current.MainWindow;
            KinectSensorChooser sensorChooser = ((MainWindow)mainWindow).GetKinectSensorChooser();

            handler = new EventHandler<AllFramesReadyEventArgs>(sensorChooserOnAllFramesReady);
            sensorChooser.Kinect.AllFramesReady += handler;
        }

        void actionGestureOnGestureRecognized()
        {
            // Detach Delegates
            Window mainWindow = Application.Current.MainWindow;
            ((MainWindow)mainWindow).GetKinectSensorChooser().Kinect.AllFramesReady -= handler;

            // Navigate to Next Page
            ((MainWindow)mainWindow).NavigateToPage("MainPage.xaml");
        }

        void resetGestureOnGestureRecognized()
        {
            // Detach Delegates
            Window mainWindow = Application.Current.MainWindow;
            ((MainWindow)mainWindow).GetKinectSensorChooser().Kinect.AllFramesReady -= handler;

            // Navigate to Main Page
            ((MainWindow)mainWindow).NavigateToPage("MainPage.xaml");
        }

        void sensorChooserOnAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            HandleColorFrameOutput(e.OpenColorImageFrame());
            HandleSkeletonFrameOutput(e.OpenSkeletonFrame());
        }

        private void HandleColorFrameOutput(ColorImageFrame frame)
        {
            using (ColorImageFrame colorFrame = frame)
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * sizeof(int);

                if (_bitmap == null)
                {
                    _bitmap = new WriteableBitmap(BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride));
                }
                _bitmap.WritePixels(new Int32Rect(0, 0, colorFrame.Width, colorFrame.Height), pixels, stride, 0);

                KinectVideoOutput.Source = _bitmap;
            }
        }

        private void HandleSkeletonFrameOutput(SkeletonFrame frame)
        {
            using (SkeletonFrame skeletonFrame = frame)
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];

                    skeletonFrame.CopySkeletonDataTo(skeletons);

                    if (skeletons.Length > 0)
                    {
                        var user = skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();

                        if (user != null)
                        {
                            matcher.ProcessRealTimeSkeletonData(user);
                        }
                    }
                }
                foreach (GesturePak.Gesture gesture in matcher.Gestures)
                {
                    if (gesture.Matched)
                    {
                        switch (gesture.Name)
                        {
                            case "Arms Crossed": resetGestureOnGestureRecognized(); break;
                        }
                    }
                }
            }
        }
    }
}
