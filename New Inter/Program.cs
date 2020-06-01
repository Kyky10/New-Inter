using System;
using System.Collections.Generic;
using System.IO;

namespace New_Inter
{
    class Program
    {
        static void Main(string[] args)
        {
            Memory.Reset();
            var file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "program.inter";
            if (args.Length > 0)
            {
                file = args[0];
            }
            
            var txt = File.ReadAllText(file);
            var lib = new Lib(txt);
            lib.PreCompile(new List<object> {3});

            while (lib.MainReturn is null)
            {
                lib.Step();
            }

            Console.ReadLine();
        }
    }
}
