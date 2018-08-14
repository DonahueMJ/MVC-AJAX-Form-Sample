using System;
using System.Collections.Generic;
using System.Text;

namespace PageScraper.Models
{
    public class Page
    {
        public string PageUrl { get; set; }
        public Uri Uri { get; set; }
        public string Source { get; set; }
    }
}
