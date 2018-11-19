using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kabinet.Models
{
    public class DepartmentWithProducts
    {
        public int DepartmentID { get; set; }
        public int? ParentID { get; set; }
        public int Level { get; set; }
        public string DepartmentName { get; set; }
        public byte[] Photo { get; set; }
        public List<Products> Products { get; set; }
        public List<DepartmentWithProducts> Childs { get; set; }
        public string ShortName => DepartmentName.Length >= 2 ? DepartmentName.Substring(0, 2) : DepartmentName;
    }
}