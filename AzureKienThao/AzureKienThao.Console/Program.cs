using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureKienThao.Background
{
    class Program
    {
        static void Main(string[] args)
        {
            PrepareSeeds seed = new PrepareSeeds();
            seed.Execute();
            seed.PostProcess();
            //Console.Read();
        }
    }
}
