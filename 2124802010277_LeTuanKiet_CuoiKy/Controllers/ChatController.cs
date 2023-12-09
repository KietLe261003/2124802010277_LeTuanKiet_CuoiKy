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

        /*Id Là Id của phòng*/
        /*Menu Chỉ dùng cho TypeRoom=2 để hiển thị bài tập hoặc điểm, hoặc chat*/
        /*TypeRoom Là loại của phòng nếu 1 là phòng chat cá nhân 2 là phòng học*/
        /*Idbt khi người dùng bấm vào 1 bài tập nào đó trong phòng học thì bài tập sẽ hiển thị lên*/
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
                var tmp = db.Messages.Where(item => item.RoomId == id);
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
        /*Hiển thị danh sách các phòng*/
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
        /*Tìm kiếm người dùng cho cuộc trò chuyện*/
        [HttpPost]
        public ActionResult FindUser(string KeyWork)
        {
            try
            {
                NguoiDung nd1 = (NguoiDung)Session["TaiKhoan"];
                List<NguoiDung> nd = db.NguoiDungs.Where(item => item.HoTenKH.Contains(KeyWork) && item.MaKH!=nd1.MaKH).ToList();
                List<string> HinhDaiDien = new List<string>();
                List<int> IdKH = new List<int>();
                List<string> Hoten = new List<string>();
                foreach(var item in nd)
                {
                    HinhDaiDien.Add(item.HinhDaiDien);
                    IdKH.Add(item.MaKH);
                    Hoten.Add(item.HoTenKH);
                }    
                return Json(new {IdKh=IdKH,HinhDaiDien=HinhDaiDien,Hoten=Hoten });
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.Message);
                return Json(new { error = "An error occurred while processing your request." });
            }
        }
        [HttpPost]
        public ActionResult FindRoom(string KeyWork)
        {
            try
            {
                NguoiDung nd1 = (NguoiDung)Session["TaiKhoan"];
                List<Room> r = db.Rooms.Where(item => item.Name.Contains(KeyWork) && item.Type==2 && item.IdJoin==null).ToList();
                List<int> idr = new List<int>();
                List<string> namerom = new List<string>();
                foreach(var item in r)
                {
                    idr.Add(item.RoomId);
                    namerom.Add(item.Name);
                }    
                return Json(new { IdR = idr,NameRoom=namerom });
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.Message);
                return Json(new { error = "An error occurred while processing your request." });
            }
        }
        /*tìm kiếm 1 người dùng ở thanh tìm kiếm*/
        [HttpPost]
        public ActionResult FindUserTabar(string KeyWork,int TypeRoom)
        {
            try
            {
                if(TypeRoom==1)
                {
                    NguoiDung nd1 = (NguoiDung)Session["TaiKhoan"];
                    List<NguoiDung> nd = db.NguoiDungs.Where(item => item.HoTenKH.Contains(KeyWork)).ToList();
                    
                    List<string> HinhDaiDien = new List<string>();
                    List<int> IdKH = new List<int>();
                    List<string> Hoten = new List<string>();
                    List<int> Room = new List<int>();
                    foreach (var item in nd)
                    {
                        HinhDaiDien.Add(item.HinhDaiDien);
                        IdKH.Add(item.MaKH);
                        Hoten.Add(item.HoTenKH);
                    }
                    return Json(new { IdKh = IdKH, HinhDaiDien = HinhDaiDien, Hoten = Hoten });
                }    
                else
                {
                    NguoiDung nd1 = (NguoiDung)Session["TaiKhoan"];
                    List<Room> r = db.Rooms.Where(item => item.Name.Contains(KeyWork)).ToList();
                    List<int> idroom = new List<int>();
                    List<string> NameRoom = new List<string>();
                    foreach(var item in r)
                    {
                        if(db.ChiTietPhongs.FirstOrDefault(x=> x.IdPhong==item.RoomId && x.MaKH==nd1.MaKH)!=null)
                        {
                            idroom.Add((int)item.RoomId);
                            NameRoom.Add(item.Name);
                        }    
                    }    
                    return Json(new { IdRoom = idroom,NameRoom=NameRoom});
                }    
                
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.Message);
                return Json(new { error = "An error occurred while processing your request." });
            }
        }

        /*Kiểm tra xem người dùng hiện tại đã chat với người được chọn chưa nếu chưa thì tạo room mới không thì hiển thị chat của 2 người đó*/
        public ActionResult CreateRoomType1(int IdUserChat)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            HashSet<int> IdRoom = new HashSet<int>();
            foreach(var item in db.Messages)
            {
                if(item.IdNguoiDung==nd.MaKH && db.Rooms.FirstOrDefault(i=> i.RoomId==item.RoomId).Type==1)
                {
                    IdRoom.Add((int)item.RoomId);
                }    
            }    
            foreach(var item in db.Messages)
            {
                if(item.IdNguoiDung==IdUserChat)
                {
                    int IdR = IdRoom.FirstOrDefault(x => x == (int)item.RoomId);
                    if (IdR!=0)
                    {
                        return RedirectToAction("Index", new { id=IdR,Menu=1,TypeRoom=1});
                    }    
                }    
            }
            Room r = new Room();
            r.Name = "Phòng loại 1";
            r.Type = 1;
            db.Rooms.Add(r);
            db.SaveChanges();
            Message m1 = new Message();
            Message m2 = new Message();
            m1.IdNguoiDung = nd.MaKH;
            m1.Gio = DateTime.Now;
            m1.RoomId = r.RoomId;

            m2.IdNguoiDung = IdUserChat;
            m2.Gio = DateTime.Now;
            m2.RoomId = r.RoomId;

            db.Messages.Add(m1);
            db.Messages.Add(m2);
            db.SaveChanges();

            return RedirectToAction("Index", new { id=r.RoomId,Menu=1,TypeRoom=1});
        }

        /*Hàm tạo mã tự động*/
        string GenerateRandomCode(int length=6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            char[] codeArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                codeArray[i] = chars[random.Next(chars.Length)];
            }

            return new string(codeArray);
        }
        [HttpPost]
        public ActionResult CreateRoomType2(FormCollection f)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            var sName = f["NameNhom"];
            var sMoTa = f["DetialNhom"];
            int RiengTu = int.Parse(f["LoaiNhom"]);
            Room r = new Room();
            r.Name = sName;
            r.Type = 2;
            r.MoTa = sMoTa;
            if(RiengTu==1)
            {
                r.IdJoin = GenerateRandomCode();
            }
            db.Rooms.Add(r);
            db.SaveChanges();
            ChiTietPhong ct = new ChiTietPhong();
            ct.MaKH = nd.MaKH;
            ct.IdPhong = r.RoomId;
            ct.ChuPhong = true;
            ct.Room = r;
            db.ChiTietPhongs.Add(ct);

            Message m = new Message();
            m.IdNguoiDung = nd.MaKH;
            m.Gio = DateTime.Now;
            m.RoomId = r.RoomId;
            db.Messages.Add(m);


            db.SaveChanges();
            return RedirectToAction("Index", new { id = r.RoomId, Menu = 1, TypeRoom = 2 });
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

        public ActionResult ThongBaoThatBai()
        {
            return View();
        }
        /*Tham gia lớp học*/
        [HttpPost]
        public ActionResult AddMember(string KeyWork)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            if(!string.IsNullOrEmpty(KeyWork))
            {
                Room r = db.Rooms.FirstOrDefault(item => item.IdJoin == KeyWork);
                if (r != null)
                {
                    ChiTietPhong ct = new ChiTietPhong();
                    ct.MaKH = nd.MaKH;
                    ct.IdPhong = r.RoomId;
                    ct.ChuPhong = false;
                    db.ChiTietPhongs.Add(ct);

                    Message m = new Message();
                    m.IdNguoiDung = nd.MaKH;
                    m.Gio = DateTime.Now;
                    m.RoomId = r.RoomId;
                    db.Messages.Add(m);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = r.RoomId, Menu = 1, TypeRoom = 2 });
                }
            }    
            return RedirectToAction("ThongBaoThatBai");
        }
        public ActionResult AddMember1(int Id)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            Room r = db.Rooms.FirstOrDefault(item => item.RoomId == Id);
            if (r != null)
            {
                ChiTietPhong ct = new ChiTietPhong();
                ct.MaKH = nd.MaKH;
                ct.IdPhong = r.RoomId;
                ct.ChuPhong = false;
                db.ChiTietPhongs.Add(ct);

                Message m = new Message();
                m.IdNguoiDung = nd.MaKH;
                m.Gio = DateTime.Now;
                m.RoomId = r.RoomId;
                db.Messages.Add(m);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = r.RoomId, Menu = 1, TypeRoom = 2 });
            }
            return RedirectToAction("ThongBaoThatBai");
        }


        /*Xử lý phần giao bài tập cho phòng start*/

        /*TypeRoom trong đoạn này mặc định là 2*/
        public ActionResult BaiTap(int RoomId,int IdBt=0)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            ViewBag.RoomId = RoomId;
            ViewBag.IdBaiTap = IdBt;
            ViewBag.IdNd = nd.MaKH;

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
            return RedirectToAction("Index", new { Id=IdRoom, Menu=2, TypeRoom =2});
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaBaiTap(FormCollection f, IEnumerable<HttpPostedFileBase> fileList)
        {
            var UrlTuongDoi = "/File/OtherFile/"; //Đường dẫn lưu trên database
            var UrlTuyetDoi = Server.MapPath(UrlTuongDoi); //Đường dẫn lưu trên server
            int IdBt = int.Parse(f["IdBaiTap"]);
            BaiTapRoom bt = db.BaiTapRooms.FirstOrDefault(item => item.IdBTRoom == IdBt);


            int IdRoom = int.Parse(f["IdRoom"]);
            string sNoiDung = f["NoiDung1"];
            string sTen = f["Ten"];
            DateTime DStart = DateTime.Parse(f["DateStart"]);
            DStart = DStart.AddHours(Convert.ToInt32(f["HourStart"]));
            DStart = DStart.AddMinutes(Convert.ToInt32(f["MinuteStart"]));

            DateTime Dend = DateTime.Parse(f["DateEnd"]);
            Dend = Dend.AddHours(Convert.ToInt32(f["HourEnd"]));
            Dend = Dend.AddMinutes(Convert.ToInt32(f["MinuteEnd"]));

            bt.Ten = sTen;
            bt.NoiDungBaiTap = sNoiDung;
            bt.NgayBatDau = DStart;
            bt.NgayKetThuc = Dend;

            

            if(fileList.ElementAt(0)!=null)
            {
                List<FileMotaBtRoom> flOld = db.FileMotaBtRooms.Where(item => item.IdBtRoom == IdBt).ToList();
                List<FileMotaBtRoom> flnew = new List<FileMotaBtRoom>();
                foreach(var item in flOld)
                {
                    db.FileMotaBtRooms.Remove(item);
                }
                db.SaveChanges();
                foreach (var item in fileList)
                {
                    if(item!=null)
                    {
                        FileMotaBtRoom fl = flOld.FirstOrDefault(i => i.File == UrlTuongDoi + item.FileName && i.IdBtRoom == bt.IdBTRoom);

                        if (fl == null)
                        {
                            item.SaveAs(UrlTuyetDoi + item.FileName);
                            fl = new FileMotaBtRoom();
                            fl.IdBtRoom = bt.IdBTRoom;
                            fl.File = UrlTuongDoi + item.FileName;
                            flnew.Add(fl);

                        }
                        else
                        {
                            fl = new FileMotaBtRoom();
                            fl.IdBtRoom = bt.IdBTRoom;
                            fl.File = UrlTuongDoi + item.FileName;
                        }
                        db.FileMotaBtRooms.Add(fl);
                    }    
                }
                //bt.FileMotaBtRooms = flnew;
            }    
            db.SaveChanges();
            return RedirectToAction("Index", new { Id = IdRoom, Menu = 2, TypeRoom = 2 });
        }

        [HttpPost]
        public ActionResult NopBai(FormCollection f, IEnumerable<HttpPostedFileBase> fileList)
        {
            int IdNd =int.Parse(f["IdNd"]);
            int IdBt = int.Parse(f["IdBaiTap"]);
            int IdRoom = int.Parse(f["IdRoom"]);
            var UrlTuongDoi = "/File/OtherFile/"; //Đường dẫn lưu trên database
            var UrlTuyetDoi = Server.MapPath(UrlTuongDoi); //Đường dẫn lưu trên server

            foreach (var item in fileList)
            {
                item.SaveAs(UrlTuyetDoi + item.FileName); //Lưu lên sever

                ChiTietBaiTapRoom ct = new ChiTietBaiTapRoom();
                ct.IdNguoiNop = IdNd;
                ct.IdBTRoom = IdBt;
                ct.FileNop = UrlTuongDoi + item.FileName;
                ct.NgayNop = DateTime.Now;
                ct.Diem = 0;

                db.ChiTietBaiTapRooms.Add(ct);
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { Id=IdRoom,Menu=2,TypeRoom=2});
        }

        public ActionResult BaiNop(int IdBaiTap,int IdNguoiDung, int IdRoom)
        {
            ViewBag.IdBaiTap = IdBaiTap;
            ViewBag.IdNguoiDung = IdNguoiDung;
            ViewBag.IdRoom = IdRoom;
            return PartialView();
        }
        /*Trang điểm khi menu của index=3 sẽ hiện trang này*/
        public ActionResult Diem(int IdRoom, int IdBt)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            ViewBag.RoomId = IdRoom;
            ViewBag.IdBaiTap = IdBt;
            ViewBag.IdNd = nd.MaKH;

            List<BaiTapRoom> tmp = db.BaiTapRooms.Where(item => item.IdRoom == IdRoom).ToList();
            return PartialView(tmp);
        }
        public ActionResult ChamDiem(int IdBaiTap, int IdNguoiDung, int IdRoom)
        {
            ViewBag.IdBaiTap = IdBaiTap;
            ViewBag.IdNguoiDung = IdNguoiDung;
            ViewBag.IdRoom = IdRoom;
            return PartialView();
        }

        /*Hiển thị file của người dùng khi bấm vào phần điểm*/
        [HttpPost]
        public ActionResult FileUser(int id,int Idbt)
        {
            try
            {
                var ct = db.ChiTietBaiTapRooms.Where(item => item.IdNguoiNop == id && item.IdBTRoom == Idbt).ToList();
                ChiTietBaiTapRoom point = db.ChiTietBaiTapRooms.FirstOrDefault(item => item.IdNguoiNop==id && item.IdBTRoom==Idbt);
                int Diem = 0;
                if(point!=null)
                {
                   Diem = (int)point.Diem;
                }    
                
                List<string> fi = new List<string>();
                foreach(var item in ct)
                {
                    fi.Add(item.FileNop);
                }    
                return Json(new { data = fi,Diem=Diem });
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine(ex.Message);
                return Json(new { error = "An error occurred while processing your request." });
            }
        }
        /*Lưu điểm của người dùng*/
        public ActionResult SavePoint(int IdNguoiNop,int Idbt,int point,int IdRoom)
        {
            if(IdNguoiNop!=0)
            {
                List<ChiTietBaiTapRoom> ct = db.ChiTietBaiTapRooms.Where(item => item.IdNguoiNop == IdNguoiNop && item.IdBTRoom == Idbt).ToList();
                foreach (var item in ct)
                {
                    item.Diem = point;
                }
                db.SaveChanges();
            }    

            return RedirectToAction("Index", new { id=IdRoom,Menu=3,TypeRoom=2,Idbt=Idbt});
        }

        /*Hiển thị chi tiết của phòng*/
        public ActionResult DetailRoom(int IdRoom)
        {
            Room r = db.Rooms.FirstOrDefault(item => item.RoomId == IdRoom);
            return PartialView(r);
        }
        [HttpPost]
        public ActionResult AddUserForRoom(FormCollection f)
        {
            string[] userItems = f["UserItem"].Split(',');

            // Convert the string array to a List<int>
            List<int> tmp = userItems.Select(item => int.Parse(item)).ToList();

            int IdRoom = int.Parse(f["IdRoom"]);
            foreach(var item in tmp)
            {
                if(db.ChiTietPhongs.FirstOrDefault(i=>i.MaKH==item && IdRoom==i.IdPhong)==null)
                {
                    ChiTietPhong ct = new ChiTietPhong();
                    ct.MaKH = item;
                    ct.IdPhong = IdRoom;
                    ct.ChuPhong = false;
                    db.ChiTietPhongs.Add(ct);

                    Message m = new Message();
                    m.IdNguoiDung = item;
                    m.Gio = DateTime.Now;
                    m.RoomId = IdRoom;
                    db.Messages.Add(m);
                }    
            }
            db.SaveChanges();
            return RedirectToAction("Index", new { Menu = 4 , id = IdRoom , TypeRoom = 2 });
        }

        /*Dùng để tải file về*/
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