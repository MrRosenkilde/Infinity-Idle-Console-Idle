using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace InfiniteNumbers
{ 
    public class InfinityNumber
    {
        private int Scale = 3;
        public static string Seperator { get; set; } = "e";
        private double prefix = 0; //backup to prevent infinite loop
        public double Prefix { get => prefix;
            private set {
                if (value >= 10 || value < 1 && value > 0 || value < -10)
                {
                    var log10 = (int)Math.Floor(value.GetTenthPower());
                    Postfix += log10;

                    double debug2;
                    if (log10 > 0)
                        debug2 = value / Math.Round(Math.Pow(10, log10));
                    else debug2 = value * Math.Round(Math.Pow(10, log10 * -1));
                    prefix = debug2;
                }
                else if (value == 0 && Postfix != 0)
                {
                    prefix = 1;
                    Postfix--;
                }
                else if (value == 0 && Postfix == 0) prefix = 0;
                else prefix = value;
            } 
        }
        public BigInteger Postfix { get; set; }
        public InfinityNumber(double prefix)
        {
            Postfix =  0; // Postfix adjust when prefix is set
            Prefix = prefix;
        }
        public InfinityNumber(double prefix, BigInteger postfix)
        {
            Postfix = postfix;
            Prefix = prefix;
        }
        public InfinityNumber(double prefix, BigInteger postfix,int scale)
        {
            Scale = scale;
            Postfix = postfix;
            Prefix = prefix;
        }
        //public InfinityNumber(InfinityNumber prefix,InfinityNumber postfix)
        //{
        //    //var n1 = prefix;
        //}
        public InfinityNumber() { Prefix = 0; Postfix = 0; }
        public static InfinityNumber operator + (InfinityNumber n1,InfinityNumber n2)
        {
            var biggest = n1 > n2 ? n1 : n2;
            var smallest = n1 < n2 ? n1 : n2;
            var postFixDifference = biggest.Postfix - smallest.Postfix;
            if (postFixDifference == 0)
                return new InfinityNumber(biggest.Prefix + smallest.Prefix,biggest.Postfix); //if postfix is the same, just add
            else 
                return new InfinityNumber(
                    prefix: biggest.Prefix + smallest.Prefix / Math.Pow(10, (int)postFixDifference),
                    postfix: biggest.Postfix
                );
        }
        
        public static InfinityNumber operator - (InfinityNumber n1, InfinityNumber n2)
            => n1 + new InfinityNumber(n2.Prefix * -1, n2.Postfix);
        public static InfinityNumber operator * (InfinityNumber n1, InfinityNumber n2)
         => new InfinityNumber
            (
                prefix: n1.Prefix * n2.Prefix,
                postfix: n1.Postfix + n2.Postfix
            );
        public static InfinityNumber operator / (InfinityNumber n1, InfinityNumber n2)
         => new InfinityNumber
             (
                 postfix: n1.Postfix - n2.Postfix,
                 prefix: n1.Prefix / n2.Prefix
            );
        public InfinityNumber SetScale(int scale) {
            Scale = scale;
            return this;
        }
        public static InfinityNumber operator ^(InfinityNumber n1, int pow)
        {
            var abs = n1.abs(out int scaled);
            
            var prefix = (BigInteger) (n1.Prefix * Math.Pow(10,n1.Scale));
            prefix = BigInteger.Pow(prefix, pow);
            var prefix_digits = (int) BigInteger.Log10(prefix);
            var prefix_double = (double)BigInteger.Divide(
                prefix,
                BigInteger.Pow(10, prefix_digits - n1.Scale)
            );
            return new InfinityNumber(
                prefix: prefix_double,
                postfix: n1.Postfix * pow + prefix_digits
            );
        }
        public static InfinityNumber operator ^(InfinityNumber n1, float root)
        {
            var mod = n1.Postfix % (int)root;
            var divided = n1.Postfix / (int)root;

            var downScaled = (double)new InfinityNumber(n1.Prefix, mod);
            var prefix = Math.Pow(downScaled, 1D / root);
            var result = new InfinityNumber(prefix) * new InfinityNumber(1, divided);

            return result;
        }


        //public static InfinityNumber operator ^(InfinityNumber n1, InfinityNumber n2)
        //{

        //    //using logs to  calculate the first digits and how many digits in the result, without calculating the result.
        //    double prefix;
        //    BigInteger postfix;
        //    if (n2 > 1D)
        //        prefix = FirstDigitsOfExponent(n1.Prefix, (BigInteger)(n2 * 100D));

        //    else prefix = FirstDigitsOfExponent(n1.Prefix, (double)n2);

        //    if (n1 > 1D && n2 > 1D)
        //    {
        //        // x1^x2 => 1 + x2*log10(x1) == number of digits
        //        var log = (BigInteger)(BigInteger.Log10((BigInteger)n1) * 1000000 + 1); // multiply with 10000 to lessen data loss when converting to Biginteger
        //        postfix = log * (BigInteger)n2;
        //        postfix = postfix / 1000000;
        //    }
        //    else if (n1 < 1D && n2 < 1D)
        //    {

        //        var log = (Math.Log10((double)n1) + 1); // multiply with 10000 to lessen data loss when converting to Biginteger
        //        postfix = (BigInteger)Math.Floor(log * (double)n2);
        //    }
        //    else if (n2 < 1D)
        //    {
        //        var neededTenFactor = 1D / n2;
        //        postfix = n1.Postfix / (BigInteger)neededTenFactor;
        //        //var temp = new InfinityNumber(n1.Prefix, n1.Postfix);
        //        //temp = temp * 10D ^ neededTenFactor;
        //    }

        //    return new InfinityNumber
        //        (
        //            prefix: prefix,
        //            postfix: postfix
        //        );
        //}
        public static InfinityNumber operator ++(InfinityNumber n1)
         => n1 + 1m;
        public static InfinityNumber operator -- (InfinityNumber n1)
        {
            n1.Prefix = n1.Prefix - 1;
            return n1;
        }
        public static bool operator < (InfinityNumber n1, InfinityNumber n2)
        {
            if (n1.Postfix == n2.Postfix)
                return n1.Prefix < n2.Prefix;
            else return n1.Postfix < n2.Postfix;
        }
        public static bool operator > (InfinityNumber n1, InfinityNumber n2)
        {
            if (n1.Postfix == n2.Postfix)
                return n1.Prefix > n2.Prefix;
            else return n1.Postfix > n2.Postfix;
        }
        public static bool operator ==(InfinityNumber n1, InfinityNumber n2)
            => n1.Prefix == n2.Prefix && n2.Postfix == n2.Postfix;
        public static bool operator !=(InfinityNumber n1, InfinityNumber n2)
            => ! (n1 == n2);
        public static bool operator <=(InfinityNumber n1, InfinityNumber n2)
            => (n1 == n2 || n1 < n2);
        public static bool operator >=(InfinityNumber n1, InfinityNumber n2)
            => (n1 == n2 || n1 > n2);
        public static explicit operator InfinityNumber(BigInteger n1)
        {
            if (n1 == 0) return new InfinityNumber();
            var log10 = BigInteger.Log10(n1);
            var floored = Math.Floor(log10);
            return new InfinityNumber(
                postfix: (BigInteger)floored,
                prefix: Double.Parse(BigInteger.Multiply(n1, 100).ToString().Substring(0, 3)) / 100
            );
        }
        public static explicit operator double(InfinityNumber n1)
            => n1.Prefix * Math.Pow(10, (double) n1.Postfix);
        public static explicit operator BigInteger(InfinityNumber n1)
        {
            var multiplier = (BigInteger) (n1.Prefix * 100);
            var multiplicand = BigInteger.Pow(10, (int)n1.Postfix);
            var multiplied = BigInteger.Multiply(multiplier, multiplicand);
            var adjusted = BigInteger.Divide(multiplied, 100);
            return adjusted;

        }
        public static implicit operator InfinityNumber (double n1)
            => new InfinityNumber(n1);
        public static implicit  operator InfinityNumber (decimal n1)
            => (double)n1;
        public static explicit operator InfinityNumber (string s)
            => Parse(s);
        public static implicit operator string (InfinityNumber i) => i.ToString();
        public static InfinityNumber Parse(string s)
        {
            if (s.Contains("."))
                s = s.Replace('.', ',');
            try
            {
                if (s.Contains(Seperator))
                {
                    s = s.Replace(" ", "");
                    string[] tokens = s.Split(Seperator);
                    return new InfinityNumber
                        (
                            prefix : double.Parse(tokens[0] ),
                            postfix : BigInteger.Parse(tokens[1]) 
                        );
                }
                return new InfinityNumber(double.Parse(s));
            }
            catch (FormatException) { return new InfinityNumber(); }
        }
        public override string ToString()
        {
            var s = prefix.ToString();
            
            if (s.Contains(","))
            {
                var digits = s.Substring(s.IndexOf(",")).Length -1 /*subtract comma*/;
                if (digits < Scale)
                    for (int i = Scale - digits;i>0;i--) s += "0";
                s = s.Substring(0, s.IndexOf(",") + Scale +1).Replace(",", ".");

            } 
            if (Postfix != 0)
                return s + Seperator + Postfix.ToString();
            return s; //if postfix isn't zero and prefix doesn't have comma
            
        }
        private static double FirstDigitsOfExponent(double n, double exponent)
        {
            var log = Math.Log10(n);
            var A = exponent * (Math.Log10(n) );
            var lastDigits = A % 1;
            //var power = double.Parse("0," + lastDigits.ToString()); 
            var maybeResult = Math.Pow(10,  lastDigits);
            return maybeResult;

        }
        private static double FirstDigitsOfExponent(double n, BigInteger exponent)
        {
            var exponent2 = exponent ;
            var log2 = Math.Log10(n);
            var A2 = exponent2 * (BigInteger)(Math.Log10(n) * 1000000000000000000);
            var aboveZero = A2 / BigInteger.Pow(10, 20);
            if (A2 == 0)
                return 1;

            var digitsAboveZero = aboveZero > 0 ? Math.Floor(BigInteger.Log10(aboveZero) + 1) : 0;

            var power = double.Parse("0," + A2.ToString().Substring((int)digitsAboveZero));

            var maybeResult2 = Math.Pow(10, power);

            return maybeResult2;
        }
    }
}
