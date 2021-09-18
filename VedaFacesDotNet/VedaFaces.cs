using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VedaFacesDotNet
{
    public class VedaFaces
    {
        public static void Test1(string bmpFileName)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile("test.png");
            var img = bmpToImg(bmp);
            
            String curDir = Directory.GetCurrentDirectory();
            Console.WriteLine(curDir);
            VedaFaceNative.VedaFacePtr face = VedaFaceNative.netInit(curDir);


            var res = GetRecoResult(face, img);
            debugCompDescs(res);

            Bitmap resBmp = imgToBmp(img);
            resBmp.Save("testout.bmp");
        }

        private VedaFaceNative.VedaFacePtr face;
        public VedaFaces(String configDir)
        {
            if (String.IsNullOrEmpty(configDir))
            {
                configDir = Directory.GetCurrentDirectory();                                
            }
            face = VedaFaceNative.netInit(configDir);
        }
        public List<RecoResult> ProcessImage(VedaFaceNative.Array2dImgPtr img)
        {
            return GetRecoResult(face, img);
        }


        public static VedaFaceNative.Array2dImgPtr bmpToImg(Bitmap bmp)
        {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                VedaFaceNative.Array2dImgPtr img = VedaFaceNative.createBgrImg();
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                VedaFaceNative.populateBgrImg(new VedaFaceNative.ImageInfo
                {
                    width = bmpData.Width,
                    height = bmpData.Height,
                    stride = bmpData.Stride,
                    elementSize = 3,
                    data = bmpData.Scan0,
                }, img);
                return img;
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }            
        }
        public static Bitmap imgToBmp(VedaFaceNative.Array2dImgPtr img)
        {
            var imgMeta = VedaFaceNative.getImageMetaData(img);
            Bitmap bmp = new Bitmap((int)imgMeta.width, (int)imgMeta.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                byte[] buf = new byte[bmpData.Stride * imgMeta.height];
                VedaFaceNative.getImageData(img, (uint)bmpData.Stride, buf);
                IntPtr ptr = bmpData.Scan0;


                System.Runtime.InteropServices.Marshal.Copy(buf, 0, ptr, buf.Length);

                return bmp;
            }
            finally
            {
                bmp.UnlockBits(bmpData);
            }
        }

        public static void debugCompDescs(List<RecoResult> res)
        {
            for (int i = 0; i < res.Count; i++)
            {
                Console.Write(" " + i + ": ");
                for (int j = i + 1; j < res.Count; j++)
                {
                    Console.Write(" " + j + "-> " + GetDescDist(res[i].descriptors, res[j].descriptors));
                }
                Console.WriteLine();
            }
        }

        public static double GetDescDist(float[] a, float[] b)
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

        static List<RecoResult> GetRecoResult(VedaFaceNative.VedaFacePtr face, VedaFaceNative.Array2dImgPtr img)
        {
            List<RecoResult> ress = new List<RecoResult>();

            uint resCnt = VedaFaceNative.ProcessImage(face, img);
            for (int i = 0; i < resCnt; i++)
            {
                RecoResult res = new RecoResult();
                var meta = VedaFaceNative.getResultMeta(face, i);
                res.descriptors = new float[meta.descriptorSize];
                res.points = new VedaFaceNative.DntPoint[meta.pointSize];
                res.rect = meta.rect;

                VedaFaceNative.getResultDescriptors(face, res.descriptors, i);
                VedaFaceNative.getResultDetPoints(face, res.points, i);
                ress.Add(res);
            }
            return ress;
        }
    }
}
