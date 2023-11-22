using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class ManagerAssignmentController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Admin/ManagerAssignment
        public ActionResult Index(int pg = 1)
        {
            List<BaiTap> tmp = db.BaiTaps.ToList();
            const int pageSize = 10;
            if (pg < 1)
                pg = 1;

            int totalitem = tmp.Count();

            var pager = new Pager(totalitem, pg, pageSize);
            int skipPage = (pg - 1) * pageSize;
            List<BaiTap> tmp1 = tmp.OrderBy(item => item.IdBaiTap).Skip(skipPage).Take(pageSize).ToList();
            ViewBag.Pager = pager;
            return View(tmp1);
        }
        public ActionResult Details(int id)
        {
            BaiTap tmp = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == id);
            List <ChiTietBaiTap> ct= db.ChiTietBaiTaps.Where(item => item.IdBaiTap == id).ToList();
            ViewBag.ct = ct;
            return View(tmp);
        }
        public ActionResult Delete(int id)
        {
            BaiTap tmp = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == id);

            List<ChiTietBaiTap> ct = db.ChiTietBaiTaps.Where(item => item.IdBaiTap == id).ToList();
            foreach(var item in ct)
            {
                db.ChiTietBaiTaps.Remove(item);
            }    
            db.BaiTaps.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}