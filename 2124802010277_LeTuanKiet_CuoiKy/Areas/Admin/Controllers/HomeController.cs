using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        DataTiengAnhEntities db = new DataTiengAnhEntities();

        List<string> ChartMonth = new List<string>();
        List<int> DtChartMonth = new List<int>();
        private void CreateChart()
        {
            int nowMonth = DateTime.Now.Month;
            int nowYear = DateTime.Now.Year;
            for (int i = nowMonth - 5; i <= nowMonth; i++)
            {
                if (i < 1)
                {
                    int tmp = db.KyThis.Where(item => item.Ngay.Value.Month == (i + 12) && item.Ngay.Value.Year == nowYear).Count();
                    string chartmonth = "Tháng " + ((i + 12));
                    DtChartMonth.Add(tmp);
                    ChartMonth.Add(chartmonth);
                }
                else
                {
                    int tmp = db.KyThis.Where(item => item.Ngay.Value.Month == i && item.Ngay.Value.Year == nowYear).Count();
                    string chartmonth = "Tháng " + i;
                    DtChartMonth.Add(tmp);
                    ChartMonth.Add(chartmonth);
                }

            }
        }
        // GET: Admin
        public ActionResult Index()
        {
            List<NguoiDung> ds = db.NguoiDungs.OrderByDescending(item => item.DiemKyThi).Take(10).ToList();
            ViewBag.Top10 = ds;

            CreateChart();
            ViewBag.ChartMonth = ChartMonth;
            ViewBag.DtChartMonth = DtChartMonth;

            ViewBag.SoLuongNguPhap = db.NguPhaps.Count();
            ViewBag.SoLuongBaiTap = db.BaiTaps.Count();
            ViewBag.SoluongKyThi = db.KyThis.Count();
            return View();
        }

    }
}