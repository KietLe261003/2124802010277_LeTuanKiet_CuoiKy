using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _2124802010277_LeTuanKiet_CuoiKy.Models
{
    public class ListQuestion
    {
        public List<Question> LQuestion = new List<Question>();
        public ListQuestion()
        {

        }
        public void add(Question tmp)
        {
            LQuestion.Add(tmp);
        }
        public void CreateList()
        {
            Question tmp1 = new Question("1+1 bằng mấy","2","3","4","5","2");
            Question tmp2 = new Question("1+2 bằng mấy","2","3","4","5","3");
            Question tmp3 = new Question("1+3 bằng mấy","2","3","4","5","4");
            Question tmp4 = new Question("1+4 bằng mấy","2","3","4","5","5");
            LQuestion.Add(tmp1);
            LQuestion.Add(tmp2);
            LQuestion.Add(tmp3);
            LQuestion.Add(tmp4);
        }
    }
}