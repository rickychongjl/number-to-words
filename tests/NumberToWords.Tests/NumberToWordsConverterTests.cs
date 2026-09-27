using NumberToWords.Core.Mapping;
using NumberToWords.Core.Services;

namespace NumberToWords.Tests
{
    public class NumberToWordsConverterTests
    {
        private readonly NumberToWordsConverter _converter = new();

        //Invalid input tests
        [Theory]
        [InlineData(null, "Please enter a valid number.")]
        [InlineData("abc", "Please ensure only numerical values, ',' and '.' in the input.")]
        [InlineData("@#$%^", "Please ensure only numerical values, ',' and '.' in the input.")]
        [InlineData("", "Please enter a valid number.")]
        [InlineData("    ", "Please enter a valid number.")]
        [InlineData("1.2.3", "Please ensure input only has at most 1 decimal point.")]
        [InlineData("1.2,3", "Please ensure no commas are used after the decimal point.")]
        [InlineData("+5", "Please ensure number does not include any symbols.")]
        [InlineData("$5", "Please ensure only numerical values, ',' and '.' in the input.")]
        [InlineData("-1", "Please ensure number does not include any symbols.")]
        [InlineData("-0.01", "Please ensure number does not include any symbols.")]
        [InlineData(".", "Please enter digits after the decimal point.")]
        [InlineData("5.", "Please enter digits after the decimal point.")]
        [InlineData("10,00", "Please ensure commas are used after every 3rd digit of the input from the left to right.")]
        [InlineData("100,,000,", "Please ensure commas are used after every 3rd digit of the input from the left to right.")]
        public void Convert_InvalidInput_ReturnsValidationErrors(string input, string expectedErrors)
        {
            var result = _converter.Convert(input);
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedErrors, result.Errors);
        }

        //Rounding tests
        [Theory]
        [InlineData("0.01", "ONE CENT")]
        [InlineData("0.005", "ONE CENT")]
        [InlineData("0.125", "THIRTEEN CENTS")]
        [InlineData("1.5", "ONE DOLLAR AND FIFTY CENTS")]
        [InlineData("1.50", "ONE DOLLAR AND FIFTY CENTS")]
        [InlineData("0.999", "ONE DOLLAR")]
        [InlineData("0.004", "ZERO DOLLARS")]
        [InlineData("0.004999999999999999999999999999", "ZERO DOLLARS")]
        [InlineData("0.005999999999999999999999999999", "ONE CENT")]
        [InlineData("999999.99", "NINE HUNDRED AND NINETY-NINE THOUSAND NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
        public void Convert_InputWithDecimal_ReturnsCorrectlyRoundedOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }

        //Multiple errors in input, all returned
        [Fact]
        public void Convert_InputWithMultipleProblems_ReturnsAllErrors()
        {
            var result = _converter.Convert("-+#abc1,0.1,1.5");
            Assert.Equal(5, result.Errors.Count);
        }

        //Exceeding sextillion test
        [Fact]
        public void Convert_InputExceedsSextillion_ReturnsValidationErrors()
        {
            //Arrange
            var input = "999999999999999999999999.995";
            var errorMessage = $"Please ensure number does not exceed {Scale.Scales.Length * 3} digits. Note that the input is evaluated after a two decimal half round up.";

            //Act
            var result = _converter.Convert(input);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(errorMessage, result.Errors);
        }

        //Exceeding sextillion test but with input at 29 characters which decimal.TryParse cannot handle accurately when rounding.
        [Fact]
        public void Convert_InputWithDecimalsExceeding_ExceedsMax_ReturnsError()
        {
            //Arrange
            /*Fail*/
            var input = "999,999,999,999,999,999,999,999.99599";
            var expectedValidationError = $"Please ensure number does not exceed {Scale.Scales.Length * 3} digits. Note that the input is evaluated after a two decimal half round up.";

            //Act
            var result = _converter.Convert(input);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedValidationError, result.Errors);
        }

        //Arbitrary large number that exceeds sextillion test
        [Fact]
        public void Convert_InputWith50Digits_ExceedsMax_ReturnsError()
        {
            //Arrange
            var input = "1,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000,000";
            var expectedValidationError = $"Please ensure number does not exceed {Scale.Scales.Length * 3} digits. Note that the input is evaluated after a two decimal half round up.";

            //Act
            var result = _converter.Convert(input);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(expectedValidationError, result.Errors);
        }

        //Commas tests
        [Theory]
        [InlineData("1,000", "ONE THOUSAND DOLLARS")]
        [InlineData("1,000,000", "ONE MILLION DOLLARS")]
        public void Convert_InputWithCommas_ReturnsCorrectOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }

        //Dashes tests
        [Theory]
        [InlineData("4.35", "FOUR DOLLARS AND THIRTY-FIVE CENTS")]
        [InlineData("96.01", "NINETY-SIX DOLLARS AND ONE CENT")]
        [InlineData("56.21", "FIFTY-SIX DOLLARS AND TWENTY-ONE CENTS")]
        [InlineData("123456.21", "ONE HUNDRED AND TWENTY-THREE THOUSAND FOUR HUNDRED AND FIFTY-SIX DOLLARS AND TWENTY-ONE CENTS")]
        public void Convert_Input_ReturnsCorrectDashes(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }

        //Pluralization tests
        [Theory]
        [InlineData("0.1", "TEN CENTS")]
        [InlineData("0.01", "ONE CENT")]
        [InlineData("1.35", "ONE DOLLAR AND THIRTY-FIVE CENTS")]
        [InlineData("46.01", "FORTY-SIX DOLLARS AND ONE CENT")]
        [InlineData("56.21", "FIFTY-SIX DOLLARS AND TWENTY-ONE CENTS")]
        public void Convert_Input_ReturnsCorrectPluralisation(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }

        //Normal tests
        [Theory]
        [InlineData(" 1 ", "ONE DOLLAR")]
        [InlineData("1", "ONE DOLLAR")]
        [InlineData("10", "TEN DOLLARS")]
        [InlineData("19", "NINETEEN DOLLARS")]
        [InlineData("115", "ONE HUNDRED AND FIFTEEN DOLLARS")]
        [InlineData("25", "TWENTY-FIVE DOLLARS")]
        [InlineData("5.00", "FIVE DOLLARS")]
        [InlineData("19.25", "NINETEEN DOLLARS AND TWENTY-FIVE CENTS")]
        [InlineData("00.25", "TWENTY-FIVE CENTS")]
        [InlineData("100", "ONE HUNDRED DOLLARS")]
        [InlineData("101", "ONE HUNDRED AND ONE DOLLARS")]
        [InlineData("120", "ONE HUNDRED AND TWENTY DOLLARS")]
        [InlineData("1100", "ONE THOUSAND ONE HUNDRED DOLLARS")]
        [InlineData("1005", "ONE THOUSAND AND FIVE DOLLARS")]
        [InlineData("1013", "ONE THOUSAND AND THIRTEEN DOLLARS")]
        [InlineData("999", "NINE HUNDRED AND NINETY-NINE DOLLARS")]
        [InlineData("100001", "ONE HUNDRED THOUSAND AND ONE DOLLARS")]
        [InlineData("1000001", "ONE MILLION AND ONE DOLLARS")]
        [InlineData("1001000", "ONE MILLION ONE THOUSAND DOLLARS")]
        [InlineData("1000100", "ONE MILLION ONE HUNDRED DOLLARS")]
        [InlineData("999999.99", "NINE HUNDRED AND NINETY-NINE THOUSAND NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
        [InlineData("999999.991", "NINE HUNDRED AND NINETY-NINE THOUSAND NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
        [InlineData("999999.995", "ONE MILLION DOLLARS")]
        public void Convert_Input_ReturnsCorrectOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }

        //Cents tests
        [Theory]
        [InlineData("1.004", "ONE DOLLAR")]
        [InlineData(".1", "TEN CENTS")]
        [InlineData(".01", "ONE CENT")]
        [InlineData(".30", "THIRTY CENTS")]
        [InlineData(".355", "THIRTY-SIX CENTS")]
        [InlineData("0.99", "NINETY-NINE CENTS")]
        [InlineData("0.11", "ELEVEN CENTS")]
        public void Convert_InputWithJustCents_ReturnsCorrectOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }

        //Edge cases tests
        [Theory]
        [InlineData("0", "ZERO DOLLARS")]
        [InlineData("0.00", "ZERO DOLLARS")]
        [InlineData("007", "SEVEN DOLLARS")]
        [InlineData("1,000,000,000", "ONE BILLION DOLLARS")]
        [InlineData("1,000,000,000.99", "ONE BILLION DOLLARS AND NINETY-NINE CENTS")]
        [InlineData("1,000,000,000,000", "ONE TRILLION DOLLARS")]
        [InlineData("4,307,052,916,480,017.63", "FOUR QUADRILLION THREE HUNDRED AND SEVEN TRILLION FIFTY-TWO BILLION NINE HUNDRED AND SIXTEEN MILLION FOUR HUNDRED AND EIGHTY THOUSAND AND SEVENTEEN DOLLARS AND SIXTY-THREE CENTS")]
        [InlineData("1,000,000,000,000,000,000,000", "ONE SEXTILLION DOLLARS")]
        [InlineData("999,999,999,999,999,999,999,999.99", "NINE HUNDRED AND NINETY-NINE SEXTILLION NINE HUNDRED AND NINETY-NINE QUINTILLION NINE HUNDRED AND NINETY-NINE QUADRILLION NINE HUNDRED AND NINETY-NINE TRILLION NINE HUNDRED AND NINETY-NINE BILLION NINE HUNDRED AND NINETY-NINE MILLION NINE HUNDRED AND NINETY-NINE THOUSAND NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
        [InlineData("999,999,999,999,999,999,999,999.99499", "NINE HUNDRED AND NINETY-NINE SEXTILLION NINE HUNDRED AND NINETY-NINE QUINTILLION NINE HUNDRED AND NINETY-NINE QUADRILLION NINE HUNDRED AND NINETY-NINE TRILLION NINE HUNDRED AND NINETY-NINE BILLION NINE HUNDRED AND NINETY-NINE MILLION NINE HUNDRED AND NINETY-NINE THOUSAND NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
        public void Convert_EdgeCasesInput_ReturnsCorrectOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }
    }
}
