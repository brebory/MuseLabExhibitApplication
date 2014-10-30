﻿using System;
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
    /// Interaction logic for DancePage4.xaml
    /// </summary>
    public partial class DancePage4 : Page
    {
        public DancePage4()
        {
            InitializeComponent();
        }

        private GesturePak.GestureMatcher matcher;
        private WriteableBitmap _bitmap;
        private EventHandler<AllFramesReadyEventArgs> handler;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize Gestures

            string actionGestureFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Bow RH.xml";
            GesturePak.Gesture actionGesture = new GesturePak.Gesture(actionGestureFile);

            string actionGestureFile1 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Bow LH.xml";
            GesturePak.Gesture actionGesture1 = new GesturePak.Gesture(actionGestureFile1);

            //string actionGestureFile2 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Fox Bow RH.xml";
            //GesturePak.Gesture actionGesture2 = new GesturePak.Gesture(actionGestureFile2);

            //string actionGestureFile3 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Fox Bow LH.xml";
            //GesturePak.Gesture actionGesture3 = new GesturePak.Gesture(actionGestureFile3);

            string resetGestureFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\GesturePak\\Arms Crossed.xml";
            GesturePak.Gesture resetGesture = new GesturePak.Gesture(resetGestureFile);

            List<GesturePak.Gesture> gestures = new List<GesturePak.Gesture>();
            gestures.Add(actionGesture);
            gestures.Add(actionGesture1);
            //gestures.Add(actionGesture2);
            //gestures.Add(actionGesture3);
            gestures.Add(resetGesture);

            matcher = new GesturePak.GestureMatcher(gestures);

            // Initialize Kinect
            Window mainWindow = Application.Current.MainWindow;
            KinectSensorChooser sensorChooser = ((MainWindow)mainWindow).GetKinectSensorChooser();

            handler = new EventHandler<AllFramesReadyEventArgs>(sensorChooserOnAllFramesReady);
            sensorChooser.Kinect.AllFramesReady += handler;

            InstructionsVideo.Play();
        }

        void actionGestureOnGestureRecognized()
        {
            // Detach Delegates
            Window mainWindow = Application.Current.MainWindow;
            ((MainWindow)mainWindow).GetKinectSensorChooser().Kinect.AllFramesReady -= handler;

            // Navigate to Next Page
            ((MainWindow)mainWindow).NavigateToPage("DancePage5.xaml");
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
                            case "Bow RH":
                            case "Bow LH":
                            case "Fox Bow RH":
                            case "Fox Bow LH": actionGestureOnGestureRecognized(); break;
                        }
                    }
                }
            }
        }

        private void InstructionsVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            InstructionsVideo.Position = TimeSpan.FromSeconds(0);
            InstructionsVideo.Play();
        }
    }
}
