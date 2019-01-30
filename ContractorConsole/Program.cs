using ContractorCore;
using System;
using System.Diagnostics;

namespace ContractorConsole
{
    class Program
    {
        static Process mongod;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = @"mongod.exe";
            start.WindowStyle = ProcessWindowStyle.Hidden;

            start.Arguments = "--dbpath .\\";

            mongod = Process.Start(start);

            var t = new Class1();

        }
        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            mongod.Kill();
        }
    }
}
