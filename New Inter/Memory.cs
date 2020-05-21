using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    static class Memory
    {
        public static List<byte> MemoryBytes;
        public static List<Variable> Variables;
        public static List<Function> Functions; 

        static Memory()
        {
            MemoryBytes = new List<byte>();
            Variables = new List<Variable>();
            Functions = new List<Function>();
        }

        public static void Reset()
        {
            MemoryBytes.Clear();
            Variables.Clear();
            Functions.Clear();
        }

        public static Function GetFunction(string indentifier)
        {
            return Functions.Find(x => x.Indentifier == indentifier);
        }

        public static Variable GetVariable(string name, string block)
        {
            Variable variable = null;

            do
            {
                variable = Variables.Find(x => x.Name == name && x.Block == block);

                block = block.Split('/')[0];

                if (block == "")
                {
                    block = null;
                }

            } while (variable is null);

            return variable;
        }

        public static Variable ResetVariable(string name, string block, int value)
        {
            var f = Variables.FindIndex(x => x.Name == name && x.Block == block);
            if (f > -1)
            {
                var variable = Variables[f];
                SetPointer(variable.Address.Index, value);

                variable.Type = value.GetType();
                variable.Address.Length = 4;
                Variables[f] = variable;
                return variable;
            }

            return null;
        }

        public static Variable ResetVariable(string name, string block, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var f = Variables.FindIndex(x => x.Name == name && x.Block == block);
            if (f > -1)
            {
                var variable = Variables[f];

                if (variable.Address.Length >= bytes.Length)
                {
                    SetPointer(variable.Address.Index, value);

                    variable.Address.Length = bytes.Length;
                    Variables[f] = new Variable(name, block, variable.Address, value.GetType());
                    return variable;
                }
                else
                {
                    Variables.RemoveAt(f);
                    return SetVariable(name, value, block);
                }
            }

            return null;
        }

        public static Variable SetVariable(string name, string block, int value)
        {
            var f = Variables.FindIndex(x => x.Name == name && x.Block == block);
            if (f > -1)
            {
                return null;
            }

            var memoryAdd = GetFree(4);
            var variable1 = new Variable(name, block, memoryAdd, value.GetType());

            SetPointer(memoryAdd.Index, value);
            Variables.Add(variable1);

            return variable1;
        }

        public static Variable SetVariable(string name, string block, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);

            var f = Variables.FindIndex(x => x.Name == name && x.Block == block);
            if (f > -1)
            {
                return null;
            }

            var memoryAdd = GetFree(bytes.Length);
            var variable1 = new Variable(name, block, memoryAdd, value.GetType());

            SetPointer(memoryAdd.Index, value);
            Variables.Add(variable1);

            return variable1;
        }

        public static bool IsFree(int p, int len)
        {
            foreach (var oc in Variables)
            {
                var overlap = p < oc.Address.Index + oc.Address.Length && oc.Address.Index < p + len;
                if (overlap)
                {
                    return false;
                }
            }

            return true;
        }

        public static Address GetFree(int len)
        {
            var i = 0;
            var free = false;

            do
            {
                free = IsFree(i, len);
                i++;

            } while (!free);

            i--;

            return new Address(i, len);
        }

        public static void SetPointer(int p, int value)
        {
            if (MemoryBytes.Count < p + 4)
            {
                MemoryBytes.AddRange(new byte[p + 4 - MemoryBytes.Count]);
            }

            var bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                MemoryBytes[i + p] = bytes[i];
            }
        }

        public static void SetPointer(int p, string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            
            if (MemoryBytes.Count < p + 4 + valueBytes.Length)
            {
                MemoryBytes.AddRange(new byte[p + 4 + valueBytes.Length - MemoryBytes.Count]);
            }

            var lenBytes = BitConverter.GetBytes(valueBytes.Length);

            var bytes = lenBytes.Concat(valueBytes).ToArray();

            for (int i = 0; i < bytes.Length; i++)
            {
                MemoryBytes[i + p] = bytes[i];
            }
        }

        public static int GetInt(int p)
        {
            var bytes = MemoryBytes.GetRange(p, 4).ToArray();
            var i = BitConverter.ToInt32(bytes, 0);

            return i;
        }

        public static string GetString(int p)
        {
            var strLenBytes = MemoryBytes.GetRange(p, 4).ToArray();
            var strLen = BitConverter.ToInt32(strLenBytes, 0);

            var strBytes = MemoryBytes.GetRange(p + 4, strLen).ToArray();

            var str = Encoding.UTF8.GetString(strBytes);

            return str;
        }
    }



    class Address
    {
        public Address()
        {
            Index = -1;
            Length = -1;
        }

        public Address(int i, int l)
        {
            Index = i;
            Length = l;
        }

        public int Index;
        public int Length;
    }

    class Variable
    {
        public Variable(string name, string block,  Address address, Type type)
        {
            Name = name;
            Address = address;
            Type = type;
            Block = block;
        }

        public object GetValue()
        {
            if (Type == typeof(string))
            {
                return Memory.GetString(Address.Index);
            }

            if (Type == typeof(int))
            {
                return Memory.GetInt(Address.Index);
            }

            return null;
        }

        public void SetValue(int i)
        {
            Memory.ResetVariable(Name, Block, i);
        }

        public void SetValue(string s)
        {
            Memory.ResetVariable(Name, Block, s);
        }

        public string Name;
        public Address Address;
        public Type Type;
        public string Block;
    }
}
