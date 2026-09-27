namespace NumberToWords.Core.Mapping
{
    public static class Scale
    {
        public static string [] Scales =
        [
            //First is just a filler, if we have more than 2 groups of 3 digits, thousand only starts on the rightmost 2nd group
            "",
            "THOUSAND",
            "MILLION",
            "BILLION",
            "TRILLION",
            "QUADRILLION",
            "QUINTILLION",
            "SEXTILLION"
        ];
    }
}
