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
using System.Windows.Media.Animation;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;

namespace MuseLabExhibitApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Storyboard frameFadeOutStoryboard;
        private Storyboard frameFadeInStoryboard;
        private KinectSensorChooser sensorChooser;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Returns the KinectSensor currently attached to the KinectSensorChooser
        /// </summary>
        /// <returns></returns>
        public KinectSensorChooser GetKinectSensorChooser()
        {
            return sensorChooser;
        }

        public void NavigateToPage(String uriString)
        {
            frameFadeOutStoryboard.Begin(this);
            NavFrame.Navigate(new Uri(uriString, UriKind.RelativeOrAbsolute));
            NavFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            frameFadeInStoryboard.Begin(this);
        }

        private void SetupAnimations()
        {
            // Create animations
            DoubleAnimation fadeOut = new DoubleAnimation();
            fadeOut.From = 1.0;
            fadeOut.To = 0.0;
            fadeOut.Duration = TimeSpan.FromSeconds(2.0);

            DoubleAnimation fadeIn = new DoubleAnimation();
            fadeIn.From = 0.0;
            fadeIn.To = 1.0;
            fadeIn.Duration = TimeSpan.FromSeconds(2.0);

            // Set up storyboards
            frameFadeOutStoryboard = new Storyboard();
            frameFadeInStoryboard = new Storyboard();

            frameFadeOutStoryboard.Children.Add(fadeOut);
            frameFadeInStoryboard.Children.Add(fadeIn);

            Storyboard.SetTargetName(fadeOut, NavFrame.Name);
            Storyboard.SetTargetName(fadeIn, NavFrame.Name);

            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(Frame.OpacityProperty));
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(Frame.OpacityProperty));
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartKinectSensorChooser()
        {
            sensorChooser.KinectChanged += sensorChooserOnKinectChanged;
            this.SensorChooserUI.KinectSensorChooser = sensorChooser;
            sensorChooser.Start();
        }

        /// <summary>
        /// Initializes the KinectSensorChooser, KinectSensorChooserUI and sets up event handlers for KinectSensor and KinectSensorChooser
        /// </summary>
        private void StartKinect(KinectSensor sensor) 
        {
            if (sensor != null)
            {
                sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                sensor.SkeletonStream.Enable();

                try
                {
                    sensor.Start();
                    this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
                }
                catch (System.IO.IOException)
                {
                    SensorChooserUI.Content = "There was an unexpected error setting up your Kinect. Please check the connection and try again.";
                }
            }
        }

        /// <summary>
        /// Event Handler for the KinectSensorChooser KinectChanged event
        /// </summary>
        /// <param name="sender">Object that fired the KinectChanged event</param>
        /// <param name="e">Event arguments for the KinectChanged event</param>
        void sensorChooserOnKinectChanged(object sender, KinectChangedEventArgs e)
        {
            KinectSensor previousSensor = (KinectSensor)e.OldSensor;
            StopKinect(previousSensor);
            KinectSensor newSensor = (KinectSensor)e.NewSensor;
            StartKinect(newSensor);
        }

        /// <summary>
        /// Unloads KinectSensor and stops KinectSensorChooser from trying to find new KinectSensors
        /// </summary>
        private void StopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    sensor.Stop();
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }
                }
                sensor.SkeletonStream.Disable();
                sensor.ColorStream.Disable();
                sensor.DepthStream.Disable();
                sensor.Stop();
                sensor.Dispose();
            }
        }

        /// <summary>
        /// Event Handler for the WindowLoaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetupAnimations();
            sensorChooser = new KinectSensorChooser();
            StartKinectSensorChooser();
        }

        /// <summary>
        /// Event Handler for the WindowClosing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopKinect(sensorChooser.Kinect);
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    try
                    {
                        GetKinectSensorChooser().Kinect.ElevationAngle -= 5;
                    }
                    catch (Exception ex)
                    {
                        // Do nothing
                    }
                    break;
                case Key.Up:
                    try
                    {
                        GetKinectSensorChooser().Kinect.ElevationAngle += 5;
                    }
                    catch (Exception ex)
                    {
                        // Do nothing
                    }
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage("MainPage.xaml");
        }
    }
}
