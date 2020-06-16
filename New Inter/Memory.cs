using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace New_Inter
{
    static class Memory
    {
        public static List<Variable> Variables;
        public static List<Function> Functions;
        public static List<CustomClass> Classes;

        static Memory()
        {
            Variables = new List<Variable>();
            Functions = new List<Function>();
            Classes = new List<CustomClass>();
            DefaultImport.Export();
        }

        public static void Reset()
        {
            Variables.Clear();
            Functions.Clear();
            Classes.Clear();
            DefaultImport.Export();
        }

        public static Function GetFunction(string indentifier)
        {
            return Functions.Find(x => x.Identifier == indentifier);
        }

        public static CustomClass GetClass(string identifier, string block)
        {
            CustomClass customClass;

            do
            {
                customClass = Classes.Find(x => x.Identifier == identifier && x.Block == block);

                if (block != null)
                {
                    if (block.Contains('/'))
                    {
                        block = block.Split('/')[0];
                    }
                    else if(block != "")
                    {
                        block = "";
                    }
                    else
                    {
                        block = null;
                    }
                }

            } while (customClass is null && !(block is null));

            return customClass;
        }

        public static Variable GetVariable(string name, string block)
        {
            Variable variable;

            do
            {
                variable = Variables.Find(x => x.Name == name && x.Block == block);

                if (block != null)
                {
                    if (block.Contains('/'))
                    {
                        block = block.Split('/')[0];
                    }
                    else if (block != "")
                    {
                        block = "";
                    }
                    else
                    {
                        block = null;
                    }
                }

            } while (variable is null && !(block is null));

            return variable;
        }

        public static void AddVariable(Variable variable)
        {
            Variables.Add(variable);
        }

        public static Variable SetVariable(string name, string block, object value)
        {
            var variable = Variables.Find(x => x.Name == name && x.Block == block);
            if (variable is null)
            {
                variable = new Variable(name, block, value);
                Variables.Add(variable);
            }
            else
            {
                variable.SetValue(value);
            }

            return variable;
        }
    }
}
