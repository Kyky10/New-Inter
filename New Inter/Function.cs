using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    class Function
    {
        public string Indentifier;
        public List<string> Parameters;
        private Random Random;
        public List<Statement> Branch;
        public Func<object, object> ExecFunc;
        public string Txt;
        public string Block;
        public Lib Lib;

        public Function(string txt, Lib lib)
        {
            Random = new Random();
            Lib = lib;
            Block = RandomString(5);
            Branch = new List<Statement>();
            Parameters = new List<string>();
           
            txt = txt.Trim();
            Txt = txt;

            var idnenStatment = txt.Split(new[]{':'}, 2);
            var idenParam = idnenStatment[0].Split(new[] {' '}, 2);
            Indentifier = idenParam[0];
            if (idenParam.Length > 1)
            {
                var parameters = idenParam[1].Trim().Split(',');

                Parameters = new List<string>(parameters);
            }

            var statTxt = idnenStatment[1];
            var statament = new Statement(statTxt, Block);
            Branch.Add(statament);

            

            ExecFunc = o =>
            {
                var tree = treeToList(GetTree(statament));

                var arr = (object[]) o;
                for (int i = 0; i < Parameters.Count; i++)
                {
                    Parameters[i] = Parameters[i].Trim();
                    var value = arr[i];
                    if (value is string s)
                    {
                        Memory.SetVariable(Parameters[i], Block, s);
                    }
                    if (value is int ii)
                    {
                        Memory.SetVariable(Parameters[i], Block, ii);
                    }
                }

                var ret = ExecTree(tree);
                return (ret as Return)?.Value;
            };
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

        public List<object> GetTree(Statement statement = null)
        {
            if (statement is null)
            {
                statement = Branch[0];
            }

            var tree = new List<object>();
            foreach (object b in statement.Branch)
            {
                if (b is Statement s)
                {
                    tree.Add(GetTree(s));
                }

                if (b is Expression e)
                {
                    tree.Add(e);
                }
            }
            tree.Add(statement);

            return tree;
        }

        private object ExecTree(List<object> tree)
        {
            Return ret = null;

            for (int i = 0; i < tree.Count; i++)
            {
                var o = new object();
                var b = tree[i];
                if (b is Expression e)
                {
                    o = e.ExecFunc();
                }
                if (b is Statement s)
                {
                    o = s.ExecFunc();
                }

                if (b is List<object> lo)
                {
                    o = ExecTree(lo);
                }

                if (o is Return retO)
                {
                    ret = retO;
                    break;
                }
            }

            return ret;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public override string ToString()
        {
            return Txt;
        }
    }
}
