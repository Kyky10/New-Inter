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
            var txt = File.ReadAllText(file);
            var lib = new Lib(txt);
            lib.PreCompile(new List<object> {4});

            while (lib.MainReturn is null)
            {
                lib.Step();
            }

            Console.WriteLine(lib.MainReturn.Value.ToString());
            Console.ReadLine();
        }
    }
}
