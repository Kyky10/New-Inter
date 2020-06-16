using System;
using System.Collections.Generic;

namespace New_Inter.Classes
{
    class IntClass : IClass //"=", "+", "-", "+=", "-=", "||", "&&", "==", "!=", ">", "<", ">=", "<=" + "-", "!", "++", "--"
    {
        public int Value { get; set; }

        public string Identifier { get; set; }
        public List<Statement> Statements { get; set; }
        public List<Function> Functions { get; set; }
        public string Block { get; set; }
        public Lib Lib { get; set; }
        public List<Operation> Operations { get; set; }

        public IntClass(int value)
        {
            Init();
            Value = value;
        }

        public IntClass()
        {
            Init();

            Value = 0;
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

                    var a = arr[0].GetInt();
                    var b = arr[1].GetInt();

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

                    var a = arr[0].GetInt();
                    var b = arr[1].GetInt();

                    var newClass = new IntClass(a + b);

                    return newClass;
                }
            }));



            Operations.Add(new Operation("-", new Function("oper a, b:", Lib, Block)
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

                    var newClass = new IntClass(a - b);

                    return newClass;
                }
            }));





            Operations.Add(new Operation(">", new Function("oper a, b:", Lib, Block)
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

                    var newClass = new BoolClass(a > b);

                    return newClass;
                }
            }));







            Operations.Add(new Operation("<", new Function("oper a, b:", Lib, Block)
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

                    var newClass = new BoolClass(a < b);

                    return newClass;
                }
            }));






            Operations.Add(new Operation("&", new Function("oper a, b:", Lib, Block)
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





            Operations.Add(new Operation("|", new Function("oper a, b:", Lib, Block)
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
            return Value.ToString();
        }
    }
}
