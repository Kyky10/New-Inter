using System;
using System.Collections.Generic;
using System.Linq;

namespace New_Inter
{
    class Function
    {
        public string Identifier;
        public List<string> Parameters;
        public List<Statement> Branch;
        public Func<object, object> Exec;
        public string Txt;
        public string Block;
        public Lib Lib;
        public List<object> Tree;

        public Function(string txt, Lib lib, string block = null)
        {
            Lib = lib;
            Block = block is null ? RandomString(5) : block + "/" + RandomString(5);
            Branch = new List<Statement>();
            Parameters = new List<string>();
            Tree = null;
            Exec = o => null;

            txt = txt.Trim();
            Txt = txt;

            if (string.IsNullOrWhiteSpace(txt))
            {
                return;
            }

            var idnenStatment = txt.Split(new[]{':'}, 2);
            var idenParam = idnenStatment[0].Split(new[] {' '}, 2);
            Identifier = idenParam[0];
            if (idenParam.Length > 1)
            {
                var parameters = idenParam[1].Trim().Split(',');

                Parameters = new List<string>(parameters);
            }

            var statTxt = idnenStatment[1];
            if (!string.IsNullOrWhiteSpace(statTxt))
            {
                var statament = new Statement(statTxt, Lib, Block);
                Branch.Add(statament);
            }



            Exec = o =>
            {
                var tree = treeToList(GetTree(Branch[0]));

                var arr = (object[]) o;
                for (int i = 0; i < Parameters.Count; i++)
                {
                    Parameters[i] = Parameters[i].Trim();
                    var value = arr[i];
                    Memory.SetVariable(Parameters[i], Block, value);
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
            var tree = new List<object>();

            if (statement is null)
            {
                if (Tree != null)
                {
                    return Tree;
                }
                else if (Branch.Any())
                {
                    statement = Branch[0];
                }
                else
                {
                    return tree;
                }
            }

            foreach (object b in statement.Branch)
            {
                if (b is Statement s)
                {
                    tree.Add(GetTree(s));
                }

                /*if (b is Expression e)
                {
                    tree.Add(e);
                }*/
            }

            if (!statement.Ignone)
            {
                tree.Add(statement);
            }

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
                    o = e.Exec();
                }
                if (b is Statement s)
                {
                    o = s.Exec();
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
                .Select(s => s[StaticRandom.Next(s.Length)]).ToArray());
        }

        public override string ToString()
        {
            return Txt;
        }
    }
}
