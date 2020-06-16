using System;
using System.Collections.Generic;

namespace New_Inter.Classes
{
    class StrClass : IClass
    {
        public string Value { get; set; }

        public string Identifier { get; set; }
        public List<Statement> Statements { get; set; }
        public List<Function> Functions { get; set; }
        public string Block { get; set; }
        public Lib Lib { get; set; }
        public List<Operation> Operations { get; set; }

        public StrClass(string value)
        {
            Init();
            Value = value;
        }

        public StrClass()
        {
            Init();

            Value = "";
        }

        private void Init()
        {
            Operations = new List<Operation>();
            Statements = new List<Statement>();
            Functions = new List<Function>();

            Operations.Add(new Operation("==", new Function("oper a, b:", Lib, Block)
            {
                Exec = o =>
                {
                    var arr = (object[])o;

                    if (arr.Length != 2)
                    {
                        return null;
                    }

                    var a = arr[0].GetStr();
                    var b = arr[1].GetStr();

                    var newClass = new BoolClass(a == b);

                    return newClass;
                }
            }));

            Operations.Add(new Operation("!=", new Function("oper a, b:", Lib, Block)
            {
                Exec = o =>
                {
                    var arr = (object[])o;

                    if (arr.Length != 2)
                    {
                        return null;
                    }

                    var a = arr[0].GetInt();
                    var b = arr[1].GetInt();

                    var newClass = new BoolClass(a != b);

                    return newClass;
                }
            }));


            Operations.Add(new Operation("+", new Function("oper a, b:", Lib, Block)
            {
                Exec = o =>
                {
                    var arr = (object[])o;

                    if (arr.Length != 2)
                    {
                        return null;
                    }

                    var a = arr[0].GetStr();
                    var b = arr[1].GetStr();

                    var newClass = new StrClass(a + b);

                    return newClass;
                }
            }));
        }

        public object Get(string str)
        {
            return str;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
