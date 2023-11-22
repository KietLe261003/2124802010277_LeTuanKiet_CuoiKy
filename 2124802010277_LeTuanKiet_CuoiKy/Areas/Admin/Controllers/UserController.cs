using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        // GET: Admin/User
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string TenDN, string Password)
        {
            if (String.IsNullOrEmpty(TenDN))
            {
                ViewData["err1"] = "Tên đăng nhập không được để trống";
            }
            else if (String.IsNullOrEmpty(Password))
            {
                ViewData["err2"] = "Mật khẩu không được để trống";
            }
            else
            {
                NguoiDung kh = db.NguoiDungs.SingleOrDefault(item => item.TenDN == TenDN && item.MatKhau == Password);
                if (kh != null)
                {
                    Session["TaiKhoan"] = kh;
                    return RedirectToAction("Index", "Home");
                    
                }
                else
                {
                    ViewBag.thongbao = "Sai tên tk hoặc mật khẩu";
                }
            }
            return View();
        }
    }
}