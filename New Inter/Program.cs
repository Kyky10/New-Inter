using System;
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
            var a = lib.Exec(new object[] {30});

        }
    }
}
