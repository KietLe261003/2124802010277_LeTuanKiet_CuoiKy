using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Controllers
{
    public class GrammarController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        public List<NguPhap> FilterG(int op)
        {
            NguoiDung nd = (NguoiDung)Session["TaiKhoan"];
            List<NguPhap> tmp = db.NguPhaps.Where(item => item.TrangThai == true).ToList();
            if(op==1)
            {
                tmp = db.NguPhaps.OrderByDescending(item => item.NgayDang).Where(item => item.TrangThai == true).ToList();
            }    
            else if(op==2)
            {
                tmp = db.NguPhaps.OrderBy(item => item.NgayDang).Where(item => item.TrangThai == true).ToList();
            }  
            else if(op==3)
            {
                tmp = db.NguPhaps.OrderByDescending(item => item.LuotXem).Where(item => item.TrangThai == true).ToList();
            }  
            else if(op==4)
            {
                tmp = db.NguPhaps.OrderBy(item => item.LuotXem).Where(item => item.TrangThai == true).ToList();
            }    
            else if(op==5)
            {
                if(nd!=null)
                {
                    tmp = db.NguPhaps.Where(item => item.IdTacGia == nd.MaKH).ToList();
                }    
            }    
            return tmp;
        }
        // GET: Grammar
        public ActionResult Index(string KeyWork, int pg = 1,int fillter=0)
        {
            List<NguPhap> tmp = db.NguPhaps.Where(item => item.TrangThai == true).ToList();

            if(fillter!=0)
            {
                tmp = FilterG(fillter);
            }    

            if (!string.IsNullOrEmpty(KeyWork))
            {
                tmp = db.NguPhaps.Where(item => item.TrangThai==true && (item.TenNguPhap.Contains(KeyWork) || db.NguoiDungs.FirstOrDefault(x=>x.MaKH==item.IdTacGia).HoTenKH.Contains(KeyWork))).ToList();
            }    
            const int PageSize = 9;
            if(pg<1)
            {
                pg = 1;
            }
            int totalPage = tmp.Count();
            Pager p = new Pager(totalPage,pg,PageSize);
            int skipPage = (pg - 1) * PageSize;
            List<NguPhap> tmp1 = tmp.Skip(skipPage).Take(PageSize).ToList();
            if (fillter==0)
            {
                tmp1 = tmp.OrderBy(item => item.IdNguPhap).Skip(skipPage).Take(PageSize).ToList();
            }    
            ViewBag.Pager = p;
            ViewBag.fillter = fillter;
            ViewBag.TaiKhoan = db.NguoiDungs.ToList();
            if (TempData["ThongBaoThem"] != null)
                ViewBag.ThongBaoThem = TempData["ThongBaoThem"];
            return View(tmp1);
        }

        /*xử lý khi thêm 1 ngữ pháp mới start*/ 
        public ActionResult AddGrammar()
        {
            if (Session["TaiKhoan"] == null)
                return RedirectToAction("DangNhap", "User");
            return View();
        }

        public string Truncate(string s, int length)
        {
            if (s.Length < length)
            {
                return s;
            }
            else
            {
                return s.Substring(0, length);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddGrammar(FormCollection data)
        {
            NguPhap np = new NguPhap();

            var sIdTacGia = (NguoiDung)Session["TaiKhoan"];
            var sTen = data["TenNguPhap"];
            var sNoiDung = data["NoiDung"];
            var sMoTa = data["MoTa"];

            np.IdTacGia = sIdTacGia.MaKH;
            np.TenNguPhap = Truncate(sTen,100);
            np.NoiDung = sNoiDung;
            np.MoTaNgan = sMoTa;
            np.LuotXem = 0;
            np.LuotThich = 0;
            np.NgayDang = DateTime.Now;
            if (sIdTacGia.Admin == true)
                np.TrangThai = true;
            else
                np.TrangThai = false;
            db.NguPhaps.Add(np);
            db.SaveChanges();
            TempData["ThongBaoThem"] = "We have recorded your document, please wait for moderation for a few minutes";
            return RedirectToAction("Index");
        }
        /*Xử lý khi thêm 1 ngữ pháp mới kết thúc*/

        /*Chi tiết 1 ngữ pháp start*/
        public ActionResult DetailGrammar(int id)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);  
            return View(tmp);
        }
        public ActionResult MucLuc()
        {
            List<NguPhap> tmp = db.NguPhaps.Where(item=> item.TrangThai==true).ToList();
            return PartialView(tmp);
        }
        
        public ActionResult IncreaseView(int id)
        {
            NguPhap tmp = db.NguPhaps.FirstOrDefault(item => item.IdNguPhap == id);
            if (tmp != null)
            {
                tmp.LuotXem += 1;
                db.SaveChanges();
            }
            return RedirectToAction("DetailGrammar",new { id=id});
        }
        /*Chi tiết 1 ngữ pháp end*/
    }
}