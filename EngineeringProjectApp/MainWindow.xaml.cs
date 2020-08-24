using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.IO;
using Coding4Fun.Kinect.Wpf;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using System.Media;
using System.Diagnostics;



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
        private string difficultyLevel;
        private int velocity;
        private Stopwatch watch;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += WindowLoaded;
        }
        public MainWindow(int amountOfBirds, int amountOfButterflies, string hand, bool returningFlag, string difficultyLevel, int velocity)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            this.amountOfBirds = amountOfBirds;
            this.amountOfButterflies = amountOfButterflies;
            this.hand = hand;
            this.returningFlag = returningFlag;
            this.mistakeCounter = 0;
            this.difficultyLevel = difficultyLevel;
            this.velocity = velocity;
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
            drawingGroup = new DrawingGroup();
            imageSource = new DrawingImage(drawingGroup);
            Image.Source = imageSource;

            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    sensor = potentialSensor;
                    break;
                }
            }

            if (null != sensor)
            {
                sensor.SkeletonStream.Enable(smoothParameters);
                sensor.SkeletonFrameReady += SensorSkeletonFrameReady;

                try
                {
                    sensor.Start();
                    AddManyItems();
                    watch = Stopwatch.StartNew();
                }
                catch (IOException)
                {
                    sensor = null;
                }
            }
        }

        protected void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
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

            using (DrawingContext dc = drawingGroup.Open())
            {
                dc.DrawImage(new BitmapImage(new Uri("pack://application:,,,/Resources/backgroundImage.png")), new Rect(0.0, 0.0, width, height));
                if (skeletons.Length != 0)
                { 
                    Skeleton skel=skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                    if (skel != null)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Joint mainHand;
                            Joint otherHand;
                            if (hand == "Prawa")
                            {
                                mainHand = skel.Joints[JointType.HandRight];
                                otherHand = skel.Joints[JointType.HandLeft];
                            }
                            else {
                                mainHand = skel.Joints[JointType.HandLeft];
                                otherHand = skel.Joints[JointType.HandRight];
                            }
                            CheckBoundaries(mainHand, dc);
                            if (difficultyLevel == "Średni") {
                                ManyItemsFly(2);
                            }
                            if (difficultyLevel == "Trudny") {
                                ManyItemsFly(velocity);
                            }


                            if (mainHand.TrackingState == JointTrackingState.Tracked || mainHand.TrackingState == JointTrackingState.Inferred)
                            {
                                DrawTrasmorfedPoint(mainHand);
                                ScanItems(mainHand, skel.Joints[JointType.Head], otherHand);
                                
                            }
                        }
                    }
                }
                drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, width, height));

            }
        }

        private void StartFlying(Item item,int shiftLeft, int shiftTop) {
            if ((item.GetX() + shiftLeft) <= 680 && (item.GetX() + shiftLeft) >= 210
                && (item.GetY() + shiftTop) <= 600 && (item.GetY() + shiftTop) >= 40) {
                Canvas.SetLeft(item.GetImage(), item.GetX() + shiftLeft);
                Canvas.SetTop(item.GetImage(), item.GetY() + shiftTop);
                item.SetX(item.GetX() + shiftLeft);
                item.SetY(item.GetY() + shiftTop);
            }
            
        }

        private void ManyItemsFly(int velocity) {
            Random r = new Random();
            for (int i = 0; i < itemsArray.Length; i++) {
                switch (r.Next(0,4))
                {
                    case 0: StartFlying(itemsArray[i], velocity, 0); break;
                    case 1: StartFlying(itemsArray[i], -velocity, 0); break;
                    case 2: StartFlying(itemsArray[i], 0, velocity); break;
                    case 3: StartFlying(itemsArray[i], 0, -velocity); break;
                }
            }
        }

        private void CheckPosition(Item item) {
            if (returningFlag) {
                if ((item.GetActualPosition() != Position.OTHER) && (item.GetActualPosition() != item.GetTargetPosition()))
                {
                    Console.Beep();
                    Canvas.SetLeft(item.GetImage(), item.GetStartX());
                    Canvas.SetTop(item.GetImage(), item.GetStartY());
                    item.SetActualPosition(Position.OTHER);
                    item.SetX(item.GetStartX());
                    item.SetY(item.GetStartY());

                }
            }
            else
            {
                if ((item.GetActualPosition() != Position.OTHER) && (item.GetActualPosition() != item.GetTargetPosition() 
                    && (item.GetPreviousPosition() != item.GetActualPosition())))
                {
                    Console.Beep();
                    mistakeCounter++;
                }
                item.SetPreviousPosition(item.GetActualPosition());
            }

            if (item.GetActualPosition() == item.GetTargetPosition())
            {
                item.SetCorrectPostionFlag(true);
                CheckFinallyCondition();
            }
            else { item.SetCorrectPostionFlag(false); }
        }

        private bool CheckFinallyCondition() {
            bool resultCondition = true;
            for (int i = 0; i < itemsArray.Length; i++) {
                if (itemsArray[i].GetCorrectPosition() == false) {
                    resultCondition = false;
                }
            }
            if (resultCondition == true) {
                watch.Stop();
                ShowFinalScene();
            }
            return resultCondition;
        }

        private void ShowFinalScene() {
            var elapsedMs = watch.ElapsedMilliseconds/1000;
            
            BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/balloonsImage.png"));
            Image image = new Image{ Width = width, Height = height, Source = bitmapImage };
            Label label = new Label
            {
                Content = "Ukończono poziom: "+difficultyLevel.ToString() +", liczba pomyłek: " + mistakeCounter.ToString()+",",
                FontSize = 30
                
            };

            Label label2 = new Label
            {
                Content = "czas gry: " + elapsedMs.ToString()+" s",
                FontSize = 30

            };
            Button button = new Button
            {
                Content = "Powrót do menu",
                FontSize = 30
            };
            button.Click += BtnClick;

            
            mainCanva.Children.Add(image);
            mainCanva.Children.Add(label);
            mainCanva.Children.Add(label2);
            mainCanva.Children.Add(button);
            Canvas.SetTop(label, height/5*3);
            Canvas.SetLeft(label, width/5);
            Canvas.SetTop(label2, height / 5 * 3+35);
            Canvas.SetLeft(label2, width / 5);
            Canvas.SetLeft(button, width / 3);
            Canvas.SetTop(button, height / 5 * 4);
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);
            SoundPlayer soundPlayerAction = new SoundPlayer(Properties.Resources.fanfareSound);
            soundPlayerAction.PlaySync();
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            this.itemsArray = new Item[0];
            this.mistakeCounter = 0;
            this.Content = new Menu();
            this.Show();
        }

        private Position FindPosition(Item item) {
            Position previousPosition = item.GetActualPosition();
            Position resultPosition;
            if (item.GetX() >= 10 && item.GetX() <= 205
                         && item.GetY() >= 50 && item.GetY() <= 360)
            {
                resultPosition = Position.TREE;

            }
            else if (item.GetX() >= 690 && item.GetX() <= 885
                        && item.GetY() >= 215 && item.GetY() <= 395)
            {
                resultPosition = Position.SUNFLOWER;

            }
            else resultPosition = Position.OTHER;
            CheckPosition(item);
            return resultPosition;
        }


        private void AddManyItems() {
            itemsArray = new Item[amountOfButterflies + amountOfBirds];
            Random r = new Random();
            int randomWidth, randomHeight;
            for (int i = 0; i < amountOfBirds; i++) {
                randomWidth = r.Next(210, 680);
                randomHeight = r.Next(40, 600);
                itemsArray[i] = AddItem(randomWidth, randomHeight, new Item(ItemType.BIRD, Position.TREE));
            }
            for (int i = amountOfBirds; i < itemsArray.Length; i++) {
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
                for (int i = 0; i < itemsArray.Length; i++)
                {
                    if (mainHandPoint.Position.X <= (itemsArray[i].GetX() + shift) && mainHandPoint.Position.X >= (itemsArray[i].GetX() - shift)
                        && mainHandPoint.Position.Y <= (itemsArray[i].GetY() + shift) && mainHandPoint.Position.Y >= (itemsArray[i].GetY() - shift))
                    {
                        MoveItem(mainHandPoint, itemsArray[i]);
                        break;
                    }
                }
            }
            else {
                OrangeDot.Fill = new SolidColorBrush(Colors.OrangeRed);
            }
        }
        private void MoveItem(Joint mainHandJoint, Item item)
        {
            int offset = 50;
            OrangeDot.Fill = new SolidColorBrush(Colors.BlueViolet);
            if (mainHandJoint.Position.X <= width - offset)
            {
                Canvas.SetLeft(item.GetImage(), mainHandJoint.Position.X);
                item.SetX((float)mainHandJoint.Position.X);
            }
            if (mainHandJoint.Position.Y <= height - offset)
            {
                Canvas.SetTop(item.GetImage(), mainHandJoint.Position.Y);
                item.SetY((float)mainHandJoint.Position.Y);
            }
            item.SetActualPosition(FindPosition(item));
        }

        private Item AddItem(int x, int y, Item item) {
            BitmapImage bitmapImage=null;
            switch (item.GetItemType()) {
                case ItemType.BUTTERFLY: bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/butterflyImage.png")); break;
                case ItemType.BIRD: bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/birdImage.png"));  break;
            }
            item.SetImage(new Image
            {
                Height = 50,
                Width = 50,
                Source = bitmapImage
            });
            mainCanva.Children.Add(item.GetImage());
            Canvas.SetLeft(item.GetImage(), x);
            Canvas.SetTop(item.GetImage(), y);
            item.SetX(x);
            item.SetY(y);
            item.SetStartX(x);
            item.SetStartY(y);
            return item;
        }

        private void DrawTrasmorfedPoint(Joint joint)
        {
            Joint scaledJoint = joint.ScaleTo(scaleWidth, scaleHeight,0.25f,0.25f);
            if (scaledJoint.Position.X <= width && scaledJoint.Position.X>=0)
            {
                Canvas.SetLeft(OrangeDot, scaledJoint.Position.X);
            }
            if (scaledJoint.Position.Y <= height && scaledJoint.Position.Y >= 0)
            {
                Canvas.SetTop(OrangeDot, scaledJoint.Position.Y);
            }
        }
        
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != sensor)
            {
                sensor.Stop();
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
