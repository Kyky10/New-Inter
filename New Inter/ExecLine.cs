using System.Collections.Generic;

namespace New_Inter
{
    class ExecLine
    {
        public List<object> Line;
        public int I;
        public Function Function;

        public ExecLine(List<object> line, int i, Function function)
        {
            Line = line;
            I = i;
            Function = function;
        }
    }
}
