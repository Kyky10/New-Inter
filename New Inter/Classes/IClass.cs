using System;
using System.Collections.Generic;

namespace New_Inter
{
    interface IClass
    {
        string Identifier { get; set; }
        List<Statement> Statements { get; set; }
        List<Function> Functions { get; set; }
        string Block { get; set; }
        Lib Lib { get; set; }
        List<Operation> Operations { get; set; }

        object Get(string str);
        string ToString();
    }
}
