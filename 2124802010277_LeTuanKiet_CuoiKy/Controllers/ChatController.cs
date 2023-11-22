using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        public ActionResult Index(int? id)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            if (nd == null)
                return RedirectToAction("DangNhap", "User",FormMethod.Get);

            if(id==null)
            {
                ViewBag.Name = nd.HoTenKH;
                ViewBag.Id = nd.MaKH;
                ViewBag.Hinh = nd.HinhDaiDien;
            }    
            else
            {
                ViewBag.Name = nd.HoTenKH;
                ViewBag.Id = nd.MaKH;
                ViewBag.Hinh = nd.HinhDaiDien;
                var tmp = db.Messages.Where(item => item.RoomId == id && item.NoiDung != null);
                ViewBag.IdRoom = id;
                return View(tmp);
            }
            return View();
        }
        public ActionResult ListRoom()
        {
            NguoiDung user = (NguoiDung)Session["TaiKhoan"];

            List<Message> ms = db.Messages.Where(item => item.IdNguoiDung == user.MaKH).ToList();
            HashSet<int> IdRoom = new HashSet<int>();
            foreach(var item in ms)
            {
                IdRoom.Add((int)item.RoomId);
            }
            ViewBag.NowId = user.MaKH;
            return PartialView(IdRoom);
        }
    }
}