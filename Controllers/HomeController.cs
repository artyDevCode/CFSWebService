using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace CFSWebService.Controllers
{
    //[RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
           
            return View();
        }

      
        public ActionResult About()
        {
            ViewBag.Message = "Privacy Policy";

            return View();
        }

      
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us";

            return View();
        }

        public ActionResult DrawChart()
        {

            var chart = new Chart(width: 300, height: 200)
                .AddSeries(
                            chartType: "Bar",
                            legend: "Energy Level",
                            xValue: new[] { "Week 1", "Week 2", "Week 3", "Week 4" },
                            yValues: new[] { "70", "44", "78", "89" })
                            .GetBytes("png");

            return File(chart, "image/bytes");
        }
    }
}