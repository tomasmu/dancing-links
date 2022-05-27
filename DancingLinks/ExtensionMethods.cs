using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DancingLinks
{
    public static class ExtensionMethods
    {
        public static int NumberOfDigits(this int number) =>
            (int)(1 + Math.Log10(number));
    }
}
