using System;
using System.Collections.Generic;
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
            String curDir = Directory.GetCurrentDirectory();
            Console.WriteLine(curDir);
            VedaFace.netInit(curDir);
        }
    }
}
