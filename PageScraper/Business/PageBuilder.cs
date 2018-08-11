using PageScraper.Models;
using PageScraper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebpageGetter;
using WebpageGetter.Models;

namespace PageScraper.Business
{
    public class PageBuilder
    {
        public ScrapedResults GetDataFromUrl(string url)
        {
            var manager = new RequestManager();

            var page = manager.GetPage(url);

            var results = new ScrapedResults
            {
                Images = getImages(page),
                WordStats = getWordStats(page),
                PageData = getPageData(page)
            };

            return results;
        }


        private Page getPageData(Page page)
        {
            page.Url = "http://www.test.com";

            return page;
        }

        private List<Image> getImages(Page page)
        {
            return new List<Image>();
        }

        private List<WordStat> getWordStats(Page page)
        {
            return new List<WordStat>();
        }
    }

}