using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Controllers
{
    public class AdminController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();

        List<string> ChartMonth = new List<string>();
        List<int> DtChartMonth = new List<int>();
        private void CreateChart()
        {   
            int nowMonth = DateTime.Now.Month;
            int nowYear = DateTime.Now.Year;
            for(int i=nowMonth-5;i<=nowMonth;i++)
            {
                if (i < 1)
                {
                    int tmp = db.KyThis.Where(item => item.Ngay.Value.Month == (i + 12) && item.Ngay.Value.Year==nowYear).Count();
                    string chartmonth = "Tháng " + ((i+12));
                    DtChartMonth.Add(tmp);
                    ChartMonth.Add(chartmonth);
                }
                else
                {
                    int tmp = db.KyThis.Where(item => item.Ngay.Value.Month == i && item.Ngay.Value.Year == nowYear).Count();
                    string chartmonth = "Tháng " + i;
                    DtChartMonth.Add(tmp);
                    ChartMonth.Add(chartmonth);
                }    
                
            }    
        }
        // GET: Admin
        public ActionResult Index()
        {
            List<NguoiDung> ds = db.NguoiDungs.OrderByDescending(item => item.DiemKyThi).Take(10).ToList();
            ViewBag.Top10 = ds;

            CreateChart();
            ViewBag.ChartMonth = ChartMonth;
            ViewBag.DtChartMonth = DtChartMonth;

            ViewBag.SoLuongNguPhap = db.NguPhaps.Count();
            ViewBag.SoLuongBaiTap = db.BaiTaps.Count();
            ViewBag.SoluongKyThi = db.KyThis.Count();            
            return View();
        }
        public ActionResult ManagerUser()
        {
            List<NguoiDung> tmp = db.NguoiDungs.ToList();
            return View(tmp);
        }
        public ActionResult ManagerGrammar()
        {
            List<NguPhap> tmp = db.NguPhaps.ToList();
            return View(tmp);
        }
        public ActionResult ManagerAssignment()
        {
            List<BaiTap> tmp = db.BaiTaps.ToList();

            return View(tmp);
        }

        /*Quản lý kì thi start*/
        public ActionResult ManagerExams()
        {
            List<KyThi> tmp = db.KyThis.Where(item => item.TinhTrang==false).ToList();

            return View(tmp);
        }
        public ActionResult CreateExams()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateExams(FormCollection data,List<CauHoi> LQuestion)
        {
            //Thêm kỳ thi start
            //Ngày thi
            DateTime tmp = DateTime.Parse(data["Ngay"]);
            tmp = tmp.AddHours(Convert.ToInt32(data["Hour"]));
            tmp = tmp.AddMinutes(Convert.ToInt32(data["Minute"]));

            //Ngày kết thúc
            DateTime tmp1 = DateTime.Parse(data["NgayKetThuc"]);
            tmp1 = tmp1.AddHours(Convert.ToInt32(data["HourEnd"]));
            tmp1 = tmp1.AddMinutes(Convert.ToInt32(data["MinuteEnd"]));


            KyThi newExam = new KyThi();
            newExam.IdKyThi = data["IdKyThi"];
            newExam.TenKyThi = data["TenKyThi"];
            newExam.Ngay = tmp;
            newExam.NgayKetThuc = tmp1;
            newExam.SoThiSinh = 0;
            newExam.TinhTrang = false;
            db.KyThis.Add(newExam);
            db.SaveChanges();
            //Thêm kỳ thi end

            //Thêm các câu hỏi của kỳ thi start
            foreach(var item in LQuestion)
            {
                
                CauHoi Q = new CauHoi();
                Q.IdBoCauHoi = data["IdKyThi"];
                Q.TenCauHoi = item.TenCauHoi;
                Q.DapAnA = item.DapAnA;
                Q.DapAnB = item.DapAnB;
                Q.DapAnC = item.DapAnC;
                Q.DapAnD = item.DapAnD;
                Q.DapAnDung = item.DapAnDung;
                db.CauHois.Add(Q);
                
            }
            db.SaveChanges();

            //Thêm các câu hỏi của kỳ thi end

            Session["IdKyThi"] = data["IdKyThi"];
            return View();
        }
        public ActionResult DetailExams(string id)
        {
            KyThi tmp = db.KyThis.FirstOrDefault(item=> item.IdKyThi==id);
            List<ChiTietThiSinh> ts = db.ChiTietThiSinhs.Where(item => item.IdKyThi==id).ToList();
            ViewBag.ThiSinh = ts;
            return View(tmp);
        }
        public ActionResult CongBo(string id)
        {
            KyThi tmp = db.KyThis.FirstOrDefault(item => item.IdKyThi == id);
            List<ChiTietThiSinh> ts = db.ChiTietThiSinhs.Where(item => item.IdKyThi == id).ToList();

            tmp.TinhTrang = true;
            foreach(var item in ts)
            {
                NguoiDung nd = db.NguoiDungs.FirstOrDefault(it=>it.MaKH==item.IdThiSinh);
                nd.DiemKyThi += item.Diem;
            }
            db.SaveChanges();
            foreach(var item in ts)
            {
                NguoiDung nd = db.NguoiDungs.FirstOrDefault(it => it.MaKH == item.IdThiSinh);
                List<CauHoi> ch = db.CauHois.Where(it=>it.IdBoCauHoi==tmp.IdKyThi).ToList();

                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/Temple/send2.html"));
                contentCustomer = contentCustomer.Replace("{{TenKyThi}}", tmp.TenKyThi);
                contentCustomer = contentCustomer.Replace("{{TenNd}}",nd.HoTenKH);
                contentCustomer = contentCustomer.Replace("{{IdKyThi}}", tmp.IdKyThi);
                contentCustomer = contentCustomer.Replace("{{NgayThi}}", tmp.Ngay.Value.ToString());
                contentCustomer = contentCustomer.Replace("{{Diem}}",item.Diem.ToString());
                contentCustomer = contentCustomer.Replace("{{Dung}}", item.Dung.ToString());
                contentCustomer = contentCustomer.Replace("{{Sai}}",item.Sai.ToString());
                contentCustomer = contentCustomer.Replace("{{ChuaLam}}", item.ChuaLam.ToString());
                contentCustomer = contentCustomer.Replace("{{NgayKetThuc}}", tmp.NgayKetThuc.ToString());
                contentCustomer = contentCustomer.Replace("{{SoCauHoi}}", ch.Count.ToString());
                WebBanHangOnline.Common.Common.SendMail("Admin Hoc Tieng Anh KL", "Kết quả kỳ thi " + tmp.IdKyThi, contentCustomer.ToString(), nd.Email);

            }    
            return RedirectToAction("ManagerExams");
        }
        public ActionResult DeleteExams(string id)
        {
            List<CauHoi> ch = db.CauHois.Where(item => item.IdBoCauHoi == id).ToList();
            List<ChiTietThiSinh> ts = db.ChiTietThiSinhs.Where(item => item.IdKyThi==id).ToList();
            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi==id);
            foreach(var item in ch)
            {
                db.CauHois.Remove(item);
            } 
            foreach(var item in ts)
            {
                db.ChiTietThiSinhs.Remove(item);
            }
            db.KyThis.Remove(kt);
            db.SaveChanges();


            return RedirectToAction("ManagerExams");
        }
        public ActionResult EditExams(string id)
        {
            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi == id);
            List<CauHoi> tmp = db.CauHois.Where(item => item.IdBoCauHoi == id).ToList();
            ViewBag.DanhSachCauHoi = tmp;
            return View();
        }
        /*Quản lý kì thi end*/
        public ActionResult Test()
        {
            return View();
        }
    }
}