using System;
using System.Collections.Generic;
using System.Linq;
using New_Inter.Classes;

namespace New_Inter
{
    class Statement
    {
        public List<Object> Branch;

        public string Txt;
        public string Block;
        public Func<object> Exec;
        public Lib Lib;
        public bool Ignone;
        public bool Assign;
        private bool awaitFunction;

        public Statement(string txt, Lib lib, string prevBlock = null)
        {
            Branch = new List<Object>();
            Lib = lib;
            txt = txt.Trim();
            Txt = txt;
            Ignone = false;
            Assign = false;
            awaitFunction = false;
            var randomStr = RandomString(10);

            Block = prevBlock;



            if (GetStr(txt, out var outStr, '{', '}') && txt.StartsWith("{"))
            {
                Ignone = true;

                var statTxt = outStr;
                var stats = new List<Statement>();

                var statsTxt = statTxt.SplitSkip(";");

                foreach (var s in statsTxt)
                {
                    var ss = new Statement(s, lib, prevBlock);
                    stats.Add(ss);
                }
                 
                Branch.AddRange(stats);

                Exec = () => null;

                return;
            }

            if (txt.StartsWith("if"))
            {
                Block = (prevBlock ?? "") + "/" + randomStr;

                var exprP = txt.Substring(2, txt.Length - 2);
                var exprTxt =  GetExpressionPar(exprP);
                var expr = new Expression(exprTxt, Block, this);
                var split = '(' + exprTxt + ')';
                var positionSplit = txt.IndexOf(split, StringComparison.Ordinal);
                var txtSplit = txt.Substring(positionSplit + split.Length, txt.Length - positionSplit - split.Length);
                var ifStat = new Statement(txtSplit, Lib, Block);
                var tree = new List<object>();

                Branch.Add(expr);

                Exec = () =>
                {
                    if (!awaitFunction)
                    {

                        if (!tree.Any())
                        {
                            tree = treeToList(GetTree(ifStat));
                        }
                        var value = expr.GetValue();

                        if (IsTrue(value))
                        {
                            awaitFunction = true;

                            var tempFunction = new Function("temp:", Lib, Block);

                            tempFunction.Tree = tree;

                            Lib.AccessNewFunction(tempFunction, null);

                            return new Return();
                        }

                        return null;
                    }
                    else
                    {
                        awaitFunction = false;

                        var ret = Lib.GetReturn();
                        return ret;
                    }
                };
                return;
            }

            if (txt.StartsWith("while"))
            {
                Block = (prevBlock ?? "") + "/" + randomStr;

                var exprP = txt.Substring(5, txt.Length - 5);
                var exprTxt = GetExpressionPar(exprP);
                var expr = new Expression(exprTxt, Block, this);
                var split = '(' + exprTxt + ')';
                var positionSplit = txt.IndexOf(split, StringComparison.Ordinal);
                var txtSplit = txt.Substring(positionSplit + split.Length, txt.Length - positionSplit - split.Length);
                var whileStat = new Statement(txtSplit, Lib, Block);
                var tree = new List<object>();


                Branch.Add(expr);

                Exec = () =>
                {
                    if (awaitFunction)
                    {
                        awaitFunction = false;

                        var ret = Lib.GetReturn();
                    }

                    if (!tree.Any())
                    {
                        tree = treeToList(GetTree(whileStat));
                    }
                    var value = expr.GetValue();

                    if (IsTrue(value))
                    {
                        awaitFunction = true;

                        var tempFunction = new Function("temp:", Lib, Block) {Tree = tree};


                        Lib.AccessNewFunction(tempFunction, null);

                        return new Return();
                    }

                    return null;
                };
                return;
            }

            if (txt.StartsWith("return"))
            {
                var exprP = txt.Substring(7, txt.Length - 7);
                var expr = new Expression(exprP, Block, this);

                Branch.Add(expr);

                Exec = () =>
                {
                    var value = expr.GetValue();
                    var ret = new Return(value);

                    return ret;
                };
                return;
            }

            if (txt.StartsWith("var"))
            {
                Assign = true;
                var inEx = txt.Substring(4, txt.Length - 4);
                var inSplitEx = inEx.Split(new []{'='}, 2);
                var identifier = inSplitEx[0].Trim();
                var expressionTxt = inSplitEx[1];
                var expression = new Expression(expressionTxt, Block, this);

                Branch.Add(expression);

                Exec = () =>
                {
                    var value = expression.GetValue();


                    if (value is Return r)
                    {
                        return r;
                    }

                    if (value is bool b)
                    {
                        Memory.SetVariable(identifier, Block, b ? 1 : 0);
                        return null;
                    }

                    Memory.SetVariable(identifier, Block, value);

                    return null;
                };
                return;
            }

            if (txt.Length == 0)
            {
                Exec = () => null;
            }

            var exp = new Expression(txt, Block, this);

            Assign = exp.Type == EType.Asg;

            Branch.Add(exp);
            Exec = () => exp.Exec();
        }

        private bool IsTrue(object ob)
        {
            if (ob is BoolClass boolClass)
            {
                return boolClass.Value;
            }
            else
            {
                return ob.GetInt() % 2 == 1;
            }
        }

        private bool GetStr(string str, out string outStr, char caracterStart = '"', char caracterEnd = '"')
        {
            var startBool = false;
            var start = 0;
            var countP = 0;

            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (!startBool)
                {
                    if (c == caracterStart)
                    {
                        startBool = true;
                        start = i;
                        countP++;
                    }
                }
                else
                {
                    if (c == caracterStart)
                    {
                        countP++;
                    }

                    if (c == caracterEnd)
                    {
                        countP--;

                        if (countP == 0)
                        {
                            var end = i;
                            outStr = str.Substring(start + 1, end - start - 1);
                            return true;//
                        }
                    }
                }
            }

            outStr = null;
            return false;
        }

        private string GetExpressionPar(string txt)
        {
            var startBool = false;
            var start = 0;
            var countP = 0;

            for (int i = 0; i < txt.Length; i++)
            {
                var c = txt[i];
                if (!startBool)
                {
                    if (c == '(')
                    {
                        startBool = true;
                        start = i;
                        countP++;
                    }
                }
                else
                {
                    if (c == '(')
                    {
                        countP++;
                    }

                    if (c == ')')
                    {
                        countP--;

                        if (countP == 0)
                        {
                            var end = i;
                            return txt.Substring(start + 1, end - start - 1);
                        }
                    }
                }
            }

            return null;
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

        private List<object> GetTree(Statement statement)
        {
            var tree = new List<object>();
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
                    if (retO.Flag != Flag.Continue)
                    {
                        ret = retO;
                        break;
                    }
                }

                tree.Remove(b);
                i--;
            }

            return ret;
        }

        public override string ToString()
        {
            return Txt;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[StaticRandom.Next(s.Length)]).ToArray());
        }
    }
}
