using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ModelEntity;
using System.Text;
using System.Web.UI.DataVisualization.Charting;
using System.IO;
using System.Drawing;

namespace CFSWebService.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
       private CFSDB db = new CFSDB();

           
        
        // GET: Chart
        public ActionResult Index()
        {
           // return View(await db.CFSActivities.Where(r => r.RecordedDate > DateTime.Now.AddDays(-7)).ToListAsync());
           
          

            return View();
        }

        public ActionResult Chart(string chartType)
        {
            var chart = buildChart(chartType);
            StringBuilder result = new StringBuilder();
            result.Append(getChartImage(chart));
            result.Append(chart.GetHtmlImageMap("ImageMap"));
            return Content(result.ToString());
        }

        private Chart buildChart(string chartType)
        {
            // Build Chart
            var chart = new Chart();

            chart.Width = 500;
            chart.Height = 300;

            // Create chart here
            chart.Titles.Add(CreateTitle());
            chart.Legends.Add(CreateLegend());
            chart.ChartAreas.Add(CreateChartArea());
            chart.Series.Add(CreateSeries(chartType));

            return chart;
        }

        private string getChartImage(Chart chart)
        {
            using (var stream = new MemoryStream())
            {
                string img = "<img src='data:image/png;base64,{0}' alt='' usemap='#ImageMap'>";
                chart.SaveImage(stream, ChartImageFormat.Png);
                string encoded = Convert.ToBase64String(stream.ToArray());
                return String.Format(img, encoded);
            }
        }

        private Title CreateTitle()
        {
            Title title = new Title();
            title.Text = "Result Chart";
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            title.ShadowOffset = 3;
            title.ForeColor = Color.FromArgb(26, 59, 105);
            return title;
        }

        private Legend CreateLegend()
        {
            Legend legend = new Legend();
            legend.Enabled = true;
            legend.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            legend.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            legend.ShadowOffset = 3;
            legend.ForeColor = Color.FromArgb(26, 59, 105);
            legend.Title = "Legend";
            return legend;
        }

        private ChartArea CreateChartArea()
        {
            ChartArea chartArea = new ChartArea();
            chartArea.Name = "Result Chart";
            chartArea.BackColor = Color.Transparent;
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisY.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            chartArea.Area3DStyle.Enable3D = true;
            return chartArea;
        }

        public Series CreateSeries(string chartType)
        {
            Series seriesDetail = new Series();
            seriesDetail.Name = "Result Chart";
            seriesDetail.IsValueShownAsLabel = false;
            seriesDetail.Color = Color.FromArgb(198, 99, 99);
            
            switch (chartType)
            {
                case "Pie":
                    seriesDetail.ChartType = SeriesChartType.Pie;
                    break;
                case "Bar":
                    seriesDetail.ChartType = SeriesChartType.Bar;
                    break;
                case "Line":
                    seriesDetail.ChartType = SeriesChartType.Line;
                    break;
                case "other":
                    seriesDetail.ChartType = SeriesChartType.Funnel;
                    break;
            }
            seriesDetail.BorderWidth = 2;

            //data

            GetData(seriesDetail);
            //for (int i = 1; i < 20; i++)
            //{
            //    var p = seriesDetail.Points.Add(i);
            //    p.Label = String.Format("Point {0}", i);
            //    p.LabelMapAreaAttributes = String.Format("href=\"javascript:void(0)\" onclick=\"myfunction('{0}')\"", i);
            //    p["BarLabelStyle"] = "Center";
            //}

            seriesDetail.ChartArea = "Result Chart";
            return seriesDetail;
        }

        private void GetData(Series seriesDetail)
        {
            var result =  db.CFSActivities.Where(r => r.RecordedDate > DateTime.Now.Date.AddDays(-7)).ToListAsync();
            for (int i = 1; i < 7; i++)
            {
                var p = seriesDetail.Points.Add(i);
                p.Label = String.Format("Point {0}", i);
                p.LabelMapAreaAttributes = String.Format("href=\"javascript:void(0)\" onclick=\"myfunction('{0}')\"", i);
                p["BarLabelStyle"] = "Center";
            }
        }
        // GET: Chart/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    CFSActivity cFSActivity = await db.CFSActivities.FindAsync(id);
        //    if (cFSActivity == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(cFSActivity);
        //}

     

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
