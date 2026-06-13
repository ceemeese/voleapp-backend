namespace Application.Abstractions.Options;

public class UrlOptions
{
    public const string SectionName = "Url";
    public string FrontendUrl { get; set; } = string.Empty;
}