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

                    var wordStats = getWordStats(document);
                    var wordCount = wordStats.Sum(stat => stat.Count);

                    results.PageUrl = url;
                    results.Images = getImages(document);
                    results.WordStats = wordStats.OrderByDescending(w => w.Count).Take(Constants.GeneralSettings.TopWordResultQty);
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

        private IEnumerable<Image> getImages(HtmlDocument pageSource)
        {
            var images = pageSource.DocumentNode.Descendants("img")
                                .Select(e => new Image { Url = e.GetAttributeValue("src", null), Title = e.GetAttributeValue("alt", null) })
                                .Where(s => !String.IsNullOrEmpty(s.Url));

            return images;

        }


        private IEnumerable<WordStat> getWordStats(HtmlDocument pageSource)
        {
            var words = getWords(pageSource)
                .Split(' ')
                .GroupBy(x => x)
                .Select(x => new WordStat
                {
                    Name = x.Key,
                    Count = x.Count()
                });

            return words;

        }

        private string getWords(HtmlDocument pageSource)
        {
            var root = pageSource.DocumentNode;
            string content = "";
            foreach (var node in root.SelectNodes("//text()"))
            {
                if (!node.HasChildNodes)
                {
                    string text = node.InnerText;
                    if (!string.IsNullOrEmpty(text))
                        content += text.Trim() + " ";
                }
            }

            return cleanString(content.Trim());
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