using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    class Return
    {
        public object Value;
        public Flag Flag;
        public Return(object value = null, Flag flag = Flag.Continue)
        {
            Value = value;
            Flag = flag;
        }
    }


    enum Flag
    {
        Continue,
        Repeat,
        Stop,
        Error,
        Function
    }
}
