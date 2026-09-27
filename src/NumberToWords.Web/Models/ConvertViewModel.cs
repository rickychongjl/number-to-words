namespace NumberToWords.Web.Models;

public class ConvertViewModel
{
    public string? Number { get; set; }
    public string? Words { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}