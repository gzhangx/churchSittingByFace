using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleNetTest
{
    public class VedaFace
    {

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ImageInfo
        {
            public int width;
            public int height;
            public int stride;
            public int elementSize;
            public IntPtr data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DntRect
        {
            public long t, l, r, b;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DntPoint
        {
            public long x, y;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ResultMeta
        {
            public int descriptorSize;
            public DntRect rect;
            public int pointSize;
        };

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

        [DllImport("VedaFaces.dll")]
        public static extern ResultMeta getResultMeta(VedaFacePtr face, int i);

        [DllImport("VedaFaces.dll")]
        public static extern int getResultDescriptors(VedaFacePtr face, float[] data, int who);

        [DllImport("VedaFaces.dll")]
        public static extern int getResultDetPoints(VedaFacePtr face, DntPoint[] data, int who);
    }

    public class RecoResult
    {
        public VedaFace.DntRect rect;
        public VedaFace.DntPoint[] points;
        public float[] descriptors;        
    }
}
