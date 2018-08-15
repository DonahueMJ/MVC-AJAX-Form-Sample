using System;
using System.Collections.Generic;
using System.Text;

namespace PageScraper.Models
{
    public class Image : IImage
    {
        public string Url { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public string Title { get; set; }
    }
}
