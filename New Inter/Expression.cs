using System;
using System.Collections.Generic;
using System.Linq;
using New_Inter.Classes;

namespace New_Inter
{
    class Expression
    {
        public string Txt;
        public EType Type;
        public Func<object> Exec;
        public string Block;
        public Statement Statement;
        public object CallFunctionReturn;
        public bool AwaitReturn;
        public Lib Lib;

        private readonly string[][] _tokens = {
            new[]{"=", "+", "-", "+=", "-=", "||", "&&", "==", "!=", ">", "<", ">=", "<="}, //binary 0
            new[]{ "-", "!", "++", "--"}, //unitary 1
            new[]{ "(", ")", "[", "]", "{", "}", ";"}, //parantes 2
            new[]{ "?"} //tenary 3
            };

        private readonly string[] priority = {"||", "&&"};


        public Expression(string txt, string block, Statement statement)
        {
            txt = txt.Trim();
            Txt = txt;
            Block = block;
            Statement = statement;
            CallFunctionReturn = null;
            AwaitReturn = false;
            Lib = Statement.Lib;

            var binaryOperators = _tokens[0]
                .OrderByDescending(x => x.Length)
                .ToArray();
            var unitaryOperators = _tokens[1]
                .OrderByDescending(x => x.Length)
                .ToArray();

            if (int.TryParse(txt, out _))
            {
                Type = EType.Int;

                var p = txt;

                Exec = () =>
                {
                    var value = p.GetInt();

                    var i = new IntClass(value);

                    return i;
                };
                return;
            }

            if (GetStr(txt, out var qtxt))
            {
                if (qtxt.Length == txt.Length - 2)
                {
                    Type = EType.Str;

                    Exec = () =>
                    {
                        var value = qtxt.GetStr();

                        var s = new StrClass(value);

                        return s;
                    };
                    return;
                }
            }

            if (bool.TryParse(txt, out var valueBool))
            {
                Type = EType.Bool;

                Exec = () =>
                {
                    var value = new BoolClass(valueBool);

                    return value;
                };
                return;
            }

            if (txt.StartsWith("new"))
            {
                var ident = txt.Substring(3);
                if(GetStr(ident, out var brackets, '(', ')'))
                {
                    CustomClass customClass = null;
                    var i = ident.IndexOf("(" + brackets + ")", StringComparison.Ordinal);
                    var identWb = ident.Remove(i, brackets.Length + 2).Trim();
                    var parametersExpresions = new List<Expression>();

                    if (!string.IsNullOrWhiteSpace(brackets))
                    {
                        var parametersTxt = brackets.Split(',');

                        foreach (var parantese in parametersTxt)
                        {
                            var paranteseTrim = parantese.Trim();

                            var expresion = new Expression(paranteseTrim, Block, Statement);
                            parametersExpresions.Add(expresion);
                        }
                    }

                    Exec = () =>
                    {
                        if (!AwaitReturn)
                        {
                            var parametersValue = parametersExpresions.Select(x => x.GetValue()).ToList();

                            customClass = Memory.GetClass( identWb, Block).Clone();

                            customClass.Assign(parametersValue);
                            AwaitReturn = true;

                            return new Return(null, Flag.Function);
                        }
                        else
                        {
                            AwaitReturn = false;
                            return (IClass)customClass;
                        }
                    };
                }
                return;
            }

            var l = txt.First() == '(';
            var f = txt.Last() == ')';

            if (l && f)
            {

                txt = txt.Substring(1, txt.Length - 2);

                Type = EType.Par;

                var exp = new Expression(txt, Block ,Statement);

                Exec = exp.Exec;
                return;
            }

            foreach (var @operator in unitaryOperators)
            {
                if (txt.StartsWith(@operator))
                {
                    Type = EType.None;


                    var len = @operator.Length;
                    var exprTxt = txt.Substring(len, txt.Length - len);
                    var expr = new Expression(exprTxt, Block ,Statement);
                    
                    if (@operator == "-")
                    {
                        Exec = () =>
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

                            if (expr.Type == EType.Ind)
                            {
                                var variable = expr.Exec().ToString();
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
                        Exec = () =>
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

                            if (expr.Type == EType.Ind)
                            {
                                var variable = expr.Exec().ToString();
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
                        Exec = () =>
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

                            if (expr.Type == EType.Ind)
                            {
                                var variable = expr.Exec().ToString();
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
                        Exec = () =>
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

                            if (expr.Type == EType.Ind)
                            {
                                var variable = expr.Exec().ToString();
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
                    Type = EType.None;


                    var len = @operator.Length;
                    var exprTxt = txt.Substring(0, txt.Length - len);
                    var expr = new Expression(exprTxt, Block, Statement);


                    if (@operator == "++")
                    {
                        Exec = () =>
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

                            if (expr.Type == EType.Ind)
                            {
                                var variable = expr.Exec().ToString();
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
                        Exec = () =>
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

                            if (expr.Type == EType.Ind)
                            {
                                var variable = expr.Exec().ToString();
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
                    var add = true;

                    foreach (var op in ops)
                    {
                        var overlap = toAdd.i < op.i + op.l && op.i < toAdd.i + toAdd.Length;
                        if (overlap)
                        {
                            add = false;
                            break;
                        }
                    }

                    if (add)
                    {
                        ops.Add(toAdd);
                    }
                }
            }

            ops = ops.OrderBy(x =>
            {
                if (priority.Contains(x.o))
                {
                    return 0;
                }
                else
                {
                    return x.i;
                }
            }).ToList();

            var exprsStr = txt.Split(binaryOperators, 2, StringSplitOptions.None);

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

                    Type = EType.Asg;

                    SetOperation(op, exp1, exp2);
                    return;
                }
            }


            if (GetStr(Txt, out var paranteses, '(', ')'))
            {
                Type = EType.Func;

                var indexOfParant = Txt.IndexOf('(' + paranteses + ')', StringComparison.Ordinal);
                var function = Txt.Substring(0, indexOfParant).Trim();
                var parametersExpresions = new List<Expression>();

                if (!string.IsNullOrWhiteSpace(paranteses))
                {
                    parametersExpresions = paranteses.Split(',')
                        .Select(x => new Expression(x.Trim(),
                            Block,
                            Statement))
                        .ToList();
                }

                if (function.Contains("."))
                {
                    var classAccess = function;
                    var exprClassAccess = new Expression(classAccess, block, Statement);
                    Exec = () =>
                    {
                        if (!AwaitReturn)
                        {
                            var value = exprClassAccess.GetValue();

                            if (value is Function func)
                            {
                                var parameters = parametersExpresions.Select(x => x.GetValue()).ToList();

                                Lib.AccessNewFunction(func, parameters);
                                AwaitReturn = true;
                            }
                            else if(value is Variable variable)
                            {
                                if (variable.Type == typeof(Function))
                                {
                                    var func2 = (Function)variable.GetValue();
                                    var parameters = parametersExpresions.Select(x => x.GetValue()).ToList();

                                    Lib.AccessNewFunction(func2, parameters);
                                    AwaitReturn = true;
                                }
                            }

                            return null;
                        }
                        else
                        {
                            CallFunctionReturn = Lib.GetReturn();
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
                else
                {
                    Exec = () =>
                    {
                        if (!AwaitReturn)
                        {
                            AwaitReturn = true;

                            var parameters = parametersExpresions.Select(x => x.GetValue()).ToList();

                            Lib.AccessNewFunction(function, parameters);

                            return new Return(null, Flag.Function);
                        }
                        else
                        {
                            CallFunctionReturn = Lib.GetReturn();
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
                }
                
                return;
            }

            if (Txt.Contains("."))
            {
                Type = EType.Cls;

                var dotIndex = Txt.LastIndexOf('.');
                var split = Txt.SplitAtIndex(dotIndex);
                var second = split[1].Substring(1);

                var exprTxt = split[0];
                var expr = new Expression(exprTxt, block, Statement);
                Exec = () =>
                {
                    var value = expr.GetValue();
                    if(value is IClass @class)
                    {
                        return @class.Get(second);
                    }

                    return null;
                };

                return;
            }

            Type = EType.Ind;
            Exec = () => GetIdentifier(txt);
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
            return str.Trim();
        }

        private void SetOperation(string op, Expression exp1, Expression exp2)
        {
            if (op == "=")
            {
                Exec = () =>
                {
                    var exec1 = exp1.Exec();
                    var exec2 = exp2.GetValue();

                    if (exec1 is Return r1)
                    {
                        return r1;
                    }

                    if (exec2 is Return r2)
                    {
                        return r2;
                    }

                    if (exp1.Type == EType.Ind)
                    {
                        var variable1 = Memory.GetVariable(exec1.ToString(), Block);

                        if (variable1 is null)
                        {
                            variable1 = new Variable(exec1.ToString(), Block, null);
                            Memory.AddVariable(variable1);
                        }

                        variable1.SetValue(exec2);

                        return null;
                    }


                    return null;
                };
                return;
            }
            else
            {
                Exec = () =>
                {
                    var value1 = exp1.GetValue();
                    var value2 = exp2.GetValue();

                    if (value1 is IClass class1)
                    {
                        if (value2 is IClass class2)
                        {
                            var oper = class1.Operations.Find(x => x.OperationStr == op);
                            var func = oper.Function;

                            var retValue = func.Exec(new[] {class1, class2});

                            return retValue;
                        }
                    }

                    return null;
                };
            }

        }

        public object GetValue()
        {
            var exec = Exec();

            if (Type == EType.Ind)
            {
                var variable = Memory.GetVariable(exec.ToString(), Block);
                var value = variable.GetValue();
                return value;
            }

            if (exec is Variable v)
            {
                var value = v.GetValue();
                return value;
            }

            return exec;
        }

        public override string ToString()
        {
            return Txt;
        }
    }



    enum EType
    {
        None = -1,
        Int,
        Str,
        Ind,
        Bool,
        Par,
        Cls,
        Func,
        Asg
    }
}
