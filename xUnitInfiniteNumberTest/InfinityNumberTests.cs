using InfiniteNumbers;
using System;
using Xunit;

namespace xUnitInfiniteNumberTest
{
    public class UnitTest1
    {
        [Theory]
        [InlineData(0, 0, 2D, 2D, 0, 2)]
        [InlineData(0, 0, -2, -2, 0, 2)]
        [InlineData(0, 0, 12D, 1.2, 1, 2)]
        [InlineData(10, 0, 10, 2, 1, 2)]
        [InlineData(100, 0, 10, 1.1, 2, 2)]
        [InlineData(1, 4, 10, 1.001, 4, 5)]
        [InlineData(5, 2, -100, 4, 2, 5)]
        
        public void Adding(double initPrefix, int initPostfix, double add, double expectedPrefix, int expectedPostfix,int scale)
        {
            var n0 = new InfinityNumber(initPrefix,initPostfix);
            n0.SetScale(scale);
            n0 += add;
            Assert.Equal(expectedPrefix, n0.Prefix);
            Assert.Equal(expectedPostfix, n0.Postfix);
        }
        [Theory]
        [InlineData(2,5,3,5,5,5)] //2e5 + 3e5 = 5e5
        [InlineData(2,5,-1,5,1,5)] //2e5 - 1e5 = 1e5
        [InlineData(2.5,5,2.5,4,2.75,5)] //2.5e5 + 2.5e4 = 2.75e5
        [InlineData(2.5,5,-2.5,4,2.25,5)] //2.5e5 - 2.5e4 = 2.25e5
        [InlineData(1,3, 0.001 ,3, 1.001,3)] //1e3 + 0.001e3 + 1.001e3
        [InlineData(1, 123456789, 1, 123456789, 2, 123456789)]
        public void AddingInfinityNumbers(double initPrefix1, int initPostfix1, double initPrefix2, int initPostfix2, double expectedPrefix, int expectedPostfix)
        {
            var n0 = new InfinityNumber(initPrefix1, initPostfix1);
            var n1 = new InfinityNumber(initPrefix2, initPostfix2);
            var result = n0 + n1;
            Assert.Equal(expectedPrefix, result.Prefix);
            Assert.Equal(expectedPostfix, result.Postfix);
        }
        [Theory]
        [InlineData(2,3,4,3,8,6)] // 2e3 * 4e3 = 8e6
        [InlineData(3,3,4,3,1.2,7)] // 2e3 * 4e3 = 8e6
        [InlineData(2,5,1,-1,2,4)] // 2e5 * 0.1 = 2e4
        [InlineData(2,5,0.1,4,2,8)] // 2e5 * 0.1e4 = 2e8
        public void MultiplyingInfinityNumbers(double initPrefix1, int initPostfix1, double initPrefix2, int initPostfix2, double expectedPrefix, int expectedPostfix)
        {
            var n0 = new InfinityNumber(initPrefix1, initPostfix1);
            var n1 = new InfinityNumber(initPrefix2, initPostfix2);
            var result = n0 * n1;
            Assert.Equal(expectedPrefix, result.Prefix);
            Assert.Equal(expectedPostfix, result.Postfix);
        }
        [Theory]
        [InlineData(5,2,2,1,3)]
        [InlineData(2,2,2,4,2)]
        [InlineData(2,2,10,2,3)]
        [InlineData(2,2,0.5,1,2)]
        [InlineData(2,2,0.1,2,1)]
        public void Multiplying(double initPrefix, int initPostfix, double multiplier, double expectedPrefix, int expectedPostfix)
        {
            var n0 = new InfinityNumber(initPrefix, initPostfix);
            var result = n0 * multiplier;
            Assert.Equal(expectedPrefix, result.Prefix);
            Assert.Equal(expectedPostfix, result.Postfix);
        }
        [Theory]
        [InlineData(1,9,2,4,5,4)]
        [InlineData(5,10,2,0,2.5,10)]
        [InlineData(1,0,10,0,1,-1)]
        public void DividingInfinityNumbers(double initPrefix1, int initPostfix1, double initPrefix2, int initPostfix2, double expectedPrefix, int expectedPostfix)
        {
            var n0 = new InfinityNumber(initPrefix1, initPostfix1);
            var n1 = new InfinityNumber(initPrefix2, initPostfix2);
            var result = n0 / n1;
            Assert.Equal(expectedPrefix, result.Prefix);
            Assert.Equal(expectedPostfix, result.Postfix);
        }
        [Fact]
        public void EmptyConstructor()
        {
            var n0 = new InfinityNumber();
            Assert.Equal(0, n0.Prefix);
            Assert.Equal(0, n0.Postfix);
        }
        [Theory]
        [InlineData(12, 0, 1.2, 1, 2)]
        [InlineData(123456789, 0, 1.235, 8, 3)] // gets rounded to 1.235 instead of 1.234
        [InlineData(123456789, 0, 1.23456789, 8, 10)]
        [InlineData(100, 2, 1, 4, 2)]
        [InlineData(100, -2, 1, 0, 2)]
        [InlineData(-12,0,-1.2,1,2)]
        [InlineData(0.01,0,1,-2,4)]
        public void Constuctor(double initPrefix, int initPostfix, double expectedPrefix, int expectedPostfix,int scale)
        {
            var n0 = new InfinityNumber(initPrefix, initPostfix,scale);
            Assert.Equal(expectedPrefix, n0.Prefix);
            Assert.Equal(expectedPostfix, n0.Postfix);
        }

        [Theory]
        [InlineData("2e3",2,3,4)]
        [InlineData("2.5e3",2.5,3,4)]
        [InlineData("0.1e3",1,2,4)]
        [InlineData("0.15e3",1.5,2,4)]
        [InlineData("20e3",2,4,4)]
        [InlineData("20",2,1,4)]
        [InlineData("2.5e123456789",2.5,123456789,2)]
        [InlineData("2e3000000",2,3000000,2)]
        public void StringConversion(string input, double expectedPrefix, int expectedPostfix,int scale)
        {
            var n0 = InfinityNumber.Parse(input);
            n0.SetScale(scale);
            Assert.Equal(expectedPrefix, n0.Prefix);
            Assert.Equal(expectedPostfix, n0.Postfix);
        }
        [Theory]
        [InlineData("3e2", 2, "9e4")]
        [InlineData("2e3000", 3000, "1.230e900903")]
        public void powers(string infinityNumber, int pow, string expectedResult)
        {
            var n0 = InfinityNumber.Parse(infinityNumber);
            var result = n0 ^ pow;
            var expected = InfinityNumber.Parse(expectedResult);
            Assert.True(expected == result," expected = " + expected + " actual = " + result);
        }
        
    }
}
