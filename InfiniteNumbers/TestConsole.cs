using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InfiniteNumbers
{
    public class TestConsole
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(BigInteger.Pow(2, 3000));
            Console.WriteLine("Log10 = " + BigInteger.Log10(BigInteger.Pow(2, 3000)));
        }
    }
}
