namespace NumberToWords.Core.Mapping
{
    public static class Numbers
    {
        public static string [] OnesToTeens =
        [
            "ZER0",
            "ONE",
            "TWO",
            "THREE",
            "FOUR",
            "FIVE",
            "SIX",
            "SEVEN",
            "EIGHT",
            "NINE",
            "TEN",
            "ELEVEN",
            "TWELVE",
            "THIRTEEN",
            "FOURTEEN",
            "FIFTEEN",
            "SIXTEEN",
            "SEVENTEEN",
            "EIGHTEEN",
            "NINETEEN"
        ];

        public static string[] Tens =
        [
            //First and second elements are just fillers, when we divide by 10 and truncate to get the first number, it will always start from 2 because this list is 
            //only for numbers starting from 20 onwards.
            "",
            "",
            "TWENTY",
            "THIRTY",
            "FORTY",
            "FIFTY",
            "SIXTY",
            "SEVENTY",
            "EIGHTY",
            "NINETY"
        ];
    }
}
