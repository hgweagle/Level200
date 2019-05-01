using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Xunit.Extensions;

namespace StorePromotion.UnitTest
{
    public class CodeUnitTest
    {
        [Theory]
        [InlineData(25, 2019, 5, 1, 25, 'a', "TUdbYF9Ha")]
        [InlineData(106, 2019, 5, 1, 7896, 'V', "QutQN1R9V")]
        [InlineData(1, 2019, 5, 1, 1, 'p', "XG5qb25up")]
        [InlineData(200, 2019, 5, 1, 10000, 'c', "T5pdO2FSc")]
        [InlineData(201, 2019, 5, 1, 10000, 'c', "")]
        [InlineData(101, 2019, 5, 1, 10001, 'c', "")]
        [InlineData(101, 2018, 5, 1, 9001, 'c', "")]
        [InlineData(101, 2019, 13, 1, 9001, 'c', "")]
        [InlineData(101, 2019, 12, 32, 9001, 'c', "")]
        public void CanGenerateProCode(byte storeId, ushort year, byte month, byte day, ushort transactionId, char randomChar, string expectedResult)
        {
            var proCode = Program.GeneratePromotionCode(storeId, year, month, day, transactionId, randomChar);
            Xunit.Assert.Equal(proCode, expectedResult);
        }
    }
}
