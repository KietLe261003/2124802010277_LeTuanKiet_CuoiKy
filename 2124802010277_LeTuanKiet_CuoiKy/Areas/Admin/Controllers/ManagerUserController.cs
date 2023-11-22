using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.Areas.Admin.Controllers
{
    public class ManagerUserController : Controller
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
        // GET: Admin/ManagerUser
        public ActionResult Index()
        {
            List<NguoiDung> tmp = db.NguoiDungs.ToList();
            return View(tmp);
        }
    }
}