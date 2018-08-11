using System;
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
    }
}
