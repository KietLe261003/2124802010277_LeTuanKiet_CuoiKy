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
        public ActionResult Index(int? id, int Menu = 1,int TypeRoom=1,int Idbt=0)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            if (nd == null)
                return RedirectToAction("DangNhap", "User",FormMethod.Get);

            if(id==null)
            {
                ViewBag.Menu = Menu;
                ViewBag.Name = nd.HoTenKH;
                ViewBag.Id = nd.MaKH;
                ViewBag.Hinh = nd.HinhDaiDien;
            }    
            else
            {
                ViewBag.Menu = Menu;
                ViewBag.Name = nd.HoTenKH;
                ViewBag.Id = nd.MaKH;
                ViewBag.Hinh = nd.HinhDaiDien;
                var tmp = db.Messages.Where(item => item.RoomId == id && item.NoiDung != null);
                ViewBag.IdRoom = id;
                ViewBag.TypeRoom = TypeRoom;
                ViewBag.Idbt = Idbt;
                return View(tmp);
            }
            ViewBag.TypeRoom = TypeRoom;
            ViewBag.Menu = Menu;
            ViewBag.Idbt = Idbt;
            return View();
        }
        public ActionResult ListRoom(int TypeRoom)
        {
            NguoiDung user = (NguoiDung)Session["TaiKhoan"];

            List<Message> ms = db.Messages.Where(item => item.IdNguoiDung == user.MaKH && db.Rooms.FirstOrDefault(x=>x.RoomId==item.RoomId).Type==TypeRoom).ToList();
            HashSet<int> IdRoom = new HashSet<int>();
            foreach(var item in ms)
            {
                IdRoom.Add((int)item.RoomId);
            }
            ViewBag.TypeRoom = TypeRoom;
            ViewBag.NowId = user.MaKH;
            return PartialView(IdRoom);
        }
        public ActionResult ChatUser(int? IdRoom,IEnumerable<Message> tmp)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            ViewBag.Name = nd.HoTenKH;
            ViewBag.Id = nd.MaKH;
            ViewBag.Hinh = nd.HinhDaiDien;
            ViewBag.IdRoom = IdRoom;
            return PartialView(tmp);
        }

        /*Xử lý phần giao bài tập cho phòng start*/

        /*TypeRoom trong đoạn này mặc định là 2*/
        public ActionResult BaiTap(int RoomId,int IdBt=0)
        {
            ViewBag.RoomId = RoomId;
            ViewBag.IdBaiTap = IdBt;
            List<BaiTapRoom> tmp = db.BaiTapRooms.Where(item=> item.IdRoom==RoomId).ToList();
            return PartialView(tmp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemBaiTap(FormCollection f, IEnumerable<HttpPostedFileBase> fileList)
        {
            var UrlTuongDoi = "/File/OtherFile/"; //Đường dẫn lưu trên database
            var UrlTuyetDoi = Server.MapPath(UrlTuongDoi); //Đường dẫn lưu trên server


            int IdRoom = int.Parse(f["IdRoom"]);
            string sNoiDung = f["NoiDung"];
            string sTen = f["Ten"];
            DateTime DStart = DateTime.Parse(f["DateStart"]);
            DStart = DStart.AddHours(Convert.ToInt32(f["HourStart"]));
            DStart = DStart.AddMinutes(Convert.ToInt32(f["MinuteStart"]));

            DateTime Dend = DateTime.Parse(f["DateEnd"]);
            Dend = Dend.AddHours(Convert.ToInt32(f["HourEnd"]));
            Dend = Dend.AddMinutes(Convert.ToInt32(f["MinuteEnd"]));

            BaiTapRoom btr = new BaiTapRoom();
            btr.IdRoom = IdRoom;
            btr.Ten = sTen;
            btr.NgayBatDau = DStart;
            btr.NgayKetThuc = Dend;
            btr.NgayDang = DateTime.Now;
            btr.TyLeNop = 0;
            btr.DiemCaoNhat = 0;
            btr.DiemTrungBinh = 0;
            btr.NoiDungBaiTap = sNoiDung;

            db.BaiTapRooms.Add(btr);

            foreach(var item in fileList)
            {
                item.SaveAs(UrlTuyetDoi + item.FileName);
                FileMotaBtRoom fl = new FileMotaBtRoom();
                fl.IdBtRoom = btr.IdBTRoom;
                fl.File = UrlTuongDoi + item.FileName;
                db.FileMotaBtRooms.Add(fl);
            }
            db.SaveChanges();
            return RedirectToAction("BaiTap", new { RoomId=IdRoom});
        }
        public ActionResult NopBai(int IdRoom,int btr)
        {
            BaiTapRoom bt = db.BaiTapRooms.FirstOrDefault(item => item.IdBTRoom == btr);
            ViewBag.IdRoom = IdRoom;
            return PartialView(bt);
        }
        public ActionResult DownloadFile(string filename)
        {
            // Đường dẫn đầy đủ đến thư mục chứa file
            string filePath = Server.MapPath(filename);

            // Kiểm tra xem file có tồn tại không
            if (System.IO.File.Exists(filePath))
            {
                // Đọc nội dung file thành một mảng byte
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);

                // Đặt tên file cho phản hồi
                
                var UrlTuongDoi = "/File/OtherFile/";
                int end = filename.Length;
                int start = UrlTuongDoi.Length;
                string fileName = filename.Substring(start,end - start);

                // Trả về file cho client
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                // Trả về 404 nếu file không tồn tại
                return HttpNotFound();
            }
        }
        /*Xử lý phần giao bài tập cho phòng end*/
    }
}