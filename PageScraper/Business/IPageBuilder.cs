using PageScraper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageScraper.Business
{
    public interface IPageBuilder
    {
        ScrapedResults GetDataFromUrl(string url);
    }
}
