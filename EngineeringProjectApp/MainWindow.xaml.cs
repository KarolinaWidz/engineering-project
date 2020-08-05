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
        private Image test;
        const int height = 720;
        const int width = 960;
        private Boolean isDrag = false;
        private Item[] itemsArray;
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
                smoothParameters.Prediction = 0.5f;
                smoothParameters.JitterRadius = 1.0f;
                smoothParameters.MaxDeviationRadius = 0.5f;
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
                    AddManyItems();
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
                //Bitmap bmp = Properties.Resources.Background;
                //dc.DrawImage(new BitmapImage(new Uri(Properties.Resources.Background));
                //dc.DrawImage(new BitmapImage(new Uri("pack://application:,,,/AssemblyName;component/Resources/Background.png")), new Rect(0.0, 0.0,width,height));
                //String uri = "pack://EngineeringProjectApp:,,,/AssemblyName;component/Resources/Background.png";
                //System.Drawing.Image image = bmp;


                //dc.DrawImage(new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/Background.png", UriKind.Relative)), new Rect(0.0, 0.0, width, height));
                //Uri uri = new Uri(Properties.Resources.Background.ToString(), UriKind.Relative);
                //ImageSource imgSource = new BitmapImage(uri);
                //this.Image.Source = imgSource;
                //dc.DrawImage(imgSource, new Rect(0.0, 0.0, width, height));
                dc.DrawImage(new BitmapImage(new Uri("../../Resources/Background.png", UriKind.Relative)),new Rect(0.0, 0.0, width, height));
                if (skeletons.Length != 0)
                { 
                    Skeleton skel=skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                    if (skel != null)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            var rightHand = skel.Joints[JointType.HandRight];
                            CheckBoundaries(this.SkeletonPointToScreen(rightHand.Position), dc);
                            if (rightHand.TrackingState == JointTrackingState.Tracked || rightHand.TrackingState == JointTrackingState.Inferred)
                            {
                                //Brush drawBrush = this.trackedJointBrush;
                                //dc.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(rightHand.Position), 10.0f, 10.0f);
                                this.DrawTrasmorfedPoint(rightHand);
                                this.ScanItems(rightHand, skel.Joints[JointType.Head], skel.Joints[JointType.HandLeft]);
                            }
                        }
                    }
                }
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, width, height));
            }
        }

        private void AddManyItems() {
            this.itemsArray = new Item[5];
            Random r = new Random();
            for (int i=0;i<this.itemsArray.Length;i++) {
                int randomWidth = r.Next(260, 720);
                int randomHeight = r.Next(40, 550);
                if (i % 2 == 0) {
                    itemsArray[i] = AddItem(randomWidth, randomHeight, new Item(ItemType.Bird));
                }
                else {
                    itemsArray[i] = AddItem(randomWidth, randomHeight, new Item(ItemType.Butterfly));
                }
            }
        }

        private void ScanItems(Joint rightHand, Joint head, Joint leftHand) {
            Point rightHandPoint = SkeletonPointToScreen(rightHand.Position);
            Point leftHandPoint = SkeletonPointToScreen(leftHand.Position);
            Point HeadPoint = SkeletonPointToScreen(head.Position);

            if (leftHandPoint.Y < HeadPoint.Y)
            {
                for (int i = 0; i < this.itemsArray.Length; i++)
                {
                    if (rightHandPoint.X <= (this.itemsArray[i].getX() + 25) && rightHandPoint.X >= (this.itemsArray[i].getX() - 25)
                        && rightHandPoint.Y <= (this.itemsArray[i].getY() + 25) && rightHandPoint.Y >= (this.itemsArray[i].getY() - 25))
                    {
                        MoveItem(rightHandPoint, HeadPoint, leftHandPoint, this.itemsArray[i]);
                        break;
                    }
                }
            }
            else { OrangeDot.Fill = new SolidColorBrush(Colors.OrangeRed);
            }

        }

        private void MoveItem(Point rightHandPoint, Point HeadPoint, Point leftHandPoint, Item item) {

            OrangeDot.Fill = new SolidColorBrush(Colors.BlueViolet);
            Canvas.SetLeft(item.getImage(), rightHandPoint.X);
            Canvas.SetTop(item.getImage(), rightHandPoint.Y);
            item.setX((float)rightHandPoint.X);
            item.setY((float)rightHandPoint.Y);

        }

        private Item AddItem(int x, int y, Item item) {
            BitmapImage bitmapImage=null;
            switch (item.getItemType()) {
                case ItemType.Butterfly: bitmapImage = new BitmapImage(new Uri("butterflyImage.png", UriKind.Relative)); break;
                case ItemType.Bird: bitmapImage = new BitmapImage(new Uri("birdImage.png", UriKind.Relative));  break;
            }
            item.setImage(new Image
            {
                Height = 50,
                Width = 50,
                Source = bitmapImage
            });
            mainCanva.Children.Add(item.getImage());
            Canvas.SetLeft(item.getImage(), x);
            Canvas.SetTop(item.getImage(), y);
            item.setX(x);
            item.setY(y);
            return item;
        }

        private void DrawTrasmorfedPoint(Joint joint)
        {
            Point point = SkeletonPointToScreen(joint.Position);
            if (point.X <= width && point.X>=0)
            {
                Canvas.SetLeft(OrangeDot, point.X);
            }
            if (point.Y <= height  && point.Y>=0)
            {
                Canvas.SetTop(OrangeDot, point.Y);
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
        private void ClippedEdgesUpdates(Skeleton skeleton, DrawingContext drawingContext)
        {
            switch (skeleton.ClippedEdges)
            {
                case FrameEdges.Bottom:
                    drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, height - 10.0f, width, 10.0f));
                    break;
                case FrameEdges.Left:
                    drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, 10.0f, height));
                    break;
                case FrameEdges.Right:
                    drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(width - 10.0f, 0, 10.0f, width));
                    break;
                case FrameEdges.Top:
                    drawingContext.DrawRectangle(
                         Brushes.Red,
                         null,
                         new Rect(0, 0, width, 10.0f));
                    break;
                default:
                    break;
            }
        }
        private void CheckBoundaries(Point jointPoint, DrawingContext drawingContext)
        {
            if (jointPoint.Y>=height-10)
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

            if (jointPoint.X >= width-10)
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
