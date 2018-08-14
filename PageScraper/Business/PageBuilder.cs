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
                                .Select(i => SetImageProperties(i, client, requestUri))
                                .Where(i => !String.IsNullOrEmpty(i?.Url)).ToList();

            return images;

        }

        private Image SetImageProperties(HtmlNode image, WebClient client, Uri requestUri)
        {
            var imagePath = image.GetAttributeValue("src", null);
            var imageUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                imageUrl = (imagePath.StartsWith("/") && !imagePath.StartsWith("//")) ? requestUri.GetLeftPart(UriPartial.Authority) + imagePath : imagePath;
                imageUrl = imagePath.StartsWith("//") ? requestUri.Scheme + ":" + imageUrl : imageUrl;
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

            return null;
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
            var content = string.Empty;
            try
            {
                var imgs = pageSource.DocumentNode.SelectNodes("//img");
                if(imgs != null)
                {
                    foreach (var img in imgs)
                    {
                        img.Remove();
                    }
                }

                var styleLinks = pageSource.DocumentNode.SelectNodes("//link");
                if(styleLinks!=null)
                {
                    foreach (var style in styleLinks)
                    {
                        style.Remove();
                    }
                }

                var scripts = pageSource.DocumentNode.SelectNodes("//script");
                if (scripts != null)
                {
                    foreach (var script in scripts)
                    {
                        script.Remove();
                    }
                }
                

                var styles = pageSource.DocumentNode.SelectNodes("//style");
                if(styles!=null)
                {
                    foreach (var style in styles)
                    {
                        style.Remove();
                    }
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
            catch (Exception ex)
            {
                var text = ex.Message;
                //log ex
                throw;
            }

            return content.Trim().ToLower();
        }

    }

}