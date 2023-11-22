using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(_2124802010277_LeTuanKiet_CuoiKy.Startup))]
namespace _2124802010277_LeTuanKiet_CuoiKy
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình SignalR Hub
            app.MapSignalR();

        }
    }
}