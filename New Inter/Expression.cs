using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    class Expression
    {
        public string Txt;
        public ExpType type;
        public PreType preType;

        public Func<object> ExecFunc
        {
            get
            {
                execValue = Func();
                return () => execValue;
            }
            set => Func = value;
        }

        private object execValue;
        public Func<object> Func;
        public string Block;

        private readonly string[][] Tokens = {
            new[]{"=", "+", "-", "+=", "-=", "||", "&&", "==", "!=", ">", "<", ">=", "<="}, //binary 0
            new[]{"&", "*", "-", "!", "++", "--"}, //unitary 1
            new[]{ "(", ")", "[", "]", "{", "}", ";"}, //parantes 2
            new[]{ "?"} //tenary 3
            };


        public Expression(string txt, string block)
        {
            txt = txt.Trim();
            Txt = txt;
            Block = block;

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

                ExecFunc = () => int.Parse(p);
                return;
            }

            if (GetStr(txt, out var Qtxt))
            {
                type = ExpType.Literar;
                preType = PreType.Str;

                ExecFunc = () => Qtxt;
                return;
            }

            var l = txt[0] == '(';
            var f = txt[txt.Length - 1] == ')';

            if (l && f)
            {

                txt = txt.Substring(1, txt.Length - 2);

                type = ExpType.Literar;
                preType = PreType.Par;

                var exp = new Expression(txt, Block);

                ExecFunc = exp.ExecFunc;
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
                    var expr = new Expression(exprTxt, Block);

                    if (@operator == "&")
                    {
                        ExecFunc = () => expr.ExecFunc();
                        return;
                    }
                    if (@operator == "*")
                    {
                        ExecFunc = () => expr.ExecFunc();
                        return;
                    }
                    if (@operator == "-")
                    {
                        ExecFunc = () =>
                        {
                            var value = expr.ExecFunc();

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

                            return value;
                        };
                        return;
                    }
                    if (@operator == "!")
                    {
                        ExecFunc = () =>
                        {
                            var value = expr.ExecFunc();

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

                            return value;
                        };
                        return;
                    }
                    if (@operator == "++")
                    {
                        ExecFunc = () =>
                        {
                            var value = expr.ExecFunc();

                            if (value is int i)
                            {
                                value = i + 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToUpper();
                            }

                            return value;
                        };
                        return;
                    }
                    if (@operator == "--")
                    {
                        ExecFunc = () =>
                        {
                            var value = expr.ExecFunc();

                            if (value is int i)
                            {
                                value = i - 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToLower();
                            }

                            return value;
                        };
                        return;
                    }

                    return;
                }

                if (txt.EndsWith(@operator))
                {
                    type = ExpType.Unitary;
                    preType = PreType.None;


                    var len = @operator.Length;
                    var exprTxt = txt.Substring(0, txt.Length - len);
                    var expr = new Expression(exprTxt, Block);

                    if (@operator == "++")
                    {
                        ExecFunc = () =>
                        {
                            var value = expr.ExecFunc();

                            if (value is int i)
                            {
                                value = i + 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToUpper();
                            }

                            return value;
                        };
                        return;
                    }
                    if (@operator == "--")
                    {
                        ExecFunc = () =>
                        {
                            var value = expr.ExecFunc();

                            if (value is int i)
                            {
                                value = i - 1;
                            }

                            if (value is string s)
                            {
                                value = s.ToLower();
                            }

                            return value;
                        };
                        return;
                    }

                    return;
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

                var exps1 = txt.Substring(0, ops[0].i);
                var exps2 = txt.Substring(ops[0].i + 1);

                var exp1 = new Expression(exps1, Block);
                var exp2 = new Expression(exps2, Block);

                type = ExpType.Binary;
                preType = PreType.None;

                GetOperation(op, exp1, exp2);
                return;
            }


            var identifier = GetIdentifier(txt);

            if (GetStr(identifier, out var paranteses, '(', ')'))
            {
                var indexOfParant = identifier.IndexOf('(' + paranteses + ')', StringComparison.Ordinal);
                var noParanteses = identifier.Substring(0, indexOfParant).Trim();
                
                var parametersTxt = paranteses.Split(',');
                var parametersExpresions = new List<Expression>();

                foreach (var parantese in parametersTxt)
                {
                    var paranteseTrim = parantese.Trim();

                    var expresion = new Expression(paranteseTrim, Block);
                    parametersExpresions.Add(expresion);
                }

                ExecFunc = () =>
                {
                    var fu = Memory.GetFunction(noParanteses);
                    var parameters = new List<object>();
                    foreach (var expresion in parametersExpresions)
                    {
                        parameters.Add(expresion.GetValue());
                    }

                    var ret = fu.ExecFunc(parameters.ToArray());

                    return ret;
                };

                return;
            }

            type = ExpType.Literar;
            preType = PreType.Ind;
            ExecFunc = () => GetIdentifier(txt);
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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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

                                    var var = ss.ToString().Replace(value2.ToString(), "");
                                    return var;
                                }
                                else
                                {
                                    var var = ss.ToString().Replace(exec2.ToString(), "");
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
                                    var var = ii.ToString().ToString().Replace(sss, "");
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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () =>
                    {
                        var exec1 = exp1.ExecFunc();
                        var exec2 = exp2.ExecFunc();


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
                    ExecFunc = () => null;
                }
            }
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
