//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace _2124802010277_LeTuanKiet_CuoiKy.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChiTietPhong
    {
        public int Id { get; set; }
        public Nullable<int> MaKH { get; set; }
        public Nullable<int> IdPhong { get; set; }
        public Nullable<bool> ChuPhong { get; set; }
    
        public virtual Room Room { get; set; }
    }
}
