using System;
using System.Collections.Generic;
using static VedaFacesDotNet.VedaFaceNative;

namespace VedaFacesDotNet
{
    public class FaceFeatures
    {
        public class Line
        {
            public DntPoint from { get; private set; }
            public DntPoint to { get; private set; }
            public Line(DntPoint d1, DntPoint d2)
            {
                from = d1;
                to = d2;
            }
        }

        public List<Line> shorts { get; private set; }
        public List<Line> earToEar { get; private set; }
        public List<Line> topOfNose { get; private set; }
        public List<Line> leftEyebrow { get; private set; }
        public List<Line> rightEyebrow { get; private set; }
        public List<Line> bottomNose { get; private set; }
        public List<Line> noseToBottomAbove { get; private set; }
        public List<Line> leftEye { get; private set; }
        public List<Line> rightEye { get; private set; }

        public List<Line> outerLips { get; private set; }
        public List<Line> insideLips { get; private set; }
        public FaceFeatures(RecoResult recoRes)
        {
            var d = recoRes.points;
            if (d.Length == 5)
            {
                shorts = new List<Line>();
                shorts.Add(new Line(d[0], d[1]));
                shorts.Add(new Line(d[1], d[4]));
                shorts.Add(new Line(d[4], d[3]));
                shorts.Add(new Line(d[3], d[2]));
                return;
            }

            earToEar = new List<Line>();
            // Around Chin. Ear to Ear
            for (int i = 1; i <= 16; ++i)
                earToEar.Add(new Line(d[i], d[i - 1]));

            topOfNose = new List<Line>();
            // Line on top of nose
            for (int i = 28; i <= 30; ++i)
                topOfNose.Add(new Line(d[i], d[i - 1]));

            // left eyebrow
            leftEyebrow = new List<Line>();
            for (int i = 18; i <= 21; ++i)
                leftEyebrow.Add(new Line(d[i], d[i - 1]));
            // Right eyebrow
            rightEyebrow = new List<Line>();
            for (int i = 23; i <= 26; ++i)
                rightEyebrow.Add(new Line(d[i], d[i - 1]));

            bottomNose = new List<Line>();
            // Bottom part of the nose
            for (int i = 31; i <= 35; ++i)
                bottomNose.Add(new Line(d[i], d[i - 1]));

            noseToBottomAbove = new List<Line>();
            // Line from the nose to the bottom part above
            noseToBottomAbove.Add(new Line(d[30], d[35]));

            leftEye = new List<Line>();
            // Left eye
            for (int i = 37; i <= 41; ++i)
                leftEye.Add(new Line(d[i], d[i - 1]));
            leftEye.Add(new Line(d[36], d[41]));

            rightEye = new List<Line>();
            // Right eye
            for (int i = 43; i <= 47; ++i)
                rightEye.Add(new Line(d[i], d[i - 1]));
            rightEye.Add(new Line(d[42], d[47]));

            outerLips = new List<Line>();
            // Lips outer part
            for (int i = 49; i <= 59; ++i)
                outerLips.Add(new Line(d[i], d[i - 1]));
            outerLips.Add(new Line(d[48], d[59]));

            insideLips = new List<Line>();
            // Lips inside part
            for (int i = 61; i <= 67; ++i)
                insideLips.Add(new Line(d[i], d[i - 1]));
            insideLips.Add(new Line(d[60], d[67]));
        }     

        public List<Line> getAll()
        {
            List<Line> all = new List<Line>();
            Action<List<Line>> checkAdd = (from) => { if (from != null) all.AddRange(from); };

            checkAdd(shorts);
            checkAdd(earToEar);
            checkAdd(topOfNose);
            checkAdd(leftEyebrow);
            checkAdd(rightEyebrow);
            checkAdd(bottomNose);
            checkAdd(noseToBottomAbove);
            checkAdd(leftEye);
            checkAdd(rightEye);
            checkAdd(outerLips);
            checkAdd(insideLips);

            return all;
        }
        private void CheckAdd(List<Line>all, List<Line> from)
        {
            if (from != null) all.AddRange(from);
        }
    }
}
