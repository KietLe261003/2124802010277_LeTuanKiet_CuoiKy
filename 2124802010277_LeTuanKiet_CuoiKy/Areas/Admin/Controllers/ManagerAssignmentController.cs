using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.App_Start;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class ManagerAssignmentController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Admin/ManagerAssignment
        [AdminAuthorize()]
        public ActionResult Index(string KeyWork, int pg = 1)
        {
            List<BaiTap> tmp = db.BaiTaps.ToList();
            if(!string.IsNullOrEmpty(KeyWork))
            {
                tmp = db.BaiTaps.Where(item => item.TenBaiTap.Contains(KeyWork)).ToList();
            }    
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
        [AdminAuthorize()]
        public ActionResult Details(int id)
        {
            BaiTap tmp = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == id);
            List <ChiTietBaiTap> ct= db.ChiTietBaiTaps.Where(item => item.IdBaiTap == id).ToList();
            ViewBag.ct = ct;
            return View(tmp);
        }
        [AdminAuthorize()]
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
        [AdminAuthorize()]
        public ActionResult Edit(int id)
        {
            BaiTap tmp = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == id);
            List<ChiTietBaiTap> ct = db.ChiTietBaiTaps.Where(item => item.IdBaiTap == id).ToList();
            ViewBag.ct = ct;
            return View(tmp);
        }
        [AdminAuthorize()]
        [HttpPost]
        public ActionResult Edit(int IdBaiTap,List<ChiTietBaiTap> bt)
        {
            BaiTap tmp = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == IdBaiTap);
            tmp.ChiTietBaiTaps.Clear();
            List<ChiTietBaiTap> bt1 = new List<ChiTietBaiTap>();
            foreach(var x in bt)
            {
                if (x.Ques != null && x.A != null && x.B != null && x.C != null && x.D != null && x.Ans != null)
                    bt1.Add(x);
            }    
            foreach(var item in bt1)
            {
                item.IdBaiTap = IdBaiTap;
            }    
            tmp.ChiTietBaiTaps = bt1;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}