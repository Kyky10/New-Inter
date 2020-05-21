using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    class Lib
    {
        public List<Function> Functions = new List<Function>();
        public string Txt;
        private List<object> line;
        public List<(string function, int i)> FunctionsExec;

        public Lib(string txt)
        {
            FunctionsExec = new List<(string function, int i)>();
            Txt = txt;
            var functionsTxt = txt.Split(new []{"::"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var functionTxt in functionsTxt)
            {
                var function = new Function(functionTxt, this);
                Functions.Add(function);
            }

            Memory.Functions.AddRange(Functions);
        }

        public int Exec(object[] parameters)
        {
            var main = Functions.Find(x => x.Indentifier.ToLower() == "main");
            var ret = main.ExecFunc(parameters);
            if (ret is null)
            {
                return 0;
            }

            if (ret is int i)
            {
                return i;
            }

            return 0;
        }

        public void PreCompile()
        {
            var main = Functions.Find(x => x.Indentifier.ToLower() == "main");
            line = treeToList(main.GetTree());
        }

        public void Step()
        {

        }

        public void AccessNewFunction(string function)
        {
            FunctionsExec.Add((function, 0));
        }

        private List<object> treeToList(List<object> tree)
        {
            var line = new List<object>();
            foreach (object t in tree)
            {
                if (t is List<object> list)
                {
                    line.AddRange(treeToList(list));
                    continue;
                }
                line.Add(t);
            }

            return line;
        }


        public override string ToString()
        {
            return Txt;
        }
    }
}
