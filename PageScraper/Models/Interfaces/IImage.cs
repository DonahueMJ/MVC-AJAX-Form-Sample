namespace PageScraper.Models
{
    public interface IImage
    {
        int height { get; set; }
        string Title { get; set; }
        string Url { get; set; }
        int width { get; set; }
    }
}