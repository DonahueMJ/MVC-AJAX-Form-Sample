using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PageScraper.Business;
using PageScraper.ViewModels;

namespace PageScrapeTests
{
    [TestClass]
    public class PageScrapeTest
    {
        [TestMethod]
        public void Test_DoesTakeValidUrl()
        {
            string url = "https://www.fallsdigital.com";

            var builder = new PageBuilder();

            ScrapedResults results = builder.GetDataFromUrl(url);

            Assert.IsTrue(results.WordCount > 0);

        }
    }
}
