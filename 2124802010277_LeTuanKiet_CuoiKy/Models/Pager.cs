using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2124802010277_LeTuanKiet_CuoiKy.Models
{
    public class Pager
    {
        public int TotalItem { get; set; } //tất cả các item 
        public int CurrentPage { get; set; } //Page hiện tại đang ở
        public int PageSize { get; set; } //Kích thước mặc định của thanh pageing

        public int TotalPage { get; set; } //Tất cả các Page đang có
        public int StartPage { get; set; } //Page bắt đầu
        public int EndPage { get; set; } //Page Kết thúc 

        public Pager()
        {

        }
        public Pager(int totalitem, int page, int pagesize = 10)
        {
            //Lấy tất cả các item hiện có chia cho số trang mặc định của pageing sẽ ra tổng số trang cần có(làm tròn)
            int totalpage = (int)Math.Ceiling((decimal)totalitem / (decimal)pagesize);
            int current = page; //Trang hiện tại khi bấm vào


            int startpage = current - 5;
            int endpage = current + 4;


            //Nếu page bắt đầu bé hơn 0
            if (startpage <= 0)
            {
                startpage = 1;
                endpage = 10;
            }

            //Nếu page kết thúc lớn hơn tổng số page
            if (endpage >= totalpage)
            {
                endpage = totalpage;
                if (endpage > 10)
                    startpage = totalpage - 9;
            }

            TotalItem = totalitem;
            CurrentPage = current;
            PageSize = pagesize;
            TotalPage = totalpage;
            StartPage = startpage;
            EndPage = endpage;
        }
    }
}