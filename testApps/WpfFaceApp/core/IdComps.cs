using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VedaFacesDotNet;

namespace WpfFaceApp.core
{
    public class IdComps
    {
        public int GroupMax = 5;
        public class IdGroup
        {
            List<RecoResult> results = new List<RecoResult>();
            //public double 
        }

        public List<IdGroup> groups = new List<IdGroup>();

    }
}
