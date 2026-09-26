namespace NumberToWords.Core.Models
{
    public class NumberConvertResult
    {
        public string? Words { get; set; }
        public string? Error { get; set; }
        public bool IsSuccess => Error is null;
    }
}
