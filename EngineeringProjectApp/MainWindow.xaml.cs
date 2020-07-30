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
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;
using System.Globalization;

namespace EngineeringProjectApp
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            var sensorStatus = new KinectSensorChooser();
            this.drawingGroup = new DrawingGroup();
            this.imageSource = new DrawingImage(this.drawingGroup);

            sensorStatus.KinectChanged += KinectSensorChooserKinectChanged;

            kinectChooser.KinectSensorChooser = sensorStatus;
            sensorStatus.Start();

        }
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        private void KinectSensorChooserKinectChanged(object sender, KinectChangedEventArgs e)
        {

            if (sensor != null)
                sensor.SkeletonFrameReady -= KinectSkeletonFrameReady;

            sensor = e.NewSensor;

            if (sensor == null)
                return;

            switch (Convert.ToString(e.NewSensor.Status))
            {
                case "Connected":
                    KinectStatus.Content = "Connected";
                    break;
                case "Disconnected":
                    KinectStatus.Content = "Disconnected";
                    break;
                case "Error":
                    KinectStatus.Content = "Error";
                    break;
                case "NotReady":
                    KinectStatus.Content = "Not Ready";
                    break;
                case "NotPowered":
                    KinectStatus.Content = "Not Powered";
                    break;
                case "Initializing":
                    KinectStatus.Content = "Initialising";
                    break;
                default:
                    KinectStatus.Content = "Undefined";
                    break;
            }

            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += KinectSkeletonFrameReady;

        }


        private void SensorDepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            throw new NotImplementedException();
        }
        //private void DrawJoint() {
        //    DrawLin
        //}

        private void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var skeletons = new Skeleton[0];

            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length == 0)
            {
                return;
            }

            var skel = skeletons.FirstOrDefault(x => x.TrackingState == SkeletonTrackingState.Tracked);
            if (skel == null)
            {
                return;
            }

            var rightHand = skel.Joints[JointType.WristRight];
            var rightHandPosition = rightHand.Position;
            XValueRight.Text = rightHand.Position.X.ToString(CultureInfo.InvariantCulture);
            YValueRight.Text = rightHand.Position.Y.ToString(CultureInfo.InvariantCulture);
            ZValueRight.Text = rightHand.Position.Z.ToString(CultureInfo.InvariantCulture);

            var leftHand = skel.Joints[JointType.WristLeft];
            var leftHandPosition = leftHand.Position;
            XValueLeft.Text = leftHand.Position.X.ToString(CultureInfo.InvariantCulture);
            YValueLeft.Text = leftHand.Position.Y.ToString(CultureInfo.InvariantCulture);
            ZValueLeft.Text = leftHand.Position.Z.ToString(CultureInfo.InvariantCulture);
            imageCanvas.Background = Brushes.PapayaWhip;

            var mapper = new CoordinateMapper(sensor);
            var colorPoint = mapper.MapSkeletonPointToColorPoint(rightHandPosition, ColorImageFormat.RgbResolution640x480Fps30);

            var circle = CreateCircle(colorPoint);
            imageCanvas.Children.Add(circle);


            //var centreHip = sknel.Joints[JointType.HipCenter];

            //if (centreHip.Position.Z - rightHand.Position.Z > 0.3)
            //{
            //    RightRaised.Text = "Raised";
            //}
            //else if (centreHip.Position.Z - leftHand.Position.Z > 0.3)
            //{
            //    LeftRaised.Text = "Raised";
            //}
            //else
            //{
            //    LeftRaised.Text = "Lowered";
            //    RightRaised.Text = "Lowered";
            //}
        }
        private Shape CreateCircle(ColorImagePoint colorPoint)
        {
            var circle = new Ellipse();
            circle.Fill = Brushes.Red;
            circle.Height = 20;
            circle.Width = 20;
            circle.Stroke = Brushes.Red;

            Canvas.SetLeft(circle, colorPoint.X);
            Canvas.SetTop(circle, colorPoint.Y);

            return circle;
        }
    }
}
