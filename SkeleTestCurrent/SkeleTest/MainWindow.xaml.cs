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
using System.IO;
using System.Threading;
using Microsoft.Kinect;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SkeleTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Calling kinect sensor
        KinectSensor sensor;

        //const variables
        const int SKELETON_COUNT = 6;
        Skeleton[] allSkeletons = new Skeleton[SKELETON_COUNT];

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //if there are any kinect sensors found sensor will be set to the first one
            if (KinectSensor.KinectSensors.Count > 0)
            {
                sensor = KinectSensor.KinectSensors[0];
            }

            //if the sensor is connected color, depth, and skeleton will be enabled
            if (sensor.Status == KinectStatus.Connected)
            {
                sensor.ColorStream.Enable();
                sensor.DepthStream.Enable();
                sensor.SkeletonStream.Enable();

                //Not for xbox 360, for windows kinect
                //these are used to sense upper body when user is close to sensor
                //sensor.DepthStream.Range = DepthRange.Near;
                //sensor.skeletonStream.EnableTrackingInNearRange = true;
                //sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;


                sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(sensor_AllFramesReady);
                sensor.Start();
            }
        }


        //happens when all frames (color, depth, and skeleton) are ready for use
        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            //throw new NotImplementedException();
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];
                colorFrame.CopyPixelDataTo(pixels);

                //calculates sizing of image
                int stride = colorFrame.Width * 4;

                Vid.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
            }

            Skeleton me = null;
            getSkeleton(e,ref me);

            if(me == null)
            {
                return;
            }

            getCameraPoint(me, e);



        }

        private void getSkeleton(AllFramesReadyEventArgs e, ref Skeleton me)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if(skeletonFrameData == null)
                {
                    return;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                me = (from s in allSkeletons where s.TrackingState == SkeletonTrackingState.Tracked select s).FirstOrDefault();

            }
        }

        private void getCameraPoint(Skeleton me, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null || sensor == null)
                {
                    return;
                }

                DepthImagePoint headDepthPoint = depth.MapFromSkeletonPoint(me.Joints[JointType.Head].Position);
                DepthImagePoint rHandDepthPoint = depth.MapFromSkeletonPoint(me.Joints[JointType.HandRight].Position);
                DepthImagePoint lHandDepthPoint = depth.MapFromSkeletonPoint(me.Joints[JointType.HandLeft].Position);
                DepthImagePoint spineDepthPoint = depth.MapFromSkeletonPoint(me.Joints[JointType.Spine].Position);

                ColorImagePoint headColorPoint = depth.MapToColorImagePoint(headDepthPoint.X, headDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint lHandColorPoint = depth.MapToColorImagePoint(lHandDepthPoint.X, lHandDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint rHandColorPoint = depth.MapToColorImagePoint(rHandDepthPoint.X, rHandDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);
                ColorImagePoint spineColorPoint = depth.MapToColorImagePoint(spineDepthPoint.X, spineDepthPoint.Y, ColorImageFormat.RgbResolution640x480Fps30);

                System.Console.WriteLine("Spine" + headColorPoint.X);
                System.Console.WriteLine("RightHand" + rHandColorPoint.X);
                System.Console.WriteLine("LeftHand" + lHandColorPoint.X);
                System.Console.WriteLine("RH Difference" + (rHandColorPoint.X - headColorPoint.X));
                System.Console.WriteLine("LH Difference" + (headColorPoint.X - lHandColorPoint.X));
                System.Console.WriteLine("RH Difference" + (rHandColorPoint.Y - headColorPoint.Y));


                //make a region for follow
                int stopRegionXShort = spineColorPoint.X - 100;
                int stopRegionYShort = spineColorPoint.Y + 100;
                //make region for not follow
                int stopRegionXMax = spineColorPoint.X + 100;
                int stopRegionYMax = spineColorPoint.Y - 100;

                if (!((rHandColorPoint.X < stopRegionXMax) && (rHandColorPoint.X > stopRegionXShort) && (rHandColorPoint.Y > stopRegionYMax)
                    && (rHandColorPoint.Y < stopRegionYShort) && ((lHandColorPoint.X < stopRegionXMax) && (lHandColorPoint.X > stopRegionXShort)
                    && (lHandColorPoint.Y > stopRegionYMax) && (lHandColorPoint.Y < stopRegionYShort))))
                { 
                //both hands to right
                if ((rHandColorPoint.X > headColorPoint.X) && (lHandColorPoint.X > headColorPoint.X) &&
                (rHandColorPoint.Y > headColorPoint.Y) && (lHandColorPoint.Y > headColorPoint.Y))
                {
                    Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}");

                    Console.Write("Move Right");
                    SendKeys.SendWait("{F3}");

                    Canvas.SetLeft(Fox, ((rHandColorPoint.X - Fox.Width / 2)));
                    Canvas.SetTop(Fox, (rHandColorPoint.Y) - Fox.Width / 2);
                }

                //both hands to left
                else if ((rHandColorPoint.X < headColorPoint.X) && (lHandColorPoint.X < headColorPoint.X) &&
                   (rHandColorPoint.Y > headColorPoint.Y) && (lHandColorPoint.Y > headColorPoint.Y))
                {
                    Canvas.SetLeft(Fox, ((lHandColorPoint.X - Fox.Width / 2)));
                    Canvas.SetTop(Fox, (lHandColorPoint.Y) - Fox.Width / 2);
                    Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}");

                    Console.Write("Move Left");
                    SendKeys.SendWait("{F1}");
                }


                else
                {
                    Canvas.SetLeft(Fox, (headColorPoint.X) - Fox.Width / 2);
                }

                if ((rHandColorPoint.Y < headColorPoint.Y) && (lHandColorPoint.Y < headColorPoint.Y))
                {
                    Canvas.SetLeft(Fox, (headColorPoint.X) - Fox.Width / 2);
                    Canvas.SetTop(Fox, (headColorPoint.Y) - Fox.Width / 2);
                    Console.WriteLine("\nSpeech Recognized: \t{0}\tConfidence:\t{1}");

                    Console.Write("STOP");
                    SendKeys.SendWait("{F5}");
                }

            }
                
                
                //Canvas.SetTop(Fox, (headColorPoint.Y - Fox.Height / 2) - (rHandColorPoint.X - Fox.Width / 2) / 2);
            }
        }

        //ends sensors
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sensor.Stop();
        }

    }
}
