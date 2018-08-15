using PageScraper.Business;
using PageScraper.Models;
using PageScraper.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PageScraper.Controllers
{
    public class ScrapeController : Controller
    {
        private IPageBuilder _pageBuilder;

        public ScrapeController(IPageBuilder pageBuilder)
        {
            _pageBuilder = pageBuilder;
        }
        // GET: Scrape
        public ActionResult _Request()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetScrapedResults(ScrapedResults request)
        {
            if (ModelState.IsValid)
            {
                var model = _pageBuilder.GetDataFromUrl(request.PageUrl);

                return PartialView("_Request", model);
            }
            else
            {
                return PartialView("_Request", request);
            }
                
        }
    }
}