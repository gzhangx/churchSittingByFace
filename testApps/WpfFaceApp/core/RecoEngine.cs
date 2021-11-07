using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VedaFacesDotNet;

namespace WpfFaceApp.core
{
    public class RecoEngine
    {
        public double threshold = 0.5;
        public double tooSimilarThreshold = 0.01;
        public class RecoImageResult : IDisposable
        {
            public Bitmap OutBmp { get; private set; }
            public List<RecoResult> RecoRes { get; private set; }
            public RecoImageResult(Bitmap bmp, List<RecoResult> res)
            {
                OutBmp = bmp;
                RecoRes = res;
            }

            public void Dispose()
            {
                OutBmp.Dispose();
            }
        }
        public class DrawActionParms
        {
            public Graphics g { get; private set; }
            public Bitmap outBmp { get; private set; }
            public RecoResult r;
            public FaceFeatures ff;
            public DrawActionParms(Graphics gr, Bitmap bmp)
            {
                g = gr;
                outBmp = bmp;
            }

        }
        protected VedaFacesDotNet.VedaFaces faceReco;
        public RecoEngine(String configDir = null)
        {
            faceReco = new VedaFacesDotNet.VedaFaces(configDir);
        }

        public static System.Drawing.Rectangle toRect(VedaFaceNative.DntRect r)
        {
            return new System.Drawing.Rectangle(r.l, r.t, r.r - r.l, r.b - r.t);
        }
        public RecoImageResult doOneImage(VedaFaces.FaceImage img, Action<DrawActionParms> actions)
        {
            var recoRes = faceReco.ProcessImage(img);
            //VedaFaces.debugCompDescs(recoRes);
            //Console.WriteLine("done");
            var outBmp = img.toBitmap();
            RecoImageResult retRes = new RecoImageResult(outBmp, recoRes);
            
            if (recoRes.Count > 0 && actions != null)
            {                
                using (Graphics g = Graphics.FromImage(outBmp))
                {
                    DrawActionParms prms = new DrawActionParms(g, outBmp);
                    foreach (var r in recoRes)
                    {
                        var ff = new FaceFeatures(r);
                        prms.r = r;
                        prms.ff = ff;
                        actions(prms);
                        //double diff = existing.faceDesc.diff(r.descriptor);                                               
                        {
                            //var stringSize = g.MeasureString("measureString", objFont);
                            //g.FillRectangle(System.Drawing.Brushes.White, r.rect.l, r.rect.t, stringSize.Width, stringSize.Height);
                            //g.DrawString(found.name, objFont, System.Drawing.Brushes.Black, new PointF(r.rect.l, r.rect.t));
                        }
                    }
                }

            }
            return retRes;    
        }

        public static void debugDrawFeatures(Graphics g, RecoResult r, FaceFeatures ff)
        {
            var lines = ff.getAll();
            foreach (var line in lines)
            {
                g.DrawLine(Pens.Aqua, new System.Drawing.Point(line.from.x, line.from.y), new System.Drawing.Point(line.to.x, line.to.y));
            }

            g.DrawRectangle(Pens.White, toRect(r.rect));
        }
    }
}
