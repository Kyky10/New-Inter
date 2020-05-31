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
        public List<Function> Functions = new List<Function>();
        public string Txt;
        public bool FunctionCall;
        public (string function, List<object> parameters, (List<object> line, int i) exec, object retObj) ReturnFunc;
        public List<(string function, List<object> parameters, (List<object> line, int i) exec, object retObj)> FunctionsExec;
        public Return MainReturn;

        public Lib(string txt)
        {
            FunctionsExec = new List<(string function, List<object> parameters, (List<object> line, int i) exec, object retObj)>();
            FunctionCall = false;
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

        public void PreCompile(List<object> parameters)
        {
            var main = Functions.Find(x => x.Indentifier.ToLower() == "main");
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


            FunctionsExec.Add(("main", parameters, (line, 0), null));
        }

        public void Step()
        {
            var lastF = FunctionsExec.Last();
            var i = lastF.exec.i;
            var obj = lastF.exec.line[i];
            object ret = null;

            if (obj is Statement s)
            {
                ret = s.ExecFunc();
            }
            else if (obj is Expression e)
            {
                if (e.AwaitReturn)
                {
                    e.CallFunctionReturn = ReturnFunc.retObj;
                }
                ret = e.ExecFunc();
            }

            if (FunctionCall)
            {
                FunctionCall = false;
                return;
            }

            lastF.exec.i = i + 1;

            if (lastF.exec.i > lastF.exec.line.Count || ret is Return)
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
            var functionF = Functions.Find(x => x.Indentifier == function);
            var line = treeToList(functionF.GetTree());

            var arr = parameters;
            for (int i = 0; i < functionF.Parameters.Count; i++)
            {
                functionF.Parameters[i] = functionF.Parameters[i].Trim();
                var value = arr[i];
                if (value is string s)
                {
                    Memory.SetVariable(functionF.Parameters[i], functionF.Block, s);
                }
                if (value is int ii)
                {
                    Memory.SetVariable(functionF.Parameters[i], functionF.Block, ii);
                }
            }

            FunctionsExec.Add((function, parameters, (line, 0), null));
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
