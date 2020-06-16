using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    class Operation
    {
        public string OperationStr;
        public Function Function;

        public Operation(string operation, Function function)
        {
            OperationStr = operation;
            Function = function;
        }
    }
}
