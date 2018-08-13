using HtmlAgilityPack;
using PageScraper.Models;
using PageScraper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace PageScraper.Business
{
    public class PageBuilder
    {
        public ScrapedResults GetDataFromUrl(string url)
        {
            var results = new ScrapedResults();

            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string source = webClient.DownloadString(url);

                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(source);
                    results.Images = getImages(document);
                    var wordStats = getWordStats(document);
                    var wordCount = wordStats.Sum(stat => stat.Count);

                    results.PageUrl = url;
                    
                    results.WordStats = wordStats.OrderByDescending(w => w.Count).Take(Constants.GeneralSettings.TopWordResultQty).ToList();
                    results.Source = source;
                    results.WordCount = wordCount;
                }
            }
            catch(Exception ex)
            {
                var message = ex.Message;
                results.ErrorMessage = Constants.GeneralSettings.ScraperErrorMessage;
                //TODO:IncorporateLogging
            }
            

            return results;
        }

        private List<Image> getImages(HtmlDocument pageSource)
        {
            var images = pageSource.DocumentNode.Descendants("img")
                                .Select(e => new Image { Url = e.GetAttributeValue("src", null), Title = e.GetAttributeValue("alt", null) })
                                .Where(s => !String.IsNullOrEmpty(s.Url)).ToList();

            return images;

        }


        private List<WordStat> getWordStats(HtmlDocument pageSource)
        {
            var words = getWords(pageSource)
                .Split(' ')
                .GroupBy(x => x)
                .Select(x => new WordStat
                {
                    Name = x.Key,
                    Count = x.Count()
                }).ToList();

            return words;

        }

        private string getWords(HtmlDocument pageSource)
        {
           
            string content = cleanSource(pageSource);

            return cleanString(content.Trim().ToLower());
        }

        private string cleanSource(HtmlDocument pageSource)
        {
            var content = string.Empty;
            try
            {
                var imgs = pageSource.DocumentNode.SelectNodes("//img");
                foreach (var img in imgs)
                {
                    img.Remove();
                }

                content = pageSource.DocumentNode.OuterHtml;

                //clean opening html tags
                //Remove CSS styles, if any found
                content = Regex.Replace(content, "<style(.| )*?>*</style>", "");
                //Remove script blocks
                content = Regex.Replace(content, @"<script[^>]*>[\s\S]*?</script>", "");
                // Remove all images
                // Remove all HTML tags, leaving on the text inside.
                content = Regex.Replace(content, "<(.| )*?>", "");
                content = Regex.Replace(content, "&amp;", "");
                content = Regex.Replace(content, @"[^a-zA-Z\s]+", "");
                // Remove all extra spaces, tabs and repeated line-breaks
                content = Regex.Replace(content, "(x09)?", "");
                content = Regex.Replace(content, "(x20){2,}", " ");
                content = Regex.Replace(content, "(x0Dx0A)+", " ");
                content = Regex.Replace(content, @"\s+", " ");

            }
            catch(Exception ex)
            {
                var text = ex.Message;
                //log ex
                throw;
            }
            
            

            return content;
        }
        private string cleanString(string content)
        {
            //clean opening html tags and closing html tags seperately maybe?
            //keep spaces


            //characters
            //content = Regex.Replace(content, @"[^a-zA-Z0-9_.]+", "");
            //remaining tags, mostly scripts and css
            //content = Regex.Replace(content, @"<[^>]*>", "");

            return content;
        }

    }

}