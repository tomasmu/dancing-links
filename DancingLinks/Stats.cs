using System;
using System.Collections.Generic;
using System.Text;

namespace DancingLinks
{
    public static class Stats
    {
        public static Dictionary<string, long> Counters { get; set; } = new Dictionary<string, long>();
        public static Dictionary<string, object> Values { get; set; } = new Dictionary<string, object>();

        public static void IncrementCounter(string key)
        {
            if (!Counters.ContainsKey(key))
                Counters[key] = 0;

            Counters[key]++;
        }

        public static void SetValue(string key, object value)
        {
            Values[key] = value;
        }
    }
}
