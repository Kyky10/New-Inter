using System;
using System.Collections.Generic;
using System.Linq;

namespace New_Inter
{
    class Statement
    {
        public List<Object> Branch;
        public Func<object> ExecFunc
        {
            get
            {
                if (execValue is null)
                {
                    execValue = Func();
                }
                return () => execValue;
            }
            set => Func = value;
        }

        public string Txt;
        private Random Random;
        public string Block;
        private object execValue;
        public Func<object> Func;
        public Function Function;

        public Statement(string txt, Function func, string prevBlock = null)
        {
            Branch = new List<Object>();
            Random = new Random();
            Function = func;
            txt = txt.Trim();
            Txt = txt;
            var randomStr = RandomString(10);

            Block = prevBlock;



            if (GetStr(txt, out var outStr, '{', '}') && txt.StartsWith("{"))
            {
                var statTxt = outStr;
                var stats = new List<Statement>();

                var par = statTxt.IndexOfAny(new[] { '{', '}' });
                var nextStat = statTxt.IndexOf(';');

                while (nextStat > 0)
                {
                    
                    if ((par > nextStat || par < 0))
                    {
                        var statStr = statTxt.Split(new[] {';'}, 2, StringSplitOptions.RemoveEmptyEntries);
                        var stat = new Statement(statStr[0], Function, Block);
                        stats.Add(stat);

                        nextStat++;

                        statTxt = statTxt.Substring(nextStat, statTxt.Length - nextStat);
                    }
                    else if (nextStat < 0)
                    {
                        var stat = new Statement(statTxt, Function, Block);
                        stats.Add(stat);
                    }
                    else
                    {
                        var str = "";
                        if (GetStr(statTxt, out str, '{', '}'))
                        {
                            par = statTxt.IndexOf(str, StringComparison.Ordinal) + str.Length + 1;
                        }

                        var statStr = statTxt.Substring(0, par);

                        var stat = new Statement(statStr, Function, Block);
                        stats.Add(stat);

                        par++;

                        statTxt = statTxt.Substring(par, statTxt.Length - par);
                    }

                    par = statTxt.IndexOfAny(new[] { '{', '}' });
                    nextStat = statTxt.IndexOf(';');
                }
                 
                Branch.AddRange(stats);

                ExecFunc = () => null;
                /*{
                    stats.ForEach(x => x.ExecFunc());
                    return null;
                };*/
                return;
            }

            if (txt.StartsWith("if"))
            {
                Block = (prevBlock ?? "") + "/" + randomStr;

                var exprP = txt.Substring(2, txt.Length - 2);
                var exprTxt =  GetExpressionPar(exprP);
                var expr = new Expression(exprTxt, Block, this);
                var txtSplit = txt.Split(new[] { '(' + exprTxt + ')'}, StringSplitOptions.None);
                var ifStat = new Statement(txtSplit[1], Function, Block);

                Branch.Add(expr);

                ExecFunc = () =>
                {
                    var value = expr.GetValue();

                    if (IsTrue(value))
                    {
                        ifStat.ExecFunc();
                    }

                    return null;
                };
                return;
            }

            if (txt.StartsWith("while"))
            {
                Block = (prevBlock ?? "") + "/" + randomStr;

                var exprP = txt.Substring(5, txt.Length - 5);
                var exprTxt = GetExpressionPar(exprP);
                var expr = new Expression(exprTxt, Block, this);
                var txtSplit = txt.Split(new[] { '(' + exprTxt + ')' }, StringSplitOptions.None);
                var whileStat = new Statement(txtSplit[1], Function, Block);

                Branch.Add(expr);

                ExecFunc = () =>
                {
                    var value = expr.GetValue();

                    while (IsTrue(value))
                    {
                        var tree = treeToList(GetTree(whileStat));
                        ExecTree(tree);
                        value = expr.GetValue();
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

                ExecFunc = () =>
                {
                    var value = expr.GetValue();
                    var ret = new Return(value);

                    return ret;
                };
                return;
            }

            if (txt.StartsWith("var"))
            {
                var inEx = txt.Substring(4, txt.Length - 4);
                var inSplitEx = inEx.Split(new []{'='}, 2);
                var identifier = inSplitEx[0].Trim();
                var expressionTxt = inSplitEx[1];
                var expression = new Expression(expressionTxt, Block, this);

                Branch.Add(expression);

                ExecFunc = () =>
                {
                    var value = expression.GetValue();

                    if (value is string s)
                    {
                        Memory.SetVariable(identifier, Block, s);
                    }

                    if (value is int i)
                    {
                        Memory.SetVariable(identifier, Block, i);
                    }

                    if (value is bool b)
                    {
                        Memory.SetVariable(identifier, Block, b ? 1 : 0);
                    }

                    return null;
                };
                return;
            }

            if (txt.Length == 0)
            {
                ExecFunc = () => null;
            }

            var exp = new Expression(txt, Block, this);

            Branch.Add(exp);
            ExecFunc = () => exp.ExecFunc();
        }

        private bool IsTrue(object ob)
        {
            if (ob is int i)
            {
                return i % 2 == 1;
            }
            else if (ob is string s)
            {
                var ss = s.ToLower();
                if (bool.TryParse(ss, out var b))
                {
                    return b;
                }
                else if (int.TryParse(s, out i))
                {
                    return i % 2 != 0;
                }
            }
            else if (ob is bool b)
            {
                return b;
            }

            return false;
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

        public override string ToString()
        {
            return Txt;
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }


    class Return
    {
        public object Value;
        public Return(object value = null)
        {
            Value = value;
        }
    }
}
