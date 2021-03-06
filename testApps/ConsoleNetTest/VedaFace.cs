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
        public struct ImageMetaInfo
        {
            public uint width;
            public uint height;
            public uint stride;
            public uint elementSize;
        };

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
            public int t, l, r, b;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct DntPoint
        {
            public int x, y;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public struct ResultMeta
        {
            public int descriptorSize;            
            public int pointSize;
            public DntRect rect;
        };

        public struct Array2dImgPtr { public IntPtr addr; }

        public struct VedaFacePtr { public IntPtr addr; }

        [DllImport("VedaFaces.dll")]
        public static extern VedaFacePtr netInit(String configDir);

        [DllImport("VedaFaces.dll")]
        public static extern uint ProcessImage(VedaFacePtr vedaFace, Array2dImgPtr info);

        [DllImport("VedaFaces.dll")]
        public static extern Array2dImgPtr createBgrImg();

        [DllImport("VedaFaces.dll")]
        public static extern void deleteBgrImg(Array2dImgPtr img);

        [DllImport("VedaFaces.dll")]
        public static extern void populateBgrImg(ImageInfo from, Array2dImgPtr src);      

        [DllImport("VedaFaces.dll")]
        public static extern ResultMeta getResultMeta(VedaFacePtr face, int i);

        [DllImport("VedaFaces.dll")]
        public static extern int getResultDescriptors(VedaFacePtr face, [Out] float[] data, int who);

        [DllImport("VedaFaces.dll")]
        public static extern int getResultDetPoints(VedaFacePtr face, [Out] DntPoint[] data, int who);

        [DllImport("VedaFaces.dll")]
        public static extern ImageMetaInfo getImageMetaData(Array2dImgPtr img);

        [DllImport("VedaFaces.dll")]
        public static extern void getImageData(Array2dImgPtr img, uint stride, [Out] byte[] buffer);
    }

    public class RecoResult
    {
        public VedaFace.DntRect rect;
        public VedaFace.DntPoint[] points;
        public float[] descriptors;        
    }
}
