using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Controllers
{
    public class AssignmentsController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Assignments
        public ActionResult Index(int id)
        {
            //int id = (int)Session["Id"];
            var x = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == id);
            List<ChiTietBaiTap> tmp = db.ChiTietBaiTaps.Where(item => item.IdBaiTap == id).ToList();
            ViewBag.BaiTap = x;
            return View(tmp);
        }
        public ActionResult TangLuotXem(int id)
        {
            Session["Id"] = id;
            var x = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == id);
            if (x != null)
            {
                x.LuotXem += 1;
                db.SaveChanges();
            }
            return RedirectToAction("Index",new { id=id});
        }
        public ActionResult ListAssi(string KeyWork,int pg=1)
        {
            List<BaiTap> ds = db.BaiTaps.ToList();
            if(!String.IsNullOrEmpty(KeyWork))
            {
                int idbt=0;
                if(int.TryParse(KeyWork, out int number))
                {
                    idbt = int.Parse(KeyWork);
                }    
                ds = db.BaiTaps.Where(item => item.TenBaiTap.Contains(KeyWork) || item.IdBaiTap==idbt).ToList();
            }    
            const int pageSize = 20;
            if (pg < 1)
                pg = 1;

            int totalitem = ds.Count();

            var pager = new Pager(totalitem, pg, pageSize);

            int skipPage = (pg - 1) * pageSize;
            List<BaiTap> ds1= ds.OrderBy(item => item.IdBaiTap).Skip(skipPage).Take(pageSize).ToList();
            ViewBag.Pager = pager;
            return View(ds1);
        }
        public ActionResult CreateAssignment()
        {
            if(Session["TaiKhoan"]==null)
            {
                return RedirectToAction("DangNhap","User");
            }
            List<ChiTietBaiTap> tmp = new List<ChiTietBaiTap>(); 
            return View(tmp);
        }
        [HttpPost]
        public ActionResult CreateAssignment(string TenBaiTap, List<ChiTietBaiTap> ListCauHoi)
        {
            string bg = "Các câu hỏi bị lỗi cần sửa lại: ";
            int check = 0;
            for (int i = 0; i < ListCauHoi.Count; i++)
            {
                if (ListCauHoi[i].Ques == null && ListCauHoi[i].A == null
                    && ListCauHoi[i].B == null && ListCauHoi[i].C == null
                    && ListCauHoi[i].D == null && ListCauHoi[i].Ans == null)
                {
                    continue;
                }    
                if (ListCauHoi[i].Ques == null || ListCauHoi[i].A == null 
                    || ListCauHoi[i].B == null || ListCauHoi[i].C == null 
                    || ListCauHoi[i].D == null || ListCauHoi[i].Ans == null 
                    || (ListCauHoi[i].Ans!=ListCauHoi[i].A) && (ListCauHoi[i].Ans != ListCauHoi[i].B) && (ListCauHoi[i].Ans != ListCauHoi[i].C) && (ListCauHoi[i].Ans != ListCauHoi[i].D))
                {
                    bg += ", " + i;
                    check++;
                }
            }
            if(check>0)
            {
                ViewBag.bug = bg;
                ViewBag.TenBaiTap = TenBaiTap;
                return View(ListCauHoi);
            }    
            BaiTap tmp = new BaiTap();
            tmp.TenBaiTap=TenBaiTap;
            NguoiDung Nd = (NguoiDung)Session["TaiKhoan"];
            tmp.TacGia = Nd.MaKH;
            tmp.SoLuotNop = 0;
            tmp.NgayDang = DateTime.Now;
            tmp.DiemTrungBinh = 0;
            tmp.LuotThich = 0;
            tmp.LuotXem = 0;
            db.BaiTaps.Add(tmp);
            db.SaveChanges();
            for (int i = 0; i < ListCauHoi.Count; i++)
            {
                if(ListCauHoi[i].Ques!=null)
                {
                    ChiTietBaiTap bt = ListCauHoi[i];
                    bt.IdBaiTap = tmp.IdBaiTap;
                    db.ChiTietBaiTaps.Add(bt);
                }    
            }
            db.SaveChanges();
            return RedirectToAction("CreateAssignment");
        }

       /* Show điểm của người dùng*/
       void savePoint(int Acp,int AllQues,int IdBaiTap)
        {
            BaiTap bt = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == IdBaiTap);
            bt.DiemTrungBinh += (double)Acp / AllQues * 10;
            db.SaveChanges();
        }
       public ActionResult ShowPoint()
        {
            int IdBaiTap = (int)Session["IdBaiTap"];
            if(Session["Diem"]!=null)
            {
                ViewBag.Diem = Session["Diem"];
            }
            if (Session["SoCauHoi"] != null)
            {
                ViewBag.SoCauHoi = Session["SoCauHoi"];
            }
            if (Session["SoCauDung"] != null)
            {
                ViewBag.SoCauDung = Session["SoCauDung"];
            }
            if (Session["SoCauSai"] != null)
            {
                ViewBag.SoCauSai = Session["SoCauSai"];
            }
            if (Session["SoCauChuaLam"] != null)
            {
                ViewBag.SoCauChuaLam = Session["SoCauChuaLam"];
            }
            if (Session["LuaChon"] != null)
            {
                List<string> Chosice = (List<String>)Session["LuaChon"];
                ViewBag.tmp = Chosice;
            }
            List<ChiTietBaiTap> tmp = db.ChiTietBaiTaps.Where(item => item.IdBaiTap == IdBaiTap).ToList();
            return View(tmp);
        }
        public ActionResult SavePoint(int Acp, int Wr, int AllQues, int ChuaLam, string ChosseUser, int IdBaiTap)
        {
            Session["Diem"] = (double)Acp / AllQues * 10;
            Session["SoCauHoi"] = AllQues;
            Session["SoCauDung"] = Acp;
            Session["SoCauSai"] = Wr;
            Session["SoCauChuaLam"] = ChuaLam;
            List<string> Chosice = ChosseUser.Split(',').ToList();
            Session["IdBaiTap"] = IdBaiTap;
            Session["LuaChon"] = Chosice;
        
            BaiTap bt = db.BaiTaps.FirstOrDefault(item => item.IdBaiTap == IdBaiTap);
            double Diem = (double)Acp / AllQues * 10;
            double sum = (double)bt.SoLuotNop * (double)bt.DiemTrungBinh;
            sum += Diem;
            bt.SoLuotNop++;
            bt.DiemTrungBinh =sum/bt.SoLuotNop;
            
            db.SaveChanges();
            return RedirectToAction("ShowPoint");
        }
        public ActionResult test()
        {
            return View();
        }
    }
}