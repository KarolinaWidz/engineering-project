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
using System.Media;


namespace EngineeringProjectApp
{
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        const int scaleHeight = 700;
        const int scaleWidth = 940;
        const int height = 720;
        const int width = 960;
        private Item[] itemsArray;
        private int amountOfBirds;
        private int amountOfButterflies;
        private string hand;
        private bool returningFlag;
        private int mistakeCounter;
        public MainWindow(int amountOfBirds, int amountOfButterflies, string hand, bool returningFlag)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            this.amountOfBirds = amountOfBirds;
            this.amountOfButterflies = amountOfButterflies;
            this.hand = hand;
            this.returningFlag = returningFlag;
            this.mistakeCounter = 0;
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
                dc.DrawImage(new BitmapImage(new Uri("../../Resources/backgroundImage.png", UriKind.Relative)),new Rect(0.0, 0.0, width, height));
                if (skeletons.Length != 0)
                { 
                    Skeleton skel=skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                    if (skel != null)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Joint mainHand;
                            Joint otherHand;
                            if (this.hand == "Prawa")
                            {
                                mainHand = skel.Joints[JointType.HandRight];
                                otherHand = skel.Joints[JointType.HandLeft];
                            }
                            else {
                                mainHand = skel.Joints[JointType.HandLeft];
                                otherHand = skel.Joints[JointType.HandRight];
                            }
                            CheckBoundaries(mainHand, dc);
                            if (mainHand.TrackingState == JointTrackingState.Tracked || mainHand.TrackingState == JointTrackingState.Inferred)
                            {
                                this.DrawTrasmorfedPoint(mainHand);
                                this.ScanItems(mainHand, skel.Joints[JointType.Head], otherHand);
                                
                            }
                        }
                    }
                }
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, width, height));
            }
        }

        private void CheckPosition(Item item) {
            if (returningFlag) {
                if ((item.getActualPosition() != Position.OTHER) && (item.getActualPosition() != item.getTargetPosition()))
                {
                    Console.Beep();
                    Canvas.SetLeft(item.getImage(), item.getStartX());
                    Canvas.SetTop(item.getImage(), item.getStartY());
                    item.setActualPosition(Position.OTHER);
                    item.setX(item.getStartX());
                    item.setY(item.getStartY());

                }
            }
            else
            {
                if ((item.getActualPosition() != Position.OTHER) && (item.getActualPosition() != item.getTargetPosition() 
                    && (item.getPreviousPosition() != item.getActualPosition())))
                {
                    Console.Beep();
                    mistakeCounter++;
                }
                item.setPreviousPosition(item.getActualPosition());
            }
           
            if(item.getActualPosition() == item.getTargetPosition())
            {
                item.setCorrectPosition();
                CheckFinallyCondition();
            }
        }

        private bool CheckFinallyCondition() {
            bool resultCondition = true;
            for (int i = 0; i < itemsArray.Length; i++) {
                if (itemsArray[i].getCorrectPosition() == false) {
                    resultCondition = false;
                }
            }
            if (resultCondition == true) {
                ShowFinalScene();
            }
            return resultCondition;
        }

        private void ShowFinalScene() {
            BitmapImage bitmapImage = new BitmapImage(new Uri("balloonsImage.png", UriKind.Relative));
            Image image = new Image{ Width = width, Height = height, Source = bitmapImage };
            Label label = new Label
            {
                Content = "Ukończono poziom: łatwy, liczba pomyłek: " + this.mistakeCounter.ToString(),
                FontSize = 30
                
            };
            
            mainCanva.Children.Add(image);
            mainCanva.Children.Add(label);
            Canvas.SetTop(label, height/5*3);
            Canvas.SetLeft(label, width/5);
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);
            SoundPlayer soundPlayerAction = new SoundPlayer(Properties.Resources.fanfareSound);
            soundPlayerAction.PlaySync();
        }
        private Position FindPosition(Item item) {
            Position previousPosition = item.getActualPosition();
            Position resultPosition;
            if (item.getX() >= 10 && item.getX() <= 205
                         && item.getY() >= 50 && item.getY() <= 360)
            {
                resultPosition = Position.TREE;

            }
            else if (item.getX() >= 690 && item.getX() <= 885
                        && item.getY() >= 215 && item.getY() <= 395)
            {
                resultPosition = Position.SUNFLOWER;

            }
            else resultPosition = Position.OTHER;
            this.CheckPosition(item);
            return resultPosition;
        }


        private void AddManyItems() {
            this.itemsArray = new Item[amountOfButterflies + amountOfBirds];
            Random r = new Random();
            int randomWidth, randomHeight;
            for (int i = 0; i < this.amountOfBirds; i++) {
                randomWidth = r.Next(210, 680);
                randomHeight = r.Next(40, 600);
                itemsArray[i] = AddItem(randomWidth, randomHeight, new Item(ItemType.BIRD, Position.TREE));
            }
            for (int i = this.amountOfBirds; i < itemsArray.Length; i++) {
                randomWidth = r.Next(210, 680);
                randomHeight = r.Next(40, 600);
                itemsArray[i] = AddItem(randomWidth, randomHeight, new Item(ItemType.BUTTERFLY, Position.SUNFLOWER));
            }
           
        }

        private void ScanItems(Joint mainHand, Joint head, Joint otherHand) {

            Joint mainHandPoint = mainHand.ScaleTo(scaleWidth, scaleHeight, 0.25f, 0.25f);
            Joint otherHandPoint = otherHand.ScaleTo(scaleWidth, scaleHeight, 0.25f, 0.25f);
            Joint HeadPoint = head.ScaleTo(scaleWidth, scaleHeight, 0.25f, 0.25f);
            int shift = 25;

            if (otherHandPoint.Position.Y < HeadPoint.Position.Y)
            {
                for (int i = 0; i < this.itemsArray.Length; i++)
                {
                    if (mainHandPoint.Position.X <= (this.itemsArray[i].getX() + shift) && mainHandPoint.Position.X >= (this.itemsArray[i].getX() - shift)
                        && mainHandPoint.Position.Y <= (this.itemsArray[i].getY() + shift) && mainHandPoint.Position.Y >= (this.itemsArray[i].getY() - shift))
                    {
                        MoveItem(mainHandPoint, this.itemsArray[i]);
                        break;
                    }
                }
            }
            else { OrangeDot.Fill = new SolidColorBrush(Colors.OrangeRed);
            }

        }
        private void MoveItem(Joint mainHandJoint, Item item)
        {
            int offset = 50;
     
            OrangeDot.Fill = new SolidColorBrush(Colors.BlueViolet);
            if (mainHandJoint.Position.X <= width - offset)
            {
                Canvas.SetLeft(item.getImage(), mainHandJoint.Position.X);
                item.setX((float)mainHandJoint.Position.X);
            }
            if (mainHandJoint.Position.Y <= height - offset)
            {
                Canvas.SetTop(item.getImage(), mainHandJoint.Position.Y);
                item.setY((float)mainHandJoint.Position.Y);
            }
            item.setActualPosition(this.FindPosition(item));
        }

        private Item AddItem(int x, int y, Item item) {
            BitmapImage bitmapImage=null;
            switch (item.getItemType()) {
                case ItemType.BUTTERFLY: bitmapImage = new BitmapImage(new Uri("butterflyImage.png", UriKind.Relative)); break;
                case ItemType.BIRD: bitmapImage = new BitmapImage(new Uri("birdImage.png", UriKind.Relative));  break;
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
            item.setStartX(x);
            item.setStartY(y);
            return item;
        }

        private void DrawTrasmorfedPoint(Joint joint)
        {
            Joint scaledJoint = joint.ScaleTo(scaleWidth, scaleHeight,0.25f,0.25f);
            if (scaledJoint.Position.X <= width && scaledJoint.Position.X>=0)
            {
                Canvas.SetLeft(OrangeDot, scaledJoint.Position.X);
            }
            if (scaledJoint.Position.Y <= height  && scaledJoint.Position.Y>=0)
            {
                Canvas.SetTop(OrangeDot, scaledJoint.Position.Y);
            }

        }
        
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }
       
        private void CheckBoundaries(Joint joint, DrawingContext drawingContext)
        {
            int offset = 30;
            Joint scaledJoint = joint.ScaleTo(scaleWidth, scaleHeight, 0.25f, 0.25f);
            if (scaledJoint.Position.Y>=height- offset)
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, height - 10.0f, width, 10.0f));
                
            }

            if (scaledJoint.Position.Y <= offset)
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, width, 10.0f));
                
            }

            if (scaledJoint.Position.X <= offset)
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, 10.0f, height));
                
            }

            if (scaledJoint.Position.X >= width- offset)
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(width - 10.0f, 0, 10.0f, width));
                
            }
        }
    }


}
