using System.Web;
using System.Web.Mvc;

namespace _2124802010277_LeTuanKiet_CuoiKy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
