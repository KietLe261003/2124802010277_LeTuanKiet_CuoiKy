using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.App_Start;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class ManagerExamsController : Controller
    {
        // GET: Admin/ManagerExams
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        [AdminAuthorize()]
        public ActionResult Index(string KeyWork,int pg=1)
        {
            const int pageSize = 10;
            if (pg < 1)
                pg = 1;
            if (!string.IsNullOrEmpty(KeyWork))
            {
                List<KyThi> tmp1 = db.KyThis.OrderByDescending(item => item.NgayKetThuc).Where(item=> item.IdKyThi.Contains(KeyWork) || item.TenKyThi.Contains(KeyWork)).ToList();
                int totalitem1 = tmp1.Count();
                var pager1 = new Pager(totalitem1, pg, pageSize);
                int skipPage1 = (pg - 1) * pageSize;
                ViewBag.Pager = pager1;
                ViewBag.pg = pg;
                return View(tmp1);
            }    
            List<KyThi> tmp = db.KyThis.OrderByDescending(item => item.NgayKetThuc).ToList();
            int totalitem = tmp.Count();
            var pager = new Pager(totalitem, pg, pageSize);
            int skipPage = (pg - 1) * pageSize;
            ViewBag.Pager = pager;
            ViewBag.pg = pg;
            List<KyThi> tmp2 = tmp.Skip(skipPage).Take(pageSize).ToList();
            return View(tmp2);
        }
        [AdminAuthorize()]
        public ActionResult CreateExams()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateExams(FormCollection data, List<CauHoi> LQuestion)
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

            var Ckythi = db.KyThis.FirstOrDefault(item => item.IdKyThi == newExam.IdKyThi);
            if(tmp==null || string.IsNullOrEmpty(newExam.IdKyThi) || Ckythi!=null || tmp1==null)
            {
                return RedirectToAction("Fail");
            }    

            db.SaveChanges();
            //Thêm kỳ thi end

            //Thêm các câu hỏi của kỳ thi start
            foreach (var item in LQuestion)
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
            return RedirectToAction("Index");
        }
        [AdminAuthorize()]
        public ActionResult DetailExams(string id)
        {
            KyThi tmp = db.KyThis.FirstOrDefault(item => item.IdKyThi == id);
            List<ChiTietThiSinh> ts = db.ChiTietThiSinhs.Where(item => item.IdKyThi == id).ToList();
            ViewBag.ThiSinh = ts;
            return View(tmp);
        }
        [AdminAuthorize()]
        public ActionResult CongBo(string id)
        {
            KyThi tmp = db.KyThis.FirstOrDefault(item => item.IdKyThi == id);
            List<ChiTietThiSinh> ts = db.ChiTietThiSinhs.Where(item => item.IdKyThi == id).ToList();

            tmp.TinhTrang = true;
            foreach (var item in ts)
            {
                NguoiDung nd = db.NguoiDungs.FirstOrDefault(it => it.MaKH == item.IdThiSinh);
                nd.DiemKyThi += item.Diem;
            }
            db.SaveChanges();
            foreach (var item in ts)
            {
                NguoiDung nd = db.NguoiDungs.FirstOrDefault(it => it.MaKH == item.IdThiSinh);
                List<CauHoi> ch = db.CauHois.Where(it => it.IdBoCauHoi == tmp.IdKyThi).ToList();

                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/Temple/send2.html"));
                contentCustomer = contentCustomer.Replace("{{TenKyThi}}", tmp.TenKyThi);
                contentCustomer = contentCustomer.Replace("{{TenNd}}", nd.HoTenKH);
                contentCustomer = contentCustomer.Replace("{{IdKyThi}}", tmp.IdKyThi);
                contentCustomer = contentCustomer.Replace("{{NgayThi}}", tmp.Ngay.Value.ToString());
                contentCustomer = contentCustomer.Replace("{{Diem}}", item.Diem.ToString());
                contentCustomer = contentCustomer.Replace("{{Dung}}", item.Dung.ToString());
                contentCustomer = contentCustomer.Replace("{{Sai}}", item.Sai.ToString());
                contentCustomer = contentCustomer.Replace("{{ChuaLam}}", item.ChuaLam.ToString());
                contentCustomer = contentCustomer.Replace("{{NgayKetThuc}}", tmp.NgayKetThuc.ToString());
                contentCustomer = contentCustomer.Replace("{{SoCauHoi}}", ch.Count.ToString());
                WebBanHangOnline.Common.Common.SendMail("Admin Hoc Tieng Anh KL", "Kết quả kỳ thi " + tmp.IdKyThi, contentCustomer.ToString(), nd.Email);

            }
            return RedirectToAction("Index");
        }
        [AdminAuthorize()]
        public ActionResult DeleteExams(string id)
        {
            List<CauHoi> ch = db.CauHois.Where(item => item.IdBoCauHoi == id).ToList();
            List<ChiTietThiSinh> ts = db.ChiTietThiSinhs.Where(item => item.IdKyThi == id).ToList();
            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi == id);
            foreach (var item in ch)
            {
                db.CauHois.Remove(item);
            }
            foreach (var item in ts)
            {
                db.ChiTietThiSinhs.Remove(item);
            }
            db.KyThis.Remove(kt);
            db.SaveChanges();


            return RedirectToAction("Index");
        }
        [AdminAuthorize()]
        public ActionResult EditExams(string id)
        {
            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi == id);
            List<CauHoi> tmp = db.CauHois.Where(item => item.IdBoCauHoi == id).ToList();
            ViewBag.DanhSachCauHoi = tmp;
            return View(kt);
        }
        [AdminAuthorize()]
        [HttpPost] 
        public ActionResult EditExams(FormCollection f,List<CauHoi> LQuestion)
        {
            string IdKyThi = f["IdKyThi"];
            string TenKyThi = f["TenKyThi"];
            DateTime Ngay = DateTime.Parse(f["Ngay"]);
            Ngay = Ngay.AddHours(Convert.ToInt32(f["Hour"]));
            Ngay = Ngay.AddMinutes(Convert.ToInt32(f["Minute"]));

            DateTime NgayKt = DateTime.Parse(f["NgayKetThuc"]);
            NgayKt = NgayKt.AddHours(Convert.ToInt32(f["HourEnd"]));
            NgayKt = NgayKt.AddMinutes(Convert.ToInt32(f["MinuteEnd"]));

            Boolean TinhTrang = Boolean.Parse(f["TinhTrang"]);

            KyThi kt = db.KyThis.FirstOrDefault(item => item.IdKyThi == IdKyThi);
            kt.TenKyThi = TenKyThi;
            kt.Ngay = Ngay;
            kt.NgayKetThuc = NgayKt;

            kt.CauHois.Clear();
            List<CauHoi> lq = LQuestion.Where(item => item.TenCauHoi!=null && item.DapAnA!=null && item.DapAnB!=null && item.DapAnC!=null && item.DapAnD!=null && item.DapAnDung!=null ).ToList();
            foreach(var item in lq)
            {
                item.IdBoCauHoi = IdKyThi;
            }    
            kt.CauHois = lq;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        /*Quản lý kì thi end*/

        public ActionResult Fail()
        {
            return View();
        }
    }
}