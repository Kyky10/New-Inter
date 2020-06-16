using System;

namespace New_Inter
{
    class Variable
    {
        public Variable(string name, string block, object value)
        {
            Name = name;
            Type = value?.GetType();
            Block = block;
            Value = value;
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object o)
        {
            Type = o?.GetType();
            Value = o;
        }

        public override string ToString()
        {
            return Name + ":" + GetValue();
        }

        public string Name;
        public Type Type;
        public string Block;
        public object Value;
    }
}
