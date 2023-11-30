using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Controllers
{
    public class UserController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKy(FormCollection data)
        {
            NguoiDung kh = new NguoiDung();
            var sHoTen = data["HoTenKH"];
            var sTenDN = data["TenDN"];
            var sMatKhau = data["Matkhau"];
            var sMatKhauNhapLai = data["MatKhauNL"];
            var sDiaChi = data["DiaChiKH"];
            var sEmail = data["Email"];
            var sDienThoai = data["DienThoaiKH"];
            var sHinhAnh = data["HinhAnh"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", data["NgaySinh"]);
            

            if (String.IsNullOrEmpty(sHoTen))
            {
                ViewData["err1"] = "Họ tên không được rỗng";
            }
            else if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "Tên đăng nhập không được rỗng";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "Mật khẩu không được để trống";
            }
            else if (String.IsNullOrEmpty(sMatKhauNhapLai))
            {
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }
            else if (sMatKhau != sMatKhauNhapLai)
            {
                ViewData["err4"] = "Mật khẩu không trùng khớp ";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err5"] = "Email không được rỗng";
            }
            else if (db.NguoiDungs.FirstOrDefault(item => item.TenDN == sTenDN) != null)
            {
                ViewData["err2"] = "Tên đăng nhập đã tồn tại";
            }
            else if (db.NguoiDungs.FirstOrDefault(item => item.Email == sEmail) != null)
            {
                ViewData["err5"] = "Email đã được sử dụng";
            }
            else
            {
                kh.HoTenKH = sHoTen;
                kh.TenDN = sTenDN;
                kh.MatKhau = sMatKhau;
                kh.Email = sEmail;
                kh.DiaChiKH = sDiaChi;
                kh.DienThoaiKH = sDienThoai;
                kh.NgaySinh = DateTime.Parse(dNgaySinh);
                kh.GioiTinh = bool.Parse(data["GioiTinh"]);
                kh.HinhDaiDien = sHinhAnh;
                kh.DiemKyThi = 0;
                db.NguoiDungs.Add(kh);
                db.SaveChanges();
                return RedirectToAction("DangNhap");
            }
            return View();
        }

        /*Đăng nhập start*/
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(string TenDN, string Password)
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
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ViewBag.thongbao = "Sai tên tk hoặc mật khẩu";
                }
            }
            return View();
        }
        /*Đăng nhập end*/

        public ActionResult profile()
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            if(nd==null)
            {
                return RedirectToAction("DangNhap");
            }   
            return View(nd);
        }
        [HttpPost]
        public ActionResult profile(NguoiDung nd)
        {
            //NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            NguoiDung nd1 = db.NguoiDungs.FirstOrDefault(item => item.MaKH==nd.MaKH);
            nd1.HoTenKH = nd.HoTenKH;
            nd1.DiaChiKH = nd.DiaChiKH;
            nd1.DienThoaiKH = nd.DienThoaiKH;
            nd1.MatKhau = nd.MatKhau;
            nd1.NgaySinh = nd.NgaySinh;
            nd1.GioiTinh = nd.GioiTinh;
            nd1.Email = nd.Email;
            nd1.HinhDaiDien = nd.HinhDaiDien;
            db.SaveChanges();
            if (nd == null)
            {
                RedirectToAction("DangNhap");
            }
            return View(nd1);
        }
        public ActionResult test()
        {
            return View();
        }
    }
}