using ContractorCore;
using System;
using System.Diagnostics;
using System.IO;

namespace ContractorConsole
{
    class Program
    {
        static Process mongod;
        static void Main(string[] args)
        {
            (new FileInfo(".\\save\\")).Directory.Create();
            Console.WriteLine("Hello World!");
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"mongod.exe";
            start.WindowStyle = ProcessWindowStyle.Hidden;
            start.CreateNoWindow = true;

            start.Arguments = "--dbpath .\\save\\";

            mongod = Process.Start(start);
            try
            {
                var t = new Class1();
            }
            finally
            {
                mongod.Kill();
            }
            Console.Read();

        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            mongod.Kill();
        }
    }
}
