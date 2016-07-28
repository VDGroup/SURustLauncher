using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RustLauncher;

namespace LibraryTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            UpdateRequest ur = new UpdateRequest("http://192.168.2.106/update/v.txt", "./v.dat");
            Console.WriteLine(ur.IsUpToDate);
            ur.ProgressChanged += Dat;
            ur.Update();
            Console.ReadKey();
        }

        private static void Dat(object sender, EventArgs e)
        {
            
            //Console.WriteLine(e.Percentage + " " + e.BytesTotal+"/"+e.BytesDone);
        }
    }
}
