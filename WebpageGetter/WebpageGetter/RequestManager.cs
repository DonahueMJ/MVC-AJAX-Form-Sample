using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using WebpageGetter.Models;

namespace WebpageGetter
{
    public class RequestManager
    {
        public Page GetPage(string url)
        {
            

            //get httpreq page
            var page = new Page
            {
                Url = url,
                Source = "todo returned data"
            };

            return page;
        }

        private HtmlDocument GetPageImages(string pageSource)
        {
            HtmlDocument document = new HtmlDocument();
            document.Load(pageSource);

            var ImageURLS = document.DocumentNode.Descendants("img")
                                .Select(e => e.GetAttributeValue("src", null))
                                .Where(s => !String.IsNullOrEmpty(s));
        }
    }
}
