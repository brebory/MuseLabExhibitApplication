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
using MuseLabExhibitApplication.Gestures;

namespace MuseLabExhibitApplication
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private GesturePak.GestureMatcher matcher;
        private EventHandler<AllFramesReadyEventArgs> handler;

        public MainPage()
        {
            InitializeComponent();
        }

        private WriteableBitmap _bitmap;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize Gestures

            string actionGestureFile1 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Hat Tip LH.xml";
            GesturePak.Gesture actionGesture1 = new GesturePak.Gesture(actionGestureFile1);

            string actionGestureFile2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Hat Tip RH.xml";
            GesturePak.Gesture actionGesture2 = new GesturePak.Gesture(actionGestureFile2);

            List<GesturePak.Gesture> gestures = new List<GesturePak.Gesture>();
            gestures.Add(actionGesture1);
            gestures.Add(actionGesture2);

            matcher = new GesturePak.GestureMatcher(gestures);  
            
            // Initialize Kinect
            Window mainWindow = Application.Current.MainWindow;
            KinectSensorChooser sensorChooser = ((MainWindow)mainWindow).GetKinectSensorChooser();

            if (sensorChooser.Kinect != null)
            {
                handler = new EventHandler<AllFramesReadyEventArgs>(sensorChooserOnAllFramesReady);
                sensorChooser.Kinect.AllFramesReady += handler;
            }

            InstructionsVideo.Play();
        }

        void actionGestureOnGestureRecognized()
        {
            // Detach Delegates
            Window mainWindow = Application.Current.MainWindow;
            ((MainWindow)mainWindow).GetKinectSensorChooser().Kinect.AllFramesReady -= handler;

            // Navigate to next page
            ((MainWindow)mainWindow).NavigateToPage("DancePage1.xaml");
        }

        void resetGestureOnGestureRecognized()
        {
            // Detach Delegates
            Window mainWindow = Application.Current.MainWindow;
            ((MainWindow)mainWindow).GetKinectSensorChooser().Kinect.AllFramesReady -= handler;

            // Navigate to home page
            ((MainWindow)mainWindow).NavigateToPage("MainPage.xaml");
        }

        void sensorChooserOnAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            HandleColorFrameOutput(e.OpenColorImageFrame());
            HandleSkeletonFrameOutput(e.OpenSkeletonFrame());
        }

        private void HandleColorFrameOutput(ColorImageFrame colorFrame)
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

            colorFrame.Dispose();
        }

        private void HandleSkeletonFrameOutput(SkeletonFrame skeletonFrame)
        {
            if (skeletonFrame == null)
            {
                return;
            }

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

            foreach (GesturePak.Gesture gesture in matcher.Gestures)
            {
                if (gesture.Matched)
                {
                    switch (gesture.Name)
                    {
                        case "Hat Tip RH": 
                        case "Hat Tip LH": actionGestureOnGestureRecognized(); break;
                    }
                }
            }

            skeletonFrame.Dispose();
        }

        private void InstructionsVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            InstructionsVideo.Position = TimeSpan.FromSeconds(0);
            InstructionsVideo.Play();
        }
    }
}
