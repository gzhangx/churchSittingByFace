using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace VedaFacesDotNet
{
    public class VedaFaces : IDisposable
    {
        public class FaceImage : IDisposable
        {
            internal VedaFaceNativeInternal.Array2dImgPtr img;

            public FaceImage()
            {
                img = VedaFaceNativeInternal.createBgrImg();
            }
            public void Dispose()
            {
                if (img.addr != IntPtr.Zero) VedaFaceNativeInternal.deleteBgrImg(img);
                img.addr = IntPtr.Zero;
            }
            ~FaceImage()
            {
                Dispose();
            }

            public Bitmap toBitmap()
            {             
                var imgMeta = VedaFaceNativeInternal.getImageMetaData(img);
                Bitmap bmp = new Bitmap((int)imgMeta.width, (int)imgMeta.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                try
                {
                    byte[] buf = new byte[bmpData.Stride * imgMeta.height];
                    VedaFaceNativeInternal.getImageData(img, (uint)bmpData.Stride, buf);
                    IntPtr ptr = bmpData.Scan0;


                    System.Runtime.InteropServices.Marshal.Copy(buf, 0, ptr, buf.Length);

                    return bmp;
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }
            }

            public void savePng(String name)
            {
                VedaFaceNativeInternal.savePng(img, name);
            }

            public void loadImage(String name)
            {
                VedaFaceNativeInternal.loadImage(img, name);
            }
        }

        private VedaFaceNativeInternal.VedaFacePtr face;
        public VedaFaces(String configDir)
        {
            face.addr = IntPtr.Zero;
            if (String.IsNullOrEmpty(configDir))
            {
                configDir = Directory.GetCurrentDirectory();                                
            }
            face = VedaFaceNativeInternal.netInit(configDir);
        }
        public void Dispose()
        {
            if (face.addr != IntPtr.Zero) VedaFaceNativeInternal.netDestroy(face);
            face.addr = IntPtr.Zero;
        }
        ~VedaFaces()
        {
            Dispose();
        }
        public List<RecoResult> ProcessImage(FaceImage img)
        {
            List<RecoResult> ress = new List<RecoResult>();

            uint resCnt = VedaFaceNativeInternal.ProcessImage(face, img.img);
            for (int i = 0; i < resCnt; i++)
            {
                RecoResult res = new RecoResult();
                var meta = VedaFaceNativeInternal.getResultMeta(face, i);
                res.descriptor = new FaceDescriptor { descriptors = new float[meta.descriptorSize] };
                res.points = new VedaFaceNative.DntPoint[meta.pointSize];
                res.rect = meta.rect;

                VedaFaceNativeInternal.getResultDescriptors(face, res.descriptor.descriptors, i);
                VedaFaceNativeInternal.getResultDetPoints(face, res.points, i);
                ress.Add(res);
            }
            return ress;
        }


        public static FaceImage bmpToImg(Bitmap bmp)
        {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                var img = new FaceImage();
                IntPtr ptr = bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                // Copy the RGB values into the array.
                System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

                VedaFaceNativeInternal.populateBgrImg(new VedaFaceNative.ImageInfo
                {
                    width = bmpData.Width,
                    height = bmpData.Height,
                    stride = bmpData.Stride,
                    elementSize = 3,
                    data = bmpData.Scan0,
                }, img.img);
                return img;
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
                    Console.Write(" " + j + "-> " + GetDescDist(res[i].descriptor, res[j].descriptor));
                }
                Console.WriteLine();
            }
        }

        public static double GetDescDist(FaceDescriptor fa, FaceDescriptor fb)
        {
            float[] a = fa.descriptors;
            float[] b = fb.descriptors;
            float res = 0;
            for (int i = 0; i < a.Length; i++)
            {
                float tmp = a[i] - b[i];
                tmp *= tmp;
                res += tmp;
            }
            return Math.Sqrt(res);
        }
        


        public static int startVideoCapture(int id)
        {
            return VedaFaceNativeInternal.startVideoCapture(id);
        }

        public static int stopVideoCapture()
        {
            return VedaFaceNativeInternal.stopVideoCapture();
        }

        public static void captureVideo(FaceImage img)
        {
            VedaFaceNativeInternal.captureVideo(img.img);
        }
    }
}
