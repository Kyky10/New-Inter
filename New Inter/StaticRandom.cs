using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Inter
{
    static class StaticRandom
    {
        public static Random Random;

        static StaticRandom()
        {
            Random = new Random();
        }

        public static int Next(int i)
        {
            return Random.Next(i);
        }
    }
}
