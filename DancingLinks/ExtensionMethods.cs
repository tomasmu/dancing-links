using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DancingLinks
{
    public static class ExtensionMethods
    {
        public static void Increment(this Dictionary<string, long> dict, string key)
        {
            if (!dict.ContainsKey(key))
                dict[key] = 0;

            dict[key]++;
        }

        public static int GetWidth(this int number) =>
            (int)(1 + Math.Log10(number));
    }
}
