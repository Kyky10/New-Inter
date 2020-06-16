using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter.Classes
{
    class BoolClass : IClass
    {
        public bool Value;

        public string Identifier { get; set; }
        public List<Statement> Statements { get; set; }
        public List<Function> Functions { get; set; }
        public string Block { get; set; }
        public Lib Lib { get; set; }
        public List<Operation> Operations { get; set; }

        public BoolClass(bool value)
        {
            Init();

            Value = value;
        }

        public BoolClass()
        {
            Init();

            Value = false;
        }

        private void Init()
        {
            Operations = new List<Operation>();
            Statements = new List<Statement>();
            Functions = new List<Function>();

            Operations.Add(new Operation("&&", new Function("& a, b:", Lib, Block)
            {
                Exec = o =>
                {
                    var arr = (object[])o;

                    if (arr.Length != 2)
                    {
                        return null;
                    }

                    var a = arr[0].GetInt() % 2 == 1;
                    var b = arr[1].GetInt() % 2 == 1;

                    var newClass = new BoolClass(a && b);

                    return newClass;
                }
            }));





            Operations.Add(new Operation("||", new Function("| a, b:", Lib, Block)
            {
                Exec = o =>
                {
                    var arr = (object[])o;

                    if (arr.Length != 2)
                    {
                        return null;
                    }

                    var a = arr[0].GetInt() % 2 == 1;
                    var b = arr[1].GetInt() % 2 == 1;

                    var newClass = new BoolClass(a || b);

                    return newClass;
                }
            }));
        }

        public object Get(string str)
        {
            return null;
        }


        public override string ToString()
        {
            return Value ? "true" : "false";
        }
    }
}
