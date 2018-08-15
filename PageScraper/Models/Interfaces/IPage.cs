using System;

namespace PageScraper.Models
{
    public interface IPage
    {
        string PageUrl { get; set; }
        string Source { get; set; }
        Uri Uri { get; set; }
    }
}