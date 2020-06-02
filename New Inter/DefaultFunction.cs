using System;
using System.Collections.Generic;
using System.Linq;

namespace New_Inter
{
    static class DefaultFunctions
    {
        public static List<Function> defaultFunctions = new List<Function>();

        static DefaultFunctions()
        {
            var print = new Function("print txt:", null);
            print.ExecFunc = o =>
            {
                var param = (List<object>)o;
                var txt = param.Any() ? param[0].ToString() : null;
                Console.Write(txt);
                return null;
            };

            var printL = new Function("printL txt:", null);
            printL.ExecFunc = o =>
            {
                var param = (List<object>)o;
                var txt = param.Any() ? param[0].ToString() : null;
                Console.WriteLine(txt);
                return null;
            };

            var mul = new Function("mul a, b:", null);
            mul.ExecFunc = o =>
            {
                var param = (List<object>)o;
                if (param.Count == 2)
                {
                    var a = (int)param[0];
                    var b = (int)param[1];

                    return a * b;
                }
                
                return null;
            };

            var div = new Function("div a, b:", null);
            div.ExecFunc = o =>
            {
                var param = (List<object>)o;
                if (param.Count == 2)
                {
                    var a = (int)param[0];
                    var b = (int)param[1];

                    return a / b;
                }

                return null;
            };

            var divR = new Function("divR a, b:", null);
            divR.ExecFunc = o =>
            {
                var param = (List<object>)o;
                if (param.Count == 2)
                {
                    var a = (int)param[0];
                    var b = (int)param[1];

                    return a % b;
                }

                return null;
            };

            var input = new Function("input a:", null);
            input.ExecFunc = o =>
            {
                var param = (List<object>)o;
                if (param.Count > 0)
                {
                    var txt = param[0].ToString();
                    Console.Write(txt);
                }

                var inputRead = Console.Read();
                

                return (char)inputRead;
            };

            var inputL = new Function("inputL a:", null);
            inputL.ExecFunc = o =>
            {
                var param = (List<object>)o;
                if (param.Count > 0)
                {
                    var txt = param[0].ToString();
                    Console.Write(txt);
                }

                var inputRead = Console.ReadLine();


                return inputRead;
            };

            defaultFunctions.Add(print);
            defaultFunctions.Add(printL);

            defaultFunctions.Add(mul);
            defaultFunctions.Add(div);
            defaultFunctions.Add(divR);

            defaultFunctions.Add(input);
            defaultFunctions.Add(inputL);
        }

        public static void Export()
        {
            Memory.Functions.AddRange(defaultFunctions);
        }
    }
}
