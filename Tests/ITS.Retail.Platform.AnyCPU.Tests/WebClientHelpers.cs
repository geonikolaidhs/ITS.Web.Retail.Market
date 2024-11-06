using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Tests.Common;
using ITS.Retail.WebClient.Helpers;
using Xunit;
using Xunit.Extensions;

namespace ITS.Retail.Platform.Tests
{
    public class WebClientHelpers
    {

        public static IEnumerable<object[]> BridgeHelperSpecialCharacterReplacementInput
        {
            get
            {
               

                return new[]{
                    new object[]{"input","input"},
                    new object[]{"input\\t","input\t"},
                    new object[]{"input\\n","input\\n"},
                    new object[]{"input\\r","input\r"}
                };
            }
        }
        [Theory, PropertyDataAttribute("BridgeHelperSpecialCharacterReplacementInput")]
        public void BridgeHelperSpecialCharacterReplacement(string input, string expectedOutput)
        {
            Assert.Equal(expectedOutput, BridgeHelper.SpecialCharacterReplacement(input));
            
        }
    }
}
