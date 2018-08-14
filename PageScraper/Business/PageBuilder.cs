using HtmlAgilityPack;
using PageScraper.Models;
using PageScraper.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
                    results.Uri = new Uri(url);
                    results.Images = getImages(document, webClient, results.Uri);
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

        private List<Image> getImages(HtmlDocument pageSource, WebClient client, Uri requestUri)
        {
            var images = pageSource.DocumentNode.Descendants("img")
                                .Select(e => SetImageProperties(e, client, requestUri))
                                .Where(s => !String.IsNullOrEmpty(s.Url)).ToList();

            return images;

        }

        private Image SetImageProperties(HtmlNode image, WebClient client, Uri requestUri)
        {
            var imagePath = image.GetAttributeValue("src", null);
            var imageUrl = string.Empty;
            if (imagePath != null)
            {
                imageUrl = (imagePath.StartsWith("/") && !imagePath.StartsWith("//")) ? requestUri.GetLeftPart(UriPartial.Authority) + imagePath : imagePath;
            }
            
            byte[] imageData = client.DownloadData(imageUrl);
            MemoryStream imgStream = new MemoryStream(imageData);
            System.Drawing.Image img = System.Drawing.Image.FromStream(imgStream);

            int wSize = img.Width;
            int hSize = img.Height;
            return new Image
            {
                Url = imageUrl,
                Title = image.GetAttributeValue("alt", null),
                height = hSize,
                width = wSize
            };
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

                var styles = pageSource.DocumentNode.SelectNodes("//link");
                foreach (var style in styles)
                {
                    style.Remove();
                }

                var scripts = pageSource.DocumentNode.SelectNodes("//script");
                foreach (var script in scripts)
                {
                    script.Remove();
                }

                content = pageSource.DocumentNode.OuterHtml;

                // Remove all HTML tags, leaving on the text inside.
                content = Regex.Replace(content, "<(.| )*?>", "");
                content = Regex.Replace(content, "&amp;", "");
                content = Regex.Replace(content, "&nbsp;", "");
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