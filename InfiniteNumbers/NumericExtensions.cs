using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InfiniteNumbers
{
    public static class IntExtensions
    {
        public static int GetTenthPower(this int n1) => (int)Math.Floor( Math.Log10(n1) );
        public static double GetTenthPower(this double n1) => Math.Log10(Math.Abs(n1));

        public static decimal GetTenthPower(this decimal n1) => (decimal)Math.Log10((double)n1);
        public static InfinityNumber Clone(this InfinityNumber n1) => new InfinityNumber(n1.Prefix, n1.Postfix);
    }
}
