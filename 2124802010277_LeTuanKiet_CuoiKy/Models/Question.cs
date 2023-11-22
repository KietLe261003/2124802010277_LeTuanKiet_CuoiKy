using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2124802010277_LeTuanKiet_CuoiKy.Models
{
    public class Question
    {
        public string Ques { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
        public string Ans { get; set; }
        public Question(string ques,string a,string b, string c,string d,string ans)
        {
            this.Ques = ques;
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
            this.Ans = ans;
        }
        public Question()
        {
            
        }
    }
}