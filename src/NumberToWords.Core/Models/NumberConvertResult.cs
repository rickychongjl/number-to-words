namespace NumberToWords.Core.Models
{
    public class NumberConvertResult
    {
        public string? Words { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public bool IsSuccess => Errors.Count < 1;
    }
}
