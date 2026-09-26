using NumberToWords.Core.Mapping;
using NumberToWords.Core.Models;
using System.Globalization;

namespace NumberToWords.Core.Services
{
    public class NumberToWordsConverter
    {
        public NumberConvertResult Convert(string number)
        {
            //1. Validation first
            //1.1 Is input null or whitespace
            //1.2 Are commas are the right spot
            //1.3 Pad cents (front and back)
            //2. TRIM, STRIP whitespace, convert to decimal
            //4. 
            var result = new NumberConvertResult();

            var cleaned = number.Trim();
            
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                result.Error = "Invalid input. Please enter a valid number.";
                return result;
            }

            if (cleaned.EndsWith("."))
            {
                result.Error = "Invalid input. Please enter digits after the decimal point.";
                return result;
            }

            if (!AreCommasValid(cleaned))
            {
                result.Error = "Invalid input. Please ensure commas fall on every 3rd digit of the input.";
                return result;
            }

            cleaned = cleaned.Replace(",", string.Empty);

            if (!ContainsOnlyAllowedCharacters(cleaned))
            {
                result.Error = "Invalid input. Please enter a valid number.";
                return result;
            }

            if (!HasAtMostOneDecimalPoint(cleaned))
            {
                result.Error = "Invalid input. Please enter a valid number.";
                return result;
            }

            var wholeDigits = cleaned.Split('.')[0];
            var fractionDigits = cleaned.Contains('.') ? cleaned.Split('.')[1] : string.Empty;
            if (ExceedsMax(wholeDigits, fractionDigits, Scale.Scales.Length * 3))
            {
                result.Error = $"Invalid input. Please enter number does not exceed {Scale.Scales.Length * 3} digits.";
                return result;
            }

            var parseResult = decimal.TryParse(cleaned, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal parsedNumber);
            if (!parseResult)
            {
                result.Error = "Unable to parse the input, please reach out to a system administrator.";
                return result;
            }

            return new NumberConvertResult();
        }

        private bool ContainsOnlyAllowedCharacters(string input)
        {
            return input.All(c => char.IsAsciiDigit(c) || c == ',' || c == '.');
        }

        private bool HasAtMostOneDecimalPoint(string input)
        {
            return input.Count(x => x == '.') <= 1;
        }

        private bool AreCommasValid (string input)
        {
            var wholeDigits = input.Split('.')[0];
            var groups = wholeDigits.Split(',');

            return
                groups.Length == 1 ||
                (groups[0].Length is >= 1 and <= 3 &&
                 groups.Skip(1).All(g => g.Length == 3));
        }

        private bool ExceedsMax(string wholeDigits, string fractionDigits, int maxDigits)
        {
            wholeDigits = wholeDigits.TrimStart('0');
            if (wholeDigits.Length > maxDigits) return true;
            if (wholeDigits.Length < maxDigits) return false;

            bool centsRoundToNextDollar =
                fractionDigits.Length >= 3 &&
                fractionDigits[0] == '9' &&
                fractionDigits[1] == '9' &&
                fractionDigits[2] >= '5';

            return centsRoundToNextDollar && wholeDigits.All(c => c == '9');
        }
    }
}
