using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Controllers
{
    public class ExamsController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Exams
        public ActionResult Index(int pg = 1)
        {
            List<KyThi> tmp = db.KyThis.ToList();
            const int pageSize = 20;
            if (pg < 1)
                pg = 1;

            int totalitem = tmp.Count();

            var pager = new Pager(totalitem, pg, pageSize);

            int skipPage = (pg - 1) * pageSize;
            List<KyThi> tmp1 = tmp.OrderByDescending(item => item.NgayKetThuc).Skip(skipPage).Take(pageSize).ToList();
            ViewBag.Pager = pager;

            return View(tmp1);
        }

        /*Tạo 1 kỳ thi start*/
        public ActionResult CreateExam()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateExam(FormCollection data)
        {
           
            DateTime tmp =DateTime.Parse(data["Ngay"]);
            tmp = tmp.AddHours(Convert.ToInt32(data["Hour"]));
            tmp = tmp.AddMinutes(Convert.ToInt32(data["Minute"]));
            KyThi newExam = new KyThi();
            newExam.IdKyThi = data["IdKyThi"];
            newExam.TenKyThi = data["TenKyThi"];
            newExam.Ngay =tmp;
            newExam.SoThiSinh = 0;
            db.KyThis.Add(newExam);
            db.SaveChanges();
            Session["IdKyThi"] = data["IdKyThi"];
            return RedirectToAction("AddExam");
        }
        /*Tạo 1 kỳ thi End*/


        public ActionResult AddExam()
        {
            var x = Session["IdKyThi"];
            ViewBag.Id = x;
            List<Question> tmp = new List<Question>();
            return View(tmp);
        }
        
        [HttpPost]
        public ActionResult AddExam(List<Question> LQuestion)
        {
            /*Question tmp = new Question(
                data["tmp.Ques"],
                data["tmp.A"],
                data["tmp.B"],
                data["tmp.C"],
                data["tmp.D"],
                data["tmp.Ans"]
            );
            db.add(tmp);*/
            List<Question> tmp = LQuestion;

            return View(tmp);
        }

        /*Trang thi đấu start*/
        private List<CauHoi> LayBoCauHoi(string id)
        {
            List<CauHoi> tmp = db.CauHois.Where(item => item.IdBoCauHoi == id).ToList();
                return tmp;
        }
        public ActionResult ListQuestion(string id)
        {

            List<CauHoi> tmp = LayBoCauHoi(id);
            KyThi check = db.KyThis.FirstOrDefault(item => item.IdKyThi==id);
            if(Session["TaiKhoan"]==null)
            {
                return RedirectToAction("DangNhap","User");
            }
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            ChiTietThiSinh check1 = db.ChiTietThiSinhs.FirstOrDefault(item => item.IdThiSinh == nd.MaKH && item.IdKyThi == id);
            if (check1 == null && check.NgayKetThuc>DateTime.Now)
            {
                return RedirectToAction("ChuaDangKy");
            }
            if(check1.Diem!=null)
            {
                return RedirectToAction("DaThi");
            }    
            if (check.Ngay>DateTime.Now)
            {
                return RedirectToAction("Wait");
            }

            Session["IdKyThi"] = id;
            return View(tmp);
        }

        public ActionResult Wait()
        {
            return View();
        }
        public ActionResult DangKyThi(string IdKythi)
        {
            if(Session["TaiKhoan"]==null)
            {
                return RedirectToAction("DangNhap", "User");
            }
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            ChiTietThiSinh tmp = new ChiTietThiSinh();
            tmp.IdKyThi = IdKythi;
            tmp.IdThiSinh = nd.MaKH;
           
            db.ChiTietThiSinhs.Add(tmp);

            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi==IdKythi);
            kt.SoThiSinh += 1;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ChuaDangKy()
        {
            return View();
        }
        
        public ActionResult ShowPoint(int Acp,int Wr,int AllQues,int ChuaLam)
        {
            double point = (double)Acp/AllQues*10;
            string IdKyThi1 = (string)Session["IdKyThi"];
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];

            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi == IdKyThi1);
            if(DateTime.Now<kt.NgayKetThuc)
            {
                ChiTietThiSinh tmp = db.ChiTietThiSinhs.FirstOrDefault(item => item.IdKyThi == IdKyThi1 && item.IdThiSinh == nd.MaKH);
                tmp.Diem = point;
                tmp.Dung = Acp;
                tmp.Sai = Wr;
                tmp.ChuaLam = ChuaLam;
                db.SaveChanges();
                ViewBag.SoCauHoi = AllQues;
                return View(tmp);
            } 
            else
            {
                ChiTietThiSinh tmp = new ChiTietThiSinh();
                tmp.Diem = point;
                tmp.Dung = Acp;
                tmp.Sai = Wr;
                tmp.ChuaLam = ChuaLam;
                db.SaveChanges();
                ViewBag.SoCauHoi = AllQues;
                return View(tmp);
            }    
            
        }
        public ActionResult DaThi()
        {
            return View();
        }
        /*Trang thi đấu end*/
    }
}