using System;
using System.Collections.Generic;

namespace New_Inter //(string function, List<object> parameters, ExecLine exec, object retObj)
{
    class FuncExec
    {
        public string Function;
        public List<object> Parameters;
        public ExecLine Exec;
        public Object RetObj;

        public FuncExec(string function, List<object> parameters, ExecLine exec, object retObj)
        {
            Function = function;
            Parameters = parameters;
            Exec = exec;
            RetObj = retObj;
        }
    }
}
