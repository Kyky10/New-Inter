using System;
using System.Collections.Generic;
using System.Linq;

namespace New_Inter
{
    class CustomClass : IClass
    {
        private string[] operationsStr = 
        {"=", "+", "-", "+=", "-=", "||", "&&", "==", "!=", ">", "<", ">=", "<=",   "!", "++", "--" };

        public string Identifier { get; set; }
        public List<Statement> Statements { get; set; }
        public List<Function> Functions { get; set; }
        public string Block { get; set; }
        public string ClassBlock;
        public Lib Lib { get; set; }
        public List<Operation> Operations { get; set; }
        public string Txt { get; set; }

        public CustomClass(string identifier, string txt, Lib lib, string block = null)
        {
            operationsStr  = operationsStr.OrderByDescending(x => x.Length).ToArray();
            Identifier = identifier;
            Block = block ?? "";
            ClassBlock = RandomString(5);
            Lib = lib;
            Txt = txt;

            Functions = new List<Function>();
            Statements = new List<Statement>();
            Operations = new List<Operation>();

            Parse(txt);
        }

        public CustomClass(string identifier, string block)
        {
            Identifier = identifier;
            Block = block ?? "";
            ClassBlock = RandomString(5);
            Lib = null;
            Txt = null;

            Functions = new List<Function>();
            Statements = new List<Statement>();
        }

        private void Parse(string txt)
        {
            var sep = txt.Trim().SplitSkip( ";");

            for (int i = 0; i < sep.Count; i++)
            {
                var statFunc = sep[i].Trim();
                if (statFunc.StartsWith("$"))
                {
                    var statmentTxt = statFunc.Remove(0, 1);
                    var statment = new Statement(statmentTxt, Lib, ClassBlock);
                    Statements.Add(statment);
                }
                else
                {
                    var op = operationsStr.FirstOrDefault(x => statFunc.Substring(0, x.Length) == x);

                    if (op != null)
                    {
                        var function = new Function(statFunc, Lib, ClassBlock);
                        var operation = new Operation(op, function);
                        Operations.Add(operation);
                    }
                    else
                    {
                        var funcTxt = statFunc;
                        var func = new Function(funcTxt, Lib, ClassBlock);
                        Functions.Add(func);
                    }
                }
            }
        }

        public object Get(string str)
        {
            var find = Functions.Find(x => x.Identifier == str);
            if (find is null)
            {
                var find2 = Memory.Variables.Find(x => x.Name == str && x.Block == ClassBlock);
                return find2;
            }

            return find;
        }

        public void Assign(List<object> parameters)
        {
            Statements.ForEach(x =>
            {
                if (x.Assign)
                {
                    x.Exec();
                }
            });

            var constructor = Functions.Find(x => x.Identifier == Identifier);
            if (constructor != null)
            {
                Lib.AccessNewFunction(constructor, parameters); 
            }
        }
        

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[StaticRandom.Next(s.Length)]).ToArray());
        }

        public override string ToString()
        {
            return "Class: " + Block + "|" + ClassBlock + "|" + Identifier;
        }

        public CustomClass Clone()
        {
            var classClone = new CustomClass(Identifier, Txt, Lib, Block);

            return classClone;
        }
    }
}
