using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    class Lib
    {
        public string Txt;
        public bool FunctionCall;
        private List<object> ReturnFunc;
        public List<FuncExec> FunctionsExec;
        public Return MainReturn;

        public Lib(string txt)
        {
            FunctionsExec = new List<FuncExec>();
            ReturnFunc = new List<object>();
            FunctionCall = false;
            Txt = txt;

            var functions = new List<Function>();
            var functionsTxt = txt.SplitSkip(";");
            foreach (var functionTxt in functionsTxt)
            {
                var functionTxtTrim = functionTxt.Trim();

                if (functionTxtTrim.StartsWith("class"))
                {
                    var l = "class".Length + 1;
                    var classStruc = functionTxtTrim.Substring(l);
                    var idenEnd = classStruc.IndexOf('{');

                    var iden = classStruc.Substring(0, idenEnd).Trim();

                    var classTxt = Ext.GetExpressionBr(classStruc);

                    var c = new CustomClass(iden, classTxt.txt, this);

                    Memory.Classes.Add(c);
                }
                else
                {
                    var function = new Function(functionTxtTrim, this);
                    functions.Add(function);
                }
            }
            Memory.Functions.AddRange(functions);
        }

        public int Exec(object[] parameters)
        {
            var main = Memory.Functions.Find(x => x.Identifier.ToLower() == "main");
            var ret = main.Exec(parameters);
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

        public void PreCompile(List<object> parameters)
        {
            var main = Memory.Functions.Find(x => x.Identifier.ToLower() == "main");
            var line = treeToList(main.GetTree());
            

            var arr = parameters;
            for (int i = 0; i < main.Parameters.Count; i++)
            {
                main.Parameters[i] = main.Parameters[i].Trim();
                var value = arr[i];
                if (value is string s)
                {
                    Memory.SetVariable(main.Parameters[i], main.Block, s);
                }
                if (value is int ii)
                {
                    Memory.SetVariable(main.Parameters[i], main.Block, ii);
                }
            }


            FunctionsExec.Add(new FuncExec("main", parameters, new ExecLine(line, 0, main), null));
        }

        public void Step()
        {
            object ret = null;
            var lastF = FunctionsExec.Last();
            var i = lastF.Exec.I;

            if (!lastF.Exec.Line.Any())
            {
                ret = lastF.Exec.Function.Exec(lastF.Parameters);

                lastF.RetObj = ret;
                AddReturn(lastF.RetObj);

                FunctionsExec.RemoveAt(FunctionsExec.Count - 1);

                if (!FunctionsExec.Any())
                {
                    MainReturn = !(ret is Return) ? new Return() : (Return) ret ;
                }

                return;
            }

            var obj = lastF.Exec.Line[i];
            var flag = Flag.Continue;
            

            if (obj is Statement s)
            {
                if (!s.Ignone)
                {
                    ret = s.Exec();
                }
            }
            else if (obj is Expression e)
            {
                ret = e.Exec();
            }

            if (FunctionCall)
            {
                FunctionCall = false;
                return;
            }

            lastF.Exec.I = i + 1;

            if (ret is Return r)
            {
                if (r.Flag == Flag.Repeat)
                {
                    lastF.Exec.I = i;
                }

                flag = r.Flag;
            }

            if (lastF.Exec.I > lastF.Exec.Line.Count - 1 || (flag != Flag.Continue && flag != Flag.Repeat ))
            {
                lastF.RetObj = ret;
                AddReturn(lastF.RetObj);

                FunctionsExec.RemoveAt(FunctionsExec.Count - 1);

                if (!FunctionsExec.Any())
                {
                    MainReturn = !(ret is Return) ? new Return() : (Return)ret;
                }

                return;
            }

            FunctionsExec[FunctionsExec.Count - 1] = lastF;
        }

        public void AccessNewFunction(string function, List<object> parameters)
        {
            var functionF = Memory.Functions.Find(x => x.Identifier == function);
            var line = treeToList(functionF.GetTree());

            var arr = parameters;
            for (int i = 0; i < functionF.Parameters.Count; i++)
            {
                functionF.Parameters[i] = functionF.Parameters[i].Trim();
                var value = arr.Count - 1 <= i ? arr[i] : null;
                if (value is string s)
                {
                    Memory.SetVariable(functionF.Parameters[i], functionF.Block, s);
                }
                if (value is int ii)
                {
                    Memory.SetVariable(functionF.Parameters[i], functionF.Block, ii);
                }
            }

            FunctionsExec.Add(new FuncExec(function, parameters, new ExecLine(line, 0, functionF), null));
            FunctionCall = true;
        }

        public void AccessNewFunction(Function function, List<object> parameters)
        {
            var line = treeToList(function.GetTree());
            
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                function.Parameters[i] = function.Parameters[i].Trim();
                var value = parameters.Count - 1 <= i ? parameters[i] : null;
                if (value is string s)
                {
                    Memory.SetVariable(function.Parameters[i], function.Block, s);
                }
                if (value is int ii)
                {
                    Memory.SetVariable(function.Parameters[i], function.Block, ii);
                }
            }

            FunctionsExec.Add(new FuncExec(function.Identifier, parameters, new ExecLine(line, 0, function), null));
            FunctionCall = true;
        }

        public object GetReturn()
        {
            var ret = ReturnFunc.Last();
            ReturnFunc.Remove(ret);

            return ret;
        }

        public void AddReturn(object ret)
        {
            ReturnFunc.Add(ret);
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
