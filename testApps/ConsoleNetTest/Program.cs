using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetTest
{
    class Program
    {
        static void Main(string[] args)
        {
            VedaFacesDotNet.VedaFaces f = new VedaFacesDotNet.VedaFaces(null);
            Bitmap bmp = (Bitmap)Bitmap.FromFile("test.png");
            var img = VedaFacesDotNet.VedaFaces.bmpToImg(bmp);
            var res = f.ProcessImage(img);
            VedaFacesDotNet.VedaFaces.debugCompDescs(res);
            Console.WriteLine("done");
            var outBmp = img.toBitmap();

            using (Graphics g = Graphics.FromImage(outBmp))
            {
                foreach (var r in res)
                {
                    var ff = new VedaFacesDotNet.FaceFeatures(r);
                    var lines = ff.getAll();
                    foreach (var line in lines)
                    {
                        g.DrawLine(Pens.Aqua, new Point(line.from.x, line.from.y), new Point(line.to.x, line.to.y));
                    }
                }
            }

            outBmp.Save("testtestoutput.bmp");
            
        }
        static void Main1(string[] args) 
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile("test.png");
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            VedaFace.Array2dImgPtr img = VedaFace.createBgrImg();
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            VedaFace.populateBgrImg(new VedaFace.ImageInfo
            {
                width = bmpData.Width,
                height = bmpData.Height,
                stride = bmpData.Stride,
                elementSize = 3,
                data = bmpData.Scan0,
            }, img);

            bmp.UnlockBits(bmpData);
            String curDir = Directory.GetCurrentDirectory();
            Console.WriteLine(curDir);


            VedaFace.VedaFacePtr  face = VedaFace.netInit(curDir);


            var res = GetRecoResult(face, img);
            CompDescs(res);

            Bitmap resBmp = imgToBmp(img);
            resBmp.Save("testout.bmp");
        }

        static Bitmap imgToBmp(VedaFace.Array2dImgPtr img)
        {
            var imgMeta = VedaFace.getImageMetaData(img);            
            Bitmap bmp = new Bitmap((int)imgMeta.width, (int)imgMeta.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            byte[] buf = new byte[bmpData.Stride * imgMeta.height];
            VedaFace.getImageData(img, (uint)bmpData.Stride, buf);
            IntPtr ptr = bmpData.Scan0;

            
            System.Runtime.InteropServices.Marshal.Copy(buf, 0, ptr, buf.Length);

            bmp.UnlockBits(bmpData);
            return bmp;
        }

        static void CompDescs(List<RecoResult> res)
        {
            for (int i = 0; i < res.Count; i++)
            {
                Console.Write(" "+i+": " );
                for (int j = i + 1; j < res.Count; j++)
                {
                    Console.Write(" "+j+"-> "+GetDescDist(res[i].descriptors, res[j].descriptors));
                }
                Console.WriteLine();
            }
        }

        static double GetDescDist(float[] a, float[] b)
        {
            float res = 0;
            for (int i = 0; i < a.Length; i++)
            {
                float tmp = a[i] - b[i];
                tmp *= tmp;
                res += tmp;
            }
            return Math.Sqrt(res);
        }

        static List<RecoResult> GetRecoResult(VedaFace.VedaFacePtr face, VedaFace.Array2dImgPtr img)
        {
            List<RecoResult> ress = new List<RecoResult>();

            uint resCnt = VedaFace.ProcessImage(face, img);
            Console.WriteLine("found " + resCnt);
            for (int i = 0; i < resCnt; i++)
            {
                RecoResult res = new RecoResult();
                var meta = VedaFace.getResultMeta(face, i);
                res.descriptors = new float[meta.descriptorSize];
                res.points = new VedaFace.DntPoint[meta.pointSize];
                res.rect = meta.rect;

                VedaFace.getResultDescriptors(face, res.descriptors, i);
                VedaFace.getResultDetPoints(face, res.points, i);
                ress.Add(res);
            }
            return ress;
        }
    }
}
