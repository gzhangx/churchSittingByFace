using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
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
        BlockParser blockParser;
        public MainWindow()
        {
            InitializeComponent();
            System.Drawing.Text.InstalledFontCollection installedFontCollection = new System.Drawing.Text.InstalledFontCollection();
            var fontFamilies = installedFontCollection.Families;
            objFont = new System.Drawing.Font(fontFamilies.Where(x => x.Name == "Arial").FirstOrDefault(), 20);
            LoadPersons();            
            blockParser = new BlockParser();

            LoadSeats();
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

        List<CellInfo> seatsInfo = new List<CellInfo>();

        List<String> currentDisplayIds = new List<string>();
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
                                var locked = recoInfos.ToArray();
                                foreach (var existing in locked)
                                {
                                    double diff = existing.faceDesc.diff(r.descriptor);
                                    if (diff < 0.6)
                                    {
                                        found = existing;
                                        Console.WriteLine("found existing, diff " + diff + " " + found.name);
                                    }
                                }
                                if (found == null || 
                                    (seatsInfo.Find(s=>s.occupyedById == found.Id) == null && currentDisplayIds.Find(x=>x == found.Id) == null))
                                {
                                    RecoInfo rInfo = found;
                                    String curId = null;
                                    if (found == null)
                                    {
                                        rInfo = new RecoInfo();
                                        rInfo.faceDesc = r.descriptor;
                                        rInfo.Id = r.descriptor.getHash();
                                        rInfo.name = "";

                                        using (var bmp = CropImage(outBmp, r.rect))
                                        {
                                            SavePerson(rInfo, bmp);
                                        }

                                        lock (recoInfos)
                                        {
                                            recoInfos.Add(rInfo);
                                        }

                                        curId = (rInfo.Id);
                                    } else
                                    {
                                        curId = (found.Id);
                                    }
                                    currentDisplayIds.Add(curId);
                                    uiInvoke(() =>
                                    {
                                        var pg = new WindowSeats();
                                        pg.Show();
                                        pg.Init(blockParser, rInfo, cellInfo =>
                                        {
                                            SavePerson(rInfo, null);
                                            cellInfo.occupyedById = rInfo.Id;
                                            cellInfo.occupyedBy = rInfo.name;
                                            seatsInfo.Add(cellInfo);
                                            currentDisplayIds.Remove(curId);
                                            SaveSeats();
                                        }, () =>
                                        {
                                            if (found == null)
                                            {
                                                lock (recoInfos)
                                                {
                                                    recoInfos.Remove(rInfo);
                                                    currentDisplayIds.Remove(curId);
                                                }
                                            }
                                        }, recoInfos);
                                    });                                    
                                } else
                                
                                {                                   
                                    var stringSize = g.MeasureString("measureString", objFont);
                                    g.FillRectangle(System.Drawing.Brushes.White, r.rect.l, r.rect.t, stringSize.Width, stringSize.Height);
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
                        imgMain.Source = Util.Convert(outBmp);
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


        const String PERSONS_DIR_NAME = @"persons";
        const String PERSONS_INFO_FILE = "info.json";
        void SavePerson(RecoInfo info, Bitmap img)
        {
            string[] paths = { PERSONS_DIR_NAME, info.Id};
            string saveDir = System.IO.Path.Combine(paths);
            Directory.CreateDirectory(saveDir);
            MemoryStream msObj = new MemoryStream();
            new DataContractJsonSerializer(typeof(RecoInfo)).WriteObject(msObj, info);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            String content = sr.ReadToEnd();
            File.WriteAllText(System.IO.Path.Combine(saveDir, PERSONS_INFO_FILE), content);

            info.imageName = System.IO.Path.Combine(saveDir, "image.bmp");
            if (img != null) img.Save(info.imageName);
        }

        void LoadPersons()
        {
            recoInfos.Clear();
            String[] dirs = Directory.GetDirectories(PERSONS_DIR_NAME);
            foreach (var dir in dirs)
            {
                string[] paths = { dir, PERSONS_INFO_FILE };                
                String path = System.IO.Path.Combine(paths);
                Console.WriteLine("Loading " + path);
                string jsonStr = File.ReadAllText(path);
                var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonStr));
                RecoInfo info = (RecoInfo)new DataContractJsonSerializer(typeof(RecoInfo)).ReadObject(stream);
                recoInfos.Add(info);
            }
        }

        const String SEATS_DIR_NAME = @"seats";
        String YYYYMMDD = DateTime.Now.ToString("yyyy-MM-dd");
        const String SEATS_INFO_FILE = "seats.json";
        void SaveSeats()
        {            
            string[] paths = { SEATS_DIR_NAME, YYYYMMDD };
            string saveDir = System.IO.Path.Combine(paths);
            Directory.CreateDirectory(saveDir);
            MemoryStream msObj = new MemoryStream();
            new DataContractJsonSerializer(typeof(List<CellInfo>)).WriteObject(msObj, seatsInfo);
            msObj.Position = 0;
            StreamReader sr = new StreamReader(msObj);
            String content = sr.ReadToEnd();
            File.WriteAllText(System.IO.Path.Combine(saveDir, SEATS_INFO_FILE), content);
        }

        void LoadSeats()
        {
            try
            {
                seatsInfo.Clear();
                string[] paths = { SEATS_DIR_NAME, YYYYMMDD, SEATS_INFO_FILE };
                string saveDir = System.IO.Path.Combine(paths);

                string jsonStr = File.ReadAllText(saveDir);
                var stream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonStr));
                seatsInfo = (List<CellInfo>)new DataContractJsonSerializer(typeof(List<CellInfo>)).ReadObject(stream);

                foreach (var seat in seatsInfo)
                {
                    var cell = blockParser.blocks[seat.block].rows[seat.y][seat.x];
                    cell.occupyedById = seat.occupyedById;
                    cell.occupyedBy = seat.occupyedBy;
                }
            } catch(Exception exc)
            {
                Console.WriteLine("Warning " + exc.Message);
                Console.WriteLine(exc);
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

        


        void uiInvoke(Action act)
        {
            Dispatcher.Invoke(act);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VedaFacesDotNet.VedaFaces.stopVideoCapture();            
            videoCaptureThread = null;
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            var pg = new WindowSeats();
            pg.Show();
            pg.Init(blockParser, null, null, ()=> { }, recoInfos);
        }
    }
}
