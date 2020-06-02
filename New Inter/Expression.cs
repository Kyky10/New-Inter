using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace New_Inter
{
    class Expression
    {
        public string Txt;
        public ExpType type;
        public PreType preType;
        public Func<object> Func;
        public string Block;
        public Statement Statement;
        public object CallFunctionReturn;
        public bool AwaitReturn;
        public Lib Lib;

        private object execValue;

        private readonly string[][] Tokens = {
            new[]{"=", "+", "-", "+=", "-=", "||", "&&", "==", "!=", ">", "<", ">=", "<="}, //binary 0
            new[]{"&", "*", "-", "!", "++", "--"}, //unitary 1
            new[]{ "(", ")", "[", "]", "{", "}", ";"}, //parantes 2
            new[]{ "?"} //tenary 3
            };


        public Expression(string txt, string block, Statement statement)
        {
            txt = txt.Trim();
            Txt = txt;
            Block = block;
            Statement = statement;
            CallFunctionReturn = null;
            AwaitReturn = false;
            Lib = Statement.Function.Lib;

            var binaryOperators = Tokens[0]
                .OrderByDescending(x => x.Length)
                .ToArray();
            var unitaryOperators = Tokens[1]
                .OrderByDescending(x => x.Length)
                .ToArray();

            if (int.TryParse(txt, out _))
            {
                type = ExpType.Literar;
                preType = PreType.Int;

                var p = txt;

                Func = () => int.Parse(p);
                return;
            }

            if (GetStr(txt, out var Qtxt))
            {
                if (Qtxt.Length == txt.Length - 2)
                {
                    type = ExpType.Literar;
                    preType = PreType.Str;

                    Func = () => Qtxt;
                    return;
                }
            }

            var l = txt[0] == '(';
            var f = txt[txt.Length - 1] == ')';

            if (l && f)
            {

                txt = txt.Substring(1, txt.Length - 2);

                type = ExpType.Literar;
                preType = PreType.Par;

                var exp = new Expression(txt, Block ,Statement);

                Func = exp.Func;
                return;
            }

            foreach (var @operator in unitaryOperators)
            {
                if (txt.StartsWith(@operator))
                {
                    type = ExpType.Unitary;
                    preType = PreType.None;


                    var len = @operator.Length;
                    var exprTxt = txt.Substring(len, txt.Length - len);
                    var expr = new Expression(exprTxt, Block ,Statement);

                    if (@operator == "&")
                    {
                        Func = () => expr.GetValue();
                        return;
                    }
                    if (@operator == "*")
                    {
                        Func = () => expr.GetValue();
                        return;
                    }
                    if (@operator == "-")
                    {
                        Func = () =>
                        {
                            var value = expr.GetValue();

                            if (value is int i)
                            {
                                value = -i;
                            }

                            if (value is string s)
                            {
                                if (s[0] == '-')
                                {
                                    value = s.Substring(1, s.Length - 1);
                                }
                                else
                                {
                                    value = "-" + s;
                                }
                            }

                            if (expr.preType == PreType.Ind)
                            {
                                var variable = expr.Func().ToString();
                                var variable1 = Memory.GetVariable(variable, Block);
                                if (value is string ss)
                                {
                                    variable1.SetValue(ss);
                                }

                                if (value is int ii)
                                {
                                    variable1.SetValue(ii);
                                }
                            }

                            return value;
                        };
                        return;
                    }
                    if (@operator == "!")
                    {
                        Func = () =>
                        {
                            var value = expr.GetValue();

                            if (value is int i)
                            {
                                value = i ^ int.MaxValue;
                            }

                            if (value is string s)
                            {
                                if (char.IsLower(s, 0))
                                {
                                    value = s.ToUpper();
                                }
                                else
                                {
                                    value = s.ToLower();
                                }
                            }

                            if (expr.preType == PreType.Ind)
                            {
                                var variable = expr.Func().ToString();
                                var variable1 = Memory.GetVariable(variable, Block);
                                if (value is string ss)
                                {
                                    variable1.SetValue(ss);
                                }

                                if (value is int ii)
                                {
                                    variable1.SetValue(ii);
                                }
                            }

                            return value;
                        };
                        return;
                    }
                    if (@operator == "++")
                    {
                        Func = () =>
                        {
                            var value = expr.GetValue();

                            if (value is int i)
                            {
                                value = i + 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToUpper();
                            }

                            if (expr.preType == PreType.Ind)
                            {
                                var variable = expr.Func().ToString();
                                var variable1 = Memory.GetVariable(variable, Block);
                                if (value is string ss)
                                {
                                    variable1.SetValue(ss);
                                }

                                if (value is int ii)
                                {
                                    variable1.SetValue(ii);
                                }
                            }

                            return value;
                        };
                        return;
                    }
                    if (@operator == "--")
                    {
                        Func = () =>
                        {
                            var value = expr.GetValue();

                            if (value is int i)
                            {
                                value = i - 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToLower();
                            }

                            if (expr.preType == PreType.Ind)
                            {
                                var variable = expr.Func().ToString();
                                var variable1 = Memory.GetVariable(variable, Block);
                                if (value is string ss)
                                {
                                    variable1.SetValue(ss);
                                }

                                if (value is int ii)
                                {
                                    variable1.SetValue(ii);
                                }
                            }

                            return value;
                        };
                        return;
                    }

                    return;
                }
                else if (txt.EndsWith(@operator))
                {
                    type = ExpType.Unitary;
                    preType = PreType.None;


                    var len = @operator.Length;
                    var exprTxt = txt.Substring(0, txt.Length - len);
                    var expr = new Expression(exprTxt, Block, Statement);


                    if (@operator == "++")
                    {
                        Func = () =>
                        {
                            var value = expr.GetValue();

                            if (value is int i)
                            {
                                value = i + 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToUpper();
                            }

                            if (expr.preType == PreType.Ind)
                            {
                                var variable = expr.Func().ToString();
                                var variable1 = Memory.GetVariable(variable, Block);
                                if (value is string ss)
                                {
                                    variable1.SetValue(ss);
                                }

                                if (value is int ii)
                                {
                                    variable1.SetValue(ii);
                                }
                            }

                            return value;
                        };
                        return;
                    }
                    if (@operator == "--")
                    {
                        Func = () =>
                        {
                            var value = expr.GetValue();

                            if (value is int i)
                            {
                                value = i - 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToLower();
                            }

                            if (expr.preType == PreType.Ind)
                            {
                                var variable = expr.Func().ToString();
                                var variable1 = Memory.GetVariable(variable, Block);
                                if (value is string ss)
                                {
                                    variable1.SetValue(ss);
                                }

                                if (value is int ii)
                                {
                                    variable1.SetValue(ii);
                                }
                            }

                            return value;
                        };
                        return;
                    }
                }
            }

            var ops = new List<(int i, int l, string o)>();
            foreach (var @operator in binaryOperators)
            {
                for (int i = txt.IndexOf(@operator, StringComparison.Ordinal); i > -1; i = txt.IndexOf(@operator, i + 1, StringComparison.Ordinal))
                {
                    var toAdd = (i, @operator.Length, @operator);
                    var Add = true;

                    foreach (var op in ops)
                    {
                        var overlap = toAdd.i < op.i + op.l && op.i < toAdd.i + toAdd.Length;
                        if (overlap)
                        {
                            Add = false;
                            break;
                        }
                    }

                    if (Add)
                    {
                        ops.Add(toAdd);
                    }
                }
            }

            ops = ops.OrderBy(x => x.i).ToList();
            var exprsStr = txt.Split(binaryOperators, StringSplitOptions.None);

            if (exprsStr.Length > 1)
            {
                var op = txt.Substring(ops[0].i, ops[0].l);

                var p = txt.IndexOf('(');
                if (p > ops[0].i || p == - 1)
                {
                    var exps1 = txt.Substring(0, ops[0].i);
                    var exps2 = txt.Substring(ops[0].i + 1 + ops[0].l);

                    var exp1 = new Expression(exps1, Block, Statement);
                    var exp2 = new Expression(exps2, Block, Statement);

                    type = ExpType.Binary;
                    preType = PreType.None;

                    GetOperation(op, exp1, exp2);
                    return;
                }
            }


            var identifier = GetIdentifier(txt);

            if (GetStr(identifier, out var paranteses, '(', ')'))
            {
                var indexOfParant = identifier.IndexOf('(' + paranteses + ')', StringComparison.Ordinal);
                var noParanteses = identifier.Substring(0, indexOfParant).Trim();
                var parametersExpresions = new List<Expression>();

                if (!string.IsNullOrWhiteSpace(paranteses))
                {
                    var parametersTxt = paranteses.Split(',');

                    foreach (var parantese in parametersTxt)
                    {
                        var paranteseTrim = parantese.Trim();

                        var expresion = new Expression(paranteseTrim, Block, Statement);
                        parametersExpresions.Add(expresion);
                    }
                }

                Func = () =>
                {
                    if (!AwaitReturn)
                    {
                        AwaitReturn = true;

                        var parameters = new List<object>();
                        foreach (var expresion in parametersExpresions)
                        {
                            parameters.Add(expresion.GetValue());
                        }

                        Lib.AccessNewFunction(noParanteses, parameters);

                        return new Return(null, Flag.Function);
                    }
                    else
                    {
                        CallFunctionReturn = Lib.ReturnFunc.RetObj;
                        AwaitReturn = false;

                        if (CallFunctionReturn is null)
                        {
                            return new Return();
                        }
                        else if (CallFunctionReturn is Return r)
                        {
                            return r.Value;
                        }
                        else
                        {
                            return CallFunctionReturn;
                        }
                    }
                };

                return;
            }


            type = ExpType.Literar;
            preType = PreType.Ind;
            Func = () => GetIdentifier(txt);
        }


        private bool GetStr(string str, out string outStr, char caracterStart = '"', char caracterEnd = '"')
        {
            if (caracterStart == caracterEnd)
            {
                var c = str.Count(x => x == caracterStart);
                if (c > 2)
                {
                    outStr = null;
                    return false;
                }
                else
                {
                    var startIndex = str.IndexOf(caracterStart);
                    var endIndex = str.LastIndexOf(caracterEnd);

                    if (startIndex == 0 && !(startIndex < 0 || endIndex < 0))
                    {
                        var stri = str.Substring(startIndex + 1, endIndex - startIndex - 1);

                        outStr = stri;
                        return true;
                    }

                    outStr = null;
                    return false;
                }
            }

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
                            return true;
                        }
                    }
                }
            }

            outStr = null;
            return false;
        }

        private string GetIdentifier(string str)
        {
            var strT = str.TrimStart();
            var end = strT.Length;

            return strT.Substring(0, end);
        }

        private void GetOperation(string op, Expression exp1, Expression exp2)
        {
            {
                if (op == "=")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                if (value2 is int iii)
                                {
                                    var var = iii;
                                    variable1.SetValue(var);
                                    return var;
                                }
                                else
                                {
                                    var var = value2.ToString();
                                    variable1.SetValue(var);
                                    return var;
                                }
                            }
                            else if (exec2 is int iii)
                            {
                                var var = iii;
                                variable1.SetValue(var);
                                return var;
                            }
                            else if (exec2 is string sss)
                            {
                                var var = sss;
                                variable1.SetValue(var);
                                return var;
                            }

                            return null;
                        }


                        return null;
                    };
                    return;
                }
                if (op == ">")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = GetInt(variable1.GetValue());

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = GetInt(variable2.GetValue());

                                var var = value > value2;
                                return var;
                            }
                            else
                            {
                                var var = value > GetInt(exec2);
                                return var;
                            }
                          
                        }
                        else
                        {
                            var value = GetInt(exec1);
                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                var var = value > GetInt(value2);
                                return var;
                            }
                            else
                            {
                                var var = value > GetInt(exec2);
                                return var;
                            }
                        }
                    };
                    return;
                }
                if (op == "<")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = GetInt(variable1.GetValue());

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = GetInt(variable2.GetValue());

                                var var = value < value2;
                                return var;
                            }
                            else
                            {
                                var var = value < GetInt(exec2);
                                return var;
                            }

                        }
                        else
                        {
                            var value = GetInt(exec1);
                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                var var = value < GetInt(value2);
                                return var;
                            }
                            else
                            {
                                var var = value < GetInt(exec2);
                                return var;
                            }
                        }
                    };
                    return;
                }
                if (op == "<=")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = GetInt(variable1.GetValue());

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = GetInt(variable2.GetValue());

                                var var = value <= value2;
                                return var;
                            }
                            else
                            {
                                var var = value <= GetInt(exec2);
                                return var;
                            }

                        }
                        else
                        {
                            var value = GetInt(exec1);
                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                var var = value <= GetInt(value2);
                                return var;
                            }
                            else
                            {
                                var var = value <= GetInt(exec2);
                                return var;
                            }
                        }
                    };
                    return;
                }
                if (op == ">=")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = GetInt(variable1.GetValue());

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = GetInt(variable2.GetValue());

                                var var = value >= value2;
                                return var;
                            }
                            else
                            {
                                var var = value >= GetInt(exec2);
                                return var;
                            }

                        }
                        else
                        {
                            var value = GetInt(exec1);
                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                var var = value >= GetInt(value2);
                                return var;
                            }
                            else
                            {
                                var var = value >= GetInt(exec2);
                                return var;
                            }
                        }
                    };
                    return;
                }
                if (op == "+")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable1.GetValue();
                            if (value is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss + value2;
                                    return var;
                                }
                                else
                                {
                                    var var = ss + exec2;
                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)value;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii + iii;
                                        return var;
                                    }
                                    else
                                    {
                                        var var = ii + value2.ToString();
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii + iii;
                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii + sss;
                                    return var;
                                }
                            }

                            return null;
                        }
                        else
                        {
                            if (exec1 is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss + value2;
                                    return var;
                                }
                                else
                                {
                                    var var = ss + exec2;
                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)exec1;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii + iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii + iii;
                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii + sss;
                                    return var;
                                }
                            }

                            return null;
                        }
                    };
                    return;
                }
                if (op == "-")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable1.GetValue();
                            if (value is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss.Replace(value2.ToString(), "");
                                    return var;
                                }
                                else
                                {
                                    var var = ss.Replace(exec2.ToString(), "");
                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)value;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii - iii;
                                        return var;
                                    }
                                    else
                                    {
                                        var var = ii.ToString().Replace(value2.ToString(), "");
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii - iii;
                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii.ToString().Replace(sss, "");
                                    return var;
                                }
                            }

                            return null;
                        }
                        else
                        {
                            if (exec1 is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss.Replace(value2.ToString(), "");
                                    return var;
                                }
                                else
                                {
                                    var var = ss.Replace(exec2.ToString(), "");
                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)exec1;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii - iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii - iii;
                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii.ToString().Replace(sss, "");
                                    return var;
                                }
                            }

                            return null;
                        }
                    };
                    return;
                }
                if (op == "+=")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable.GetValue();
                            if (value is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss + value2;

                                    variable.SetValue(var);

                                    return var;
                                }
                                else
                                {
                                    var var = ss + exec2;

                                    variable.SetValue(var);

                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)value;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii + iii;
                                        variable.SetValue(var);
                                        return var;
                                    }
                                    else
                                    {
                                        var var = ii + value2.ToString();
                                        variable.SetValue(var);
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii + iii;

                                    variable.SetValue(var);

                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii + sss;

                                    variable.SetValue(var);

                                    return var;
                                }
                            }

                            return null;
                        }
                        else
                        {
                            if (exec1 is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss + value2;
                                    return var;
                                }
                                else
                                {
                                    var var = ss + exec2;
                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)exec1;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii + iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii + iii;
                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii + sss;
                                    return var;
                                }
                            }

                            return null;
                        }

                    };
                    return;
                }
                if (op == "-=")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable.GetValue();
                            if (value is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss.Replace(value2.ToString(), "");

                                    variable.SetValue(var);

                                    return var;
                                }
                                else
                                {
                                    var var = ss.Replace(exec2.ToString(), "");

                                    variable.SetValue(var);

                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)value;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii - iii;
                                        variable.SetValue(var);
                                        return var;
                                    }
                                    else
                                    {
                                        var var = ii.ToString().Replace(value2.ToString(), "");
                                        variable.SetValue(var);
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii - iii;

                                    variable.SetValue(var);

                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii.ToString().Replace(sss, "");

                                    variable.SetValue(var);

                                    return var;
                                }
                            }

                            return null;
                        }
                        else
                        {
                            if (exec1 is string ss)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    var var = ss.Replace(value2.ToString(), "");
                                    return var;
                                }
                                else
                                {
                                    var var = ss.Replace(exec2.ToString(), "");
                                    return var;
                                }

                            }
                            else
                            {
                                var ii = (int)exec1;
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii - iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii - iii;
                                    return var;
                                }
                                else if (exec2 is string sss)
                                {
                                    var var = ii.ToString().Replace(sss, "");
                                    return var;
                                }
                            }

                            return null;
                        }

                    };
                    return;
                }
                if (op == "||")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable1.GetValue();
                            if (value is int ii)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii | iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii | iii;
                                    return var;
                                }

                            }

                            return null;
                        }
                        else
                        {
                            if (exec1 is int ii)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii | iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii | iii;
                                    return var;
                                }

                            }

                            return null;
                        }
                    };
                    return;
                }
                if (op == "&&")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable1.GetValue();
                            if (value is int ii)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii & iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii & iii;
                                    return var;
                                }

                            }

                            return null;
                        }
                        else
                        {
                            if (exec1 is int ii)
                            {
                                if (exp2.preType == PreType.Ind)
                                {
                                    var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                    var value2 = variable2.GetValue();

                                    if (value2 is int iii)
                                    {
                                        var var = ii & iii;
                                        return var;
                                    }
                                }
                                else if (exec2 is int iii)
                                {
                                    var var = ii & iii;
                                    return var;
                                }

                            }

                            return null;
                        }
                    };
                    return;
                }
                if (op == "==")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable1.GetValue();

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                return value.ExEquals(value2);
                            }
                            else if (exec2 is int iii)
                            {
                                return value.ExEquals(iii);
                            }

                            return null;
                        }
                        else
                        {

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                return exec1.ExEquals(value2);
                            }
                            else
                            {
                                return exec1.ExEquals(exec2);
                            }
                        }
                    };
                    return;
                }
                if (op == "!=")
                {
                    Func = () =>
                    {
                        var exec1 = exp1.Func();
                        var exec2 = exp2.Func();

                        if (exec1 is Return r1)
                        {
                            return r1;
                        }

                        if (exec2 is Return r2)
                        {
                            return r2;
                        }

                        if (exp1.preType == PreType.Ind)
                        {
                            var variable1 = Memory.GetVariable(exec1.ToString(), Block);
                            var value = variable1.GetValue();

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                return !value.ExEquals(value2);
                            }
                            else if (exec2 is int iii)
                            {
                                return !iii.ExEquals(exec2);
                            }

                            return null;
                        }
                        else
                        {

                            if (exp2.preType == PreType.Ind)
                            {
                                var variable2 = Memory.GetVariable(exec2.ToString(), Block);
                                var value2 = variable2.GetValue();

                                return !exec1.ExEquals(value2);
                            }
                            else
                            {
                                return !exec1.ExEquals(exec2);
                            }
                        }
                    };
                    return;
                }
                if (op == ",")
                {
                    Func = () => null;
                }
            }
        }

        private bool CheckReturn(object o)
        {
            if (o is Return)
            {
                return true;
            }

            return false;
        }

        private int GetInt(object s)
        {
            if (s is string ss)
            {
                if (int.TryParse(ss, out var i))
                {
                    return i;
                }
                return ss.Length;
            }

            if (s is int ii)
            {
                return ii;
            }

            return 0;
        }

        public object GetValue()
        {
            var exec = Func();

            if (preType == PreType.Ind)
            {
                var variable = Memory.GetVariable(exec.ToString(), Block);
                var value = variable.GetValue();

                return value;
            }

            return exec;
        }

        public override string ToString()
        {
            return Txt;
        }
    }


    enum ExpType
    {
        Literar,
        Indexing,
        Function,
        Binary,
        Unitary,
        Pre
    }

    enum PreType
    {
        None = -1,
        Int,
        Str,
        Ind,
        Par,
    }
}
