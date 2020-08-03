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
using System.IO;
using Coding4Fun.Kinect.Wpf;
using System.Reflection;
using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Windows.Point;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace EngineeringProjectApp
{
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        const float height = 480.0f;
        const float width = 640.0f;
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
        public MainWindow()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            TransformSmoothParameters smoothParameters = new TransformSmoothParameters();
            {
                smoothParameters.Smoothing = 0.7f;
                smoothParameters.Correction = 0.3f;
                smoothParameters.Prediction = 1.0f;
                smoothParameters.JitterRadius = 1.0f;
                smoothParameters.MaxDeviationRadius = 1.0f;
            }
            this.drawingGroup = new DrawingGroup();
            this.imageSource = new DrawingImage(this.drawingGroup);
            Image.Source = this.imageSource;

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                this.sensor.SkeletonStream.Enable(smoothParameters);
                this.sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;

                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            using (DrawingContext dc = this.drawingGroup.Open())
            {
                //Assembly myAssembly = Assembly.GetExecutingAssembly();
                //Stream myStream = myAssembly.GetManifestResourceStream("EngineeringProjectApp.images.background.bmp");
                // Bitmap bmp = new Bitmap(myStream);
                //Bitmap myImage = (Bitmap)Properties.Resources.ResourceManager.GetObject("backround.png");
                Bitmap bmp = Properties.Resources.Background;
                //dc.DrawImage(new BitmapImage(new Uri(Properties.Resources.Background));
                //dc.DrawImage(new BitmapImage(new Uri("pack://application:,,,/AssemblyName;component/Resources/Background.png")), new Rect(0.0, 0.0,width,height));
                //String uri = "pack://EngineeringProjectApp:,,,/AssemblyName;component/Resources/Background.png";
                System.Drawing.Image image = bmp;

             

                //dc.DrawImage(image.,new Rect(0.0,0.0,width,height));
                dc.DrawImage(new BitmapImage(new Uri("../../Resources/background.png", UriKind.Relative)),new Rect(0.0, 0.0, width, height));
                if (skeletons.Length != 0)
                {
                   
                    Skeleton skel=skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                    if (skel != null)
                    {
                        Brush drawBrush = null;
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            var rightHand = skel.Joints[JointType.HandRight];
                            CheckBoundaries(this.SkeletonPointToScreen(rightHand.Position), dc);
                            if (rightHand.TrackingState == JointTrackingState.Tracked)
                            {
                                drawBrush = this.trackedJointBrush;
                            }
                            else if (rightHand.TrackingState == JointTrackingState.Inferred)
                            {
                                drawBrush = this.trackedJointBrush;
                            }

                            if (drawBrush != null)
                            {
                                dc.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(rightHand.Position), 10.0f, 10.0f);
                                //Canvas.SetLeft(OrangeDot,this.SkeletonPointToScreen(rightHand.Position).X);
                                //Canvas.SetTop(OrangeDot, this.SkeletonPointToScreen(rightHand.Position).Y);
                            }
                        }
                    }
                }
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, width, height));
            }
        }

       
        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
        
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }
        private void CheckBoundaries(Point jointPoint, DrawingContext drawingContext)
        {
            if (jointPoint.Y>=470)
            {
                //System.Media.SystemSounds.Asterisk.Play();
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, height - 10.0f, width, 10.0f));
                
            }

            if (jointPoint.Y <= 10)
            {
                // System.Media.SystemSounds.Asterisk.Play();
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, width, 10.0f));
                
            }

            if (jointPoint.X <= 10)
            {
                //System.Media.SystemSounds.Asterisk.Play();
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, 10.0f, height));
                
            }

            if (jointPoint.X >= 630)
            {
                //System.Media.SystemSounds.Asterisk.Play();
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(width - 10.0f, 0, 10.0f, width));
                
            }
        }
    }


}
