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
        public (string function, List<object> parameters, (List<object> line, int i, Function function) exec, object retObj) ReturnFunc;
        public List<(string function, List<object> parameters, (List<object> line, int i, Function function) exec, object retObj)> FunctionsExec;
        public Return MainReturn;

        public Lib(string txt)
        {
            FunctionsExec = new List<(string function, List<object> parameters, (List<object> line, int i, Function function) exec, object retObj)>();
            FunctionCall = false;
            Txt = txt;


            var functions = new List<Function>();
            var functionsTxt = txt.Split(new []{"::"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var functionTxt in functionsTxt)
            {
                var function = new Function(functionTxt, this);
                functions.Add(function);
            }

            Memory.Functions.AddRange(functions);
        }

        public int Exec(object[] parameters)
        {
            var main = Memory.Functions.Find(x => x.Indentifier.ToLower() == "main");
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

        public void PreCompile(List<object> parameters)
        {
            var main = Memory.Functions.Find(x => x.Indentifier.ToLower() == "main");
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


            FunctionsExec.Add(("main", parameters, (line, 0, main), null));
        }

        public void Step()
        {
            object ret = null;
            var lastF = FunctionsExec.Last();
            var i = lastF.exec.i;

            if (!lastF.exec.line.Any())
            {
                ret = lastF.exec.function.ExecFunc(lastF.parameters);

                lastF.retObj = ret;
                ReturnFunc = lastF;

                FunctionsExec.RemoveAt(FunctionsExec.Count - 1);

                if (!FunctionsExec.Any())
                {
                    Return mainReturn;

                    if (!(ret is Return))
                    {
                        mainReturn = new Return();
                    }
                    else
                    {
                        mainReturn = (Return)ret;
                    }

                    MainReturn = mainReturn;
                }

                return;
            }

            var obj = lastF.exec.line[i];
            var flag = Flag.Continue;
            

            if (obj is Statement s)
            {
                if (!s.Ignone)
                {
                    ret = s.Func();
                }
            }
            else if (obj is Expression e)
            {
                ret = e.Func();
            }

            if (FunctionCall)
            {
                FunctionCall = false;
                return;
            }

            lastF.exec.i = i + 1;

            if (ret is Return r)
            {
                if (r.Flag == Flag.Repeat)
                {
                    lastF.exec.i = i;
                }

                flag = r.Flag;
            }

            if (lastF.exec.i > lastF.exec.line.Count - 1 || (flag != Flag.Continue && flag != Flag.Repeat ))
            {
                lastF.retObj = ret;
                ReturnFunc = lastF;

                FunctionsExec.RemoveAt(FunctionsExec.Count - 1);

                if (!FunctionsExec.Any())
                {
                    Return mainReturn;

                    if (!(ret is Return))
                    {
                        mainReturn = new Return();
                    }
                    else
                    {
                        mainReturn = (Return)ret;
                    }

                    MainReturn = mainReturn;
                }

                return;
            }

            FunctionsExec[FunctionsExec.Count - 1] = lastF;
        }

        public void AccessNewFunction(string function, List<object> parameters)
        {
            var functionF = Memory.Functions.Find(x => x.Indentifier == function);
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

            FunctionsExec.Add((function, parameters, (line, 0, functionF), null));
            FunctionCall = true;
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
