using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InfiniteNumbers
{
    public static class InfinityNumbersExtension
    {
        public static BigInteger abs(this InfinityNumber n1) {
            var s = n1.Prefix.ToString();
            var splitted = s.Split(",");
            var decimals = splitted[1];
            var abs = n1.Prefix * Math.Pow(10,decimals.Length);
            //var doublsAbs = n1.Prefix * Math.Pow(10, n1.Prefix.ToString().Split(".")[1].Length);
            return new BigInteger(abs);
            //return new BigInteger(Math.Abs(n1.Prefix));
        }
        public static BigInteger abs(this InfinityNumber n1, out int scaled) {
            var s = n1.Prefix.ToString();
            if (s.Contains(","))
            {
                var splitted = s.Split(",");
                var decimals = splitted[1];
                var abs = n1.Prefix * Math.Pow(10, decimals.Length);
                scaled = decimals.Length;
                return new BigInteger(abs);
            }
            else {
                scaled = 0;
                return new BigInteger(n1.Prefix);
            }
        }
        public static InfinityNumber rt(this InfinityNumber n1, float root)
        {
            var mod = n1.Postfix % (int)root;
            var divided = n1.Postfix / (int)root;

            var downScaled = (double) new InfinityNumber(n1.Prefix, mod);
            var prefix = Math.Pow(downScaled, 1D / root);
            var result = new InfinityNumber(prefix) * new InfinityNumber(1,divided);

            return result;
        }
    }
}
