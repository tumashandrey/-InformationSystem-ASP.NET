using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kabinet.Models
{
    public partial class Products
    {
       public List<SelectListItem> GroupsListWithEmpty { get; set; }
        public string ShortName => Name.Length >= 2 ? Name.Substring(0, 2) : Name;

        public string PhotoUploaded { get; set; }
        //private HttpPostedFileBase _photoUploadedFile { get; set; }
        //public HttpPostedFileBase PhotoUploadedFile
        //{
        //    get { return _photoUploadedFile; }
        //    set
        //    {
        //        _photoUploadedFile = value;
        //        if ((value != null) && (value.ContentLength > 0))
        //        {
        //            Photo = new byte[value.ContentLength];
        //            value.InputStream.Read(Photo, 0, Photo.Length);
        //        }
        //    }
        //}

        public static List<Products> LoadAll(DbEntities db)
        {
            return db.Products.ToList();
        }

        public static List<SelectListItem> LoadAllAsList(DbEntities db)
        {
            var list = new List<SelectListItem>();
            var dbList = LoadAll(db);
            foreach (var item in dbList)
            {
                list.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
            }
            return list;
        }
    }
}