using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.App_Start;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class ManagerUserController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Admin/ManagerUser
        [AdminAuthorize()]
        public ActionResult Index(string KeyWork, int pg = 1)
        {
            List<NguoiDung> tmp = db.NguoiDungs.ToList();
            if (!string.IsNullOrEmpty(KeyWork))
            {
                tmp = db.NguoiDungs.Where(item => item.HoTenKH.Contains(KeyWork)).ToList();
            }
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            ViewBag.nd = nd;
            const int pageSize = 10;
            if (pg < 1)
                pg = 1;

            int totalitem = tmp.Count();

            var pager = new Pager(totalitem, pg, pageSize);
            int skipPage = (pg - 1) * pageSize;
            List<NguoiDung> tmp1 = tmp.OrderBy(item => item.MaKH).Skip(skipPage).Take(pageSize).ToList();
            ViewBag.Pager = pager;
            return View(tmp1);
        }

        public ActionResult SetAdmin(int IdUser,string valu)
        {
            NguoiDung nd = db.NguoiDungs.FirstOrDefault(item => item.MaKH == IdUser);
            if (valu == "true")
                nd.Admin = true;
            else
                nd.Admin = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int IdUser)
        {
            NguoiDung nd = db.NguoiDungs.FirstOrDefault(item => item.MaKH == IdUser);
            db.NguoiDungs.Remove(nd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}