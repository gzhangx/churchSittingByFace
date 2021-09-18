using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetTest
{
    class VedaFace
    {
        
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct ImageInfo
        {
            public int width;
            public int height;
            public int stride;
            public int elementSize;
            public IntPtr data;
        }

        public struct Array2dImgPtr { private IntPtr addr; }

        public struct VedaFacePtr { private IntPtr addr; }

        [DllImport("VedaFaces.dll")]
        public static extern VedaFacePtr netInit(String configDir);

        [DllImport("VedaFaces.dll")]
        public static extern int ProcessImage(VedaFacePtr vedaFace, Array2dImgPtr info);

        [DllImport("VedaFaces.dll")]
        public static extern Array2dImgPtr createBgrImg();

        [DllImport("VedaFaces.dll")]
        public static extern void deleteBgrImg(Array2dImgPtr img);

        [DllImport("VedaFaces.dll")]
        public static extern void populateBgrImg(ImageInfo from, Array2dImgPtr src);

        [DllImport("VedaFaces.dll")]
        public static extern int ProcessImage(VedaFacePtr face, IntPtr img);
    }
}
