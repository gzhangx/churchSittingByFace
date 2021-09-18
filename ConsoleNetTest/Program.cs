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

            
            int res = VedaFace.ProcessImage(face, img);
            Console.WriteLine("found " + res);
            
        }
    }
}
