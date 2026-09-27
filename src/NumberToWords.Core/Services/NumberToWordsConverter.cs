using NumberToWords.Core.Mapping;
using NumberToWords.Core.Models;
using System.Globalization;

namespace NumberToWords.Core.Services
{
    public class NumberToWordsConverter
    {
        public NumberConvertResult Convert(string number)
        {
            var result = new NumberConvertResult();

            var cleaned = number?.Trim() ?? "";
            var parts = cleaned.Split('.');
            var wholeDigits = parts[0];
            var fractionDigits = parts.Length > 1 ? parts[1] : "";

            wholeDigits = wholeDigits.Replace(",", "");
            fractionDigits = fractionDigits.Replace(",", "");

            var validationErrors = Validate(number, wholeDigits, fractionDigits);
            if (validationErrors.Count > 0)
            {
                result.Errors = validationErrors;
                return result;
            }

            var trimmedFraction = fractionDigits.Length > 3 ? fractionDigits.Substring(0, 3) : fractionDigits;
            var toParse = $"{wholeDigits}.{trimmedFraction}";
            
            var parseResult = decimal.TryParse(toParse, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal parsedNumber);
            if (!parseResult)
            {
                result.Errors.Add("Unable to parse the input, please reach out to a system administrator.");
                return result;
            }

            var roundedUpParsedNumber = Math.Round(parsedNumber, 2, MidpointRounding.AwayFromZero);

            var wholeNumberPart = Math.Truncate(roundedUpParsedNumber);
            var fractionalPart = roundedUpParsedNumber - wholeNumberPart;

            var wholeNumberWords = ConvertWholeNumber(wholeNumberPart);
            var fractionalPartWords = ConvertFractionalPart(fractionalPart);

            result.Words = (wholeNumberWords, fractionalPartWords) switch
            {
                ("", "") =>  "ZERO DOLLARS",
                ("", _) => $"{fractionalPartWords}",
                (_, "") => $"{wholeNumberWords}",
                _ => $"{wholeNumberWords} AND {fractionalPartWords}"
            };

            return result;
        }

        private List<string> Validate(string number, string wholeDigits, string fractionDigits)
        {
            var validationResults = new List<string>();

            if (string.IsNullOrWhiteSpace(number))
            {
                validationResults.Add("Please enter a valid number.");
                return validationResults;
            }

            var cleaned = number.Trim();

            if (cleaned.EndsWith("."))
            {
                validationResults.Add("Please enter digits after the decimal point.");
            }

            if (!HasAtMostOneDecimalPoint(cleaned))
            {
                validationResults.Add("Please ensure input only has at most 1 decimal point.");
            }

            if (!AreCommasValidInWholeNumber(cleaned))
            {
                validationResults.Add("Please ensure commas are used after every 3rd digit of the input from the left to right.");
            }

            if (!CommasExistsAfterDecimal(cleaned))
            {
                validationResults.Add("Please ensure no commas are used after the decimal point.");
            }

            cleaned = cleaned.Replace(",", string.Empty);

            if (cleaned.StartsWith("+") || cleaned.StartsWith("-"))
            {
                validationResults.Add("Please ensure number does not include any symbols.");
            }

            if (!ContainsOnlyAllowedCharacters(cleaned))
            {
                validationResults.Add("Please ensure only numerical values, ',' and '.' in the input.");
                return validationResults;
            }

            if (ExceedsMax(wholeDigits, fractionDigits, Scale.Scales.Length * 3))
            {
                validationResults.Add($"Please ensure number does not exceed {Scale.Scales.Length * 3} digits. Note that the input is evaluated after a two decimal half round up.");
                return validationResults;
            }

            return validationResults;
        }

        private bool ContainsOnlyAllowedCharacters(string input)
        {
            return input.All(c => char.IsAsciiDigit(c) || c == ',' || c == '.');
        }

        private bool HasAtMostOneDecimalPoint(string input)
        {
            return input.Count(x => x == '.') <= 1;
        }

        private bool AreCommasValidInWholeNumber(string input)
        {
            var number = input.Split('.');
            var wholeDigits = number[0];
            var groups = wholeDigits.Split(',');

            return
                groups.Length == 1 ||
                (groups[0].Length is >= 1 and <= 3 &&
                 groups.Skip(1).All(g => g.Length == 3));
        }

        private bool CommasExistsAfterDecimal(string input)
        {
            var parts = input.Split('.');
            if (parts.Length > 1 && parts[1].Contains(',')) return false;

            return true;
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

        //Converting the whole number (excluding the decimal parts)
        private string ConvertWholeNumber(decimal wholeNumber)
        {
            var groups = SplitIntoGroups(wholeNumber);
            //Because figuring out where to add trailing spaces is an absolute waste of brain cells
            var wordsToAdd = new List<string>();
            var pluralize = wholeNumber != 1;

            //Groups are back to front because we split from right, so start from the end of the list
            for (int i = groups.Count - 1; i >= 0; --i)
            {
                var group = groups[i];
                var scaleTerm = Scale.Scales[i];
                
                if (group > 0 && group < 100)
                {
                    if (i == 0 && groups.Count != 1)
                    {
                        wordsToAdd.Add("AND");
                    }
                    wordsToAdd.Add(UnderHundredToWords(group));
                }
                else if (group > 0 && group >= 100)
                {
                    //Grab the first digit first, and then process the rest of it using UnderHundredToWords();
                    var firstDigit = (int)Math.Truncate((decimal)group / 100);
                    var remainingDigits = group % 100;
                    wordsToAdd.Add(
                        remainingDigits == 0
                        ? $"{Numbers.OnesToTeens[firstDigit]} HUNDRED"
                        : $"{Numbers.OnesToTeens[firstDigit]} HUNDRED AND {UnderHundredToWords(remainingDigits)}"
                    );
                }

                //end of group processing, add scale term if not empty
                if (group > 0 && !string.IsNullOrEmpty(scaleTerm))
                {
                    wordsToAdd.Add($"{scaleTerm}");
                }

                if (i == 0)
                {
                    var dollar = pluralize ? "DOLLARS" : "DOLLAR";
                    wordsToAdd.Add($"{dollar}");
                }
            }

            return string.Join(" ", wordsToAdd);
        }

        private List<int> SplitIntoGroups(decimal number)
        {
            var groups = new List<int>();
            while (number > 0)
            {
                var last3Digits = number % 1000;
                groups.Add((int)last3Digits);
                number = Math.Truncate(number / 1000);
            }
            return groups;
        }

        private string UnderHundredToWords(int number)
        {
            var output = string.Empty;
            if (number < 20)
            {
                output += $"{Numbers.OnesToTeens[number]}";
            }
            else
            {
                var lastDigit = number % 10;
                if (lastDigit == 0)
                {
                    var firstDigit = (int)Math.Truncate((decimal)number / 10);
                    var tensWord = Numbers.Tens[firstDigit];
                    output += $"{tensWord}";
                }
                else
                {
                    var firstDigit = (int)Math.Truncate((decimal)number / 10);
                    var tensWord = Numbers.Tens[firstDigit];
                    output += $"{tensWord}-{Numbers.OnesToTeens[number % 10]}";
                }
            }
            return output;
        }

        private string ConvertFractionalPart(decimal fractionalPart)
        {
            var cents = (int)Math.Round(fractionalPart * 100, MidpointRounding.AwayFromZero);
            if (cents == 0)
            {
                return string.Empty;
            }
            var pluralize = cents != 1;
            var centsWord = pluralize ? "CENTS" : "CENT";
            return $"{UnderHundredToWords(cents)} {centsWord}";
        }
    }
}
