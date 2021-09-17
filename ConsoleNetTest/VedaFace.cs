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
        [DllImport("VedaFaces.dll")]
        public static extern IntPtr netInit(String configDir);

    }
}
