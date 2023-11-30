using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using _2124802010277_LeTuanKiet_CuoiKy.Models;
namespace _2124802010277_LeTuanKiet_CuoiKy.App_Start
{
    public class AdminAuthorize : AuthorizeAttribute
    {
        DataTiengAnhEntities db = new DataTiengAnhEntities();
       
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            NguoiDung adSession = (NguoiDung)HttpContext.Current.Session["TaiKhoan"];
            if (adSession != null && adSession.Admin==true)
            {
                return;
            }
            else
            {
                var retunUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(
                    new
                    {
                        controller = "User",
                        action = "Login",
                        area = "Admin",
                        retunUrl = retunUrl.ToString()
                    }));
            }
        }
    }
}