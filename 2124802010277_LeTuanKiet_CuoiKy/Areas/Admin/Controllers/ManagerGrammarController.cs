using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.App_Start;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class ManagerGrammarController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Admin/ManagerGrammar
        [AdminAuthorize()]
        public ActionResult Index(int pg = 1)
        {
            const int pageSize = 10;
            if (pg < 1)
                pg = 1;
            List<NguPhap> tmp = db.NguPhaps.OrderByDescending(item => item.NgayDang).ToList();
            int totalitem = tmp.Count();
            var pager = new Pager(totalitem, pg, pageSize);
            int skipPage = (pg - 1) * pageSize;
            ViewBag.Pager = pager;
            ViewBag.pg = pg;
            return View(tmp);
        }
        
        [HttpPost]
        public ActionResult ActiveGrammar(int id)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);
            if (tmp != null)
            {
                tmp.TrangThai = !tmp.TrangThai;
                db.SaveChanges();
                return Json(new { success = true, isActive = tmp.TrangThai });
            }
            return Json(new { success = false });
        }

        /*Lọc start*/
        public ActionResult ListGrammar(int? id,string KeyWork, int pg = 1)
        {
            const int pageSize = 10;
            if (pg < 1)
                pg = 1;
            List<NguPhap> tmp = db.NguPhaps.OrderByDescending(item => item.NgayDang).ToList();
            int totalitem = tmp.Count();
            var pager = new Pager(totalitem, pg, pageSize);
            int skipPage = (pg - 1) * pageSize;
            ViewBag.Pager = pager;
            if (!string.IsNullOrEmpty(KeyWork))
            {
                int idNp = -1;
                if (int.TryParse(KeyWork, out _))
                {
                    idNp = int.Parse(KeyWork);
                }
                List<NguPhap> tmp1 = db.NguPhaps.Where(item => item.TenNguPhap.Contains(KeyWork) || item.IdNguPhap == idNp).ToList();
                List<NguPhap> tmp3 = tmp1.OrderByDescending(item => item.IdNguPhap).Skip(skipPage).Take(pageSize).ToList();
                return PartialView(tmp3);
            }
            id = id ?? -1;
            if (id == 0)
            {
                List<NguPhap> tmp1 = db.NguPhaps.OrderByDescending(item=> item.NgayDang).Where(item => item.TrangThai == false).ToList();
                List<NguPhap> tmp3 = tmp1.OrderByDescending(item => item.IdNguPhap).Skip(skipPage).Take(pageSize).ToList();
                return PartialView(tmp3);
            }
            if (id == 1)
            {
                List<NguPhap> tmp1 = db.NguPhaps.OrderByDescending(item => item.NgayDang).Where(item => item.TrangThai == true).ToList();
                List<NguPhap> tmp3 = tmp1.OrderByDescending(item => item.IdNguPhap).Skip(skipPage).Take(pageSize).ToList();
                return PartialView(tmp3);
            }
            List<NguPhap> tmp2 = tmp.OrderByDescending(item => item.NgayDang).Skip(skipPage).Take(pageSize).ToList();
            return PartialView(tmp2);
        }
        /*Lọc end*/

        public ActionResult Create()
        {
            return Redirect("~/Grammar/AddGrammar");
        }

        /*Xóa Sửa Chi tiêt start*/
        public ActionResult Details(int id)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);
            NguoiDung tg = db.NguoiDungs.FirstOrDefault(item => item.MaKH == tmp.IdTacGia);
            ViewBag.TacGia = tg;
            return View(tmp);
        }
        public ActionResult Edit(int id)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap==id);
            NguoiDung tg = db.NguoiDungs.FirstOrDefault(item => item.MaKH == tmp.IdTacGia);
            ViewBag.TacGia = tg;
            return View(tmp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            int id = int.Parse(f["id"]);
            var MotaNgan = f["MoTaNgan"];
            var Noidung = f["NoiDung"];
            var TrangThai = f["TrangThai"];
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);
            tmp.MoTaNgan = MotaNgan;
            tmp.NoiDung = Noidung;
            tmp.TrangThai =bool.Parse(TrangThai);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);
            db.NguPhaps.Remove(tmp);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult test(int id=1)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);
            return View(tmp);
        }
        /*Xóa Sửa Chi tiêt end*/
    }
}