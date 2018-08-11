using PageScraper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebpageGetter;
using WebpageGetter.Models;

namespace PageScraper.ViewModels
{
    public class ScrapedResults
    {
        [Url]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Valid Url is required")]
        [DisplayName("Page Url")]
        public string RequestedUrl { get; set; }
        public Page PageData { get; set; }
        public List<Image> Images { get; set; }
        public List<WordStat> WordStats { get; set; }
    }
}