using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace New_Inter
{
    class Program
    {
        static void Main(string[] args)
        {
            var preCompileTime = 0;
            var execTime = 0;
            var watch = new Stopwatch();


            Memory.Reset();
            var file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "program.inter";
            if (args.Length > 0)
            {
                file = args[0];
            }
            
            var txt = File.ReadAllText(file);
            var lib = new Lib(txt);
            watch.Start();
            lib.PreCompile(new List<object> {3});
            watch.Stop();
            preCompileTime = (int)watch.ElapsedMilliseconds;

            Console.WriteLine($"Pre-Compile Duration: {preCompileTime}ms");

            watch.Restart();
            while (lib.MainReturn is null)
            {
                lib.Step();
            }
            watch.Stop();
            execTime = (int) watch.ElapsedMilliseconds;
            Console.WriteLine($"Execution Duration: {execTime}ms");

            Console.ReadLine();
        }
    }
}
