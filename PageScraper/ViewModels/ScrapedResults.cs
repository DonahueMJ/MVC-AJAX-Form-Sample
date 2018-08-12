using PageScraper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PageScraper.ViewModels
{
    [MetadataType(typeof(PageMetadata))]
    public class ScrapedResults : Page
    {
        public IEnumerable<Image> Images { get; set; }
        public IEnumerable<WordStat> WordStats { get; set; }
        public string ErrorMessage { get; set; }
        public int WordCount { get; set; }
    }



    internal class PageMetadata
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Valid Url is required")]
        [DisplayName("Page Url")]
        public string PageUrl { get; set; }
    }

}