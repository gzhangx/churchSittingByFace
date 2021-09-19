﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfFaceApp
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

        bool videoCaptureThreadRunning = false;
        Thread videoCaptureThread = null;

        VedaFacesDotNet.VedaFaces faceReco;
        private void start_Click(object sender, RoutedEventArgs e)
        {
            if (faceReco == null)
            {
                faceReco = new VedaFacesDotNet.VedaFaces(null);
            }
            if (videoCaptureThread == null && !videoCaptureThreadRunning)
            {
                int started = VedaFacesDotNet.VedaFaceNative.startVideoCapture(0);
                if (started == 0)
                {
                    MessageBox.Show("Video Start failed");
                    return;
                }
                videoCaptureThread = new Thread(videoCaptureRun);
                videoCaptureThread.Start();
                btnStart.Content = "Click To Stop";
            }
            else
            {
                VedaFacesDotNet.VedaFaceNative.stopVideoCapture();
                videoCaptureThread = null;
                btnStart.Content = "Click To Start";
            }
        }

        void videoCaptureRun()
        {
            try
            {
                var img = VedaFacesDotNet.VedaFaceNative.createBgrImg();
                while (videoCaptureThread != null)
                {
                    VedaFacesDotNet.VedaFaceNative.captureVideo(img);

                    //Bitmap bmp = (Bitmap)Bitmap.FromFile("test.png");
                    //img = VedaFacesDotNet.VedaFaces.bmpToImg(bmp);


                    var recoRes = faceReco.ProcessImage(img);
                    var bmp = VedaFacesDotNet.VedaFaces.imgToBmp(img);
                    VedaFacesDotNet.VedaFaces.debugCompDescs(recoRes);
                    Console.WriteLine("done");
                    var outBmp = VedaFacesDotNet.VedaFaces.imgToBmp(img);
                    Console.WriteLine("Got results " + recoRes.Count);
                    using (Graphics g = Graphics.FromImage(outBmp))
                    {
                        foreach (var r in recoRes)
                        {
                            var ff = new VedaFacesDotNet.FaceFeatures(r);
                            var lines = ff.getAll();
                            foreach (var line in lines)
                            {
                                g.DrawLine(Pens.Aqua, new System.Drawing.Point(line.from.x, line.from.y), new System.Drawing.Point(line.to.x, line.to.y));
                            }
                        }
                    }
                    uiInvoke(() =>
                    {
                        imgMain.Source = Convert(outBmp);
                    });
                    Thread.Sleep(1000);
                }
            }
            finally
            {
                videoCaptureThreadRunning = false;
                VedaFacesDotNet.VedaFaceNative.stopVideoCapture();
            }
        }

        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }


        void uiInvoke(Action act)
        {
            Dispatcher.Invoke(act);
        }
    }
}
