using NumberToWords.Core.Mapping;
using NumberToWords.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace NumberToWords.Tests
{
    public class NumberToWordsConverterTests
    {
        private readonly NumberToWordsConverter _converter = new();

        //Invalid input tests
        [Theory]
        [InlineData("abc", "Invalid input. Please enter a valid number.")]
        [InlineData("@#$%^", "Invalid input. Please enter a valid number.")]
        [InlineData("", "Invalid input. Please enter a valid number.")]
        [InlineData("    ", "Invalid input. Please enter a valid number.")]
        [InlineData("1.2.3", "Invalid input. Please enter a valid number.")]
        [InlineData("+5", "Invalid input. Please enter a valid number.")]
        [InlineData("$5", "Invalid input. Please enter a valid number.")]
        [InlineData("-1", "Invalid input. Please enter a valid number.")]
        [InlineData("-0.01", "Invalid input. Please enter a valid number.")]
        [InlineData(".", "Invalid input. Please enter digits after the decimal point.")]
        [InlineData("5.", "Invalid input. Please enter digits after the decimal point.")]
        [InlineData("10,00", "Invalid input. Please ensure commas fall on every 3rd digit of the input.")]
        [InlineData("100,,000,", "Invalid input. Please ensure commas fall on every 3rd digit of the input.")]
        public void Convert_InvalidInput_ReturnsValidationErrors(string input, string expectedErrors)
        {
            var result = _converter.Convert(input);
            Assert.False(result.IsSuccess);
            Assert.Equal(expectedErrors, result.Error);
        }

        //Exceeding sextillion test
        [Fact]
        public void Convert_InputExceedsSextillion_ReturnsValidationErrors()
        {
            //Arrange
            var input = "999999999999999999999999.955";
            var errorMessage = $"Invalid input. Please enter number does not exceed {Scale.Scales.Length * 3} digits.";

            //Act
            var result = _converter.Convert(input);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(errorMessage, result.Error);
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
        [InlineData("999999.99", "NINE HUNDRED AND NINETY-NINE THOUSAND NINE HUNDRED AND NINETY-NINE DOLLARS AND NINETY-NINE CENTS")]
        public void Convert_InputWithDecimal_ReturnsCorrectlyRoundedOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
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
        [InlineData("19", "NINETEEN DOLLARS")]
        [InlineData("25", "TWENTY-FIVE DOLLARS")]
        [InlineData("100", "ONE HUNDRED DOLLARS")]
        [InlineData("101", "ONE HUNDRED AND ONE DOLLARS")]
        [InlineData("120", "ONE HUNDRED AND TWENTY DOLLARS")]
        [InlineData("1100", "ONE THOUSAND ONE HUNDRED DOLLARS")]
        [InlineData("1005", "ONE THOUSAND FIVE DOLLARS")]
        [InlineData("999", "NINE HUNDRED AND NINETY-NINE DOLLARS")]
        [InlineData("100001", "ONE HUNDRED THOUSAND AND ONE DOLLARS")]
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
        [InlineData("1,000,000,000", "ONE BILLION DOLLARS")]
        [InlineData("1,000,000,000.99", "ONE BILLION DOLLARS AND NINETY-NINE CENTS")]
        [InlineData("1,000,000,000,000", "ONE TRILLION DOLLARS")]
        [InlineData("4,307,052,916,480,017.63", "FOUR QUADRILLION THREE HUNDRED AND SEVEN TRILLION FIFTY-TWO BILLION NINE HUNDRED AND SIXTEEN MILLION FOUR HUNDRED AND EIGHTY THOUSAND AND SEVENTEEN DOLLARS AND SIXTY-THREE CENTS")]
        public void Convert_EdgeCasesInput_ReturnsCorrectOutput(string input, string expectedOutput)
        {
            var result = _converter.Convert(input);
            Assert.True(result.IsSuccess);
            Assert.Equal(expectedOutput, result.Words);
        }
    }
}
