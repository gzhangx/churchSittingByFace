using System;
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
using VedaFacesDotNet;

namespace WpfFaceApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Drawing.Font objFont;
        public MainWindow()
        {
            InitializeComponent();
            System.Drawing.Text.InstalledFontCollection installedFontCollection = new System.Drawing.Text.InstalledFontCollection();
            var fontFamilies = installedFontCollection.Families;
            objFont = new System.Drawing.Font(fontFamilies.Where(x => x.Name == "Arial").FirstOrDefault(), 10);

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
                int started = VedaFacesDotNet.VedaFaces.startVideoCapture(0);
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
                VedaFacesDotNet.VedaFaces.stopVideoCapture();
                videoCaptureThread = null;
                btnStart.Content = "Click To Start";
            }
        }

        List<RecoInfo> recoInfos = new List<RecoInfo>();
        
        void videoCaptureRun()
        {
            try
            {
                int loop = 0;
                var img = new VedaFacesDotNet.VedaFaces.FaceImage();
                while (videoCaptureThread != null)
                {
                    VedaFacesDotNet.VedaFaces.captureVideo(img);
                    var recoRes = faceReco.ProcessImage(img);
                    //var bmp = VedaFacesDotNet.VedaFaces.imgToBmp(img);
                    VedaFacesDotNet.VedaFaces.debugCompDescs(recoRes);
                    Console.WriteLine("done");
                    var outBmp = img.toBitmap();
                    if (recoRes.Count > 0)
                    {
                        Console.WriteLine("Got results " + recoRes.Count);
                        using (Graphics g = Graphics.FromImage(outBmp))
                        {
                            foreach (var r in recoRes)
                            {
                                var ff = new FaceFeatures(r);
                                var lines = ff.getAll();
                                foreach (var line in lines)
                                {
                                    g.DrawLine(Pens.Aqua, new System.Drawing.Point(line.from.x, line.from.y), new System.Drawing.Point(line.to.x, line.to.y));
                                }

                                g.DrawRectangle(Pens.White, toRect(r.rect));

                                RecoInfo found = null;
                                foreach (var existing in recoInfos)
                                {
                                    double diff = existing.faceDesc.diff(r.descriptor);
                                    if (diff < 0.6)
                                    {
                                        found = existing;
                                        Console.WriteLine("found existing, diff " + diff + " " + found.name);
                                    }
                                }
                                if (found == null)
                                {
                                    RecoInfo rInfo = new RecoInfo();
                                    rInfo.faceDesc = r.descriptor;
                                    rInfo.Id = r.descriptor.getHash();
                                    rInfo.name = rInfo.Id;
                                    rInfo.imageName = "tests\\" + rInfo.Id + ".png";
                                    recoInfos.Add(rInfo);
                                    uiInvoke(() =>
                                    {
                                        var npw = new NewPersonConfirmation();
                                        var pimg = CropImage(outBmp, r.rect);
                                        npw.SetImage(Convert(pimg), (str) =>
                                        {
                                            rInfo.name = str;
                                            pimg.Save("tests\\" + rInfo.Id + "_" + rInfo.name + ".bmp");
                                        });
                                        npw.Show();
                                    });
                                } else
                                {
                                    g.DrawString(found.name, objFont, System.Drawing.Brushes.Black, new PointF(r.rect.l, r.rect.t));
                                }
                            }
                        }
                        //VedaFacesDotNet.VedaFaceNative.deleteBgrImg(img);
                        //var outImg = VedaFacesDotNet.VedaFaces.bmpToImg(outBmp);
                        //outImg.savePng("tests\\test" + loop + ".png");                        
                        Console.WriteLine("Number guys " + recoInfos.Count);
                    }
                    loop++;                   
                    uiInvoke(() =>
                    {
                        imgMain.Source = Convert(outBmp);
                        outBmp.Dispose();
                    });
                    Thread.Sleep(1);
                }
            }
            finally
            {
                videoCaptureThreadRunning = false;
                VedaFacesDotNet.VedaFaces.stopVideoCapture();
            }
        }


        public System.Drawing.Rectangle toRect(VedaFaceNative.DntRect r)
        {
            return new System.Drawing.Rectangle(r.l, r.t, r.r - r.l, r.b - r.t);
        }
        public Bitmap CropImage(Bitmap source, VedaFaceNative.DntRect section)
        {
            return CropImage(source, toRect(section));
        }
        public Bitmap CropImage(Bitmap source, System.Drawing.Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
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
