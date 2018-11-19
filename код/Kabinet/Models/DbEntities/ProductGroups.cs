using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kabinet.Models
{
    public partial class ProductGroups
    {
        public static List<ProductGroups> LoadAll(DbEntities db)
        {
            return db.ProductGroups.ToList();
        }

        public List<SelectListItem> ParentsList { get; set; }

        public static List<SelectListItem> LoadAllWithEmpty(DbEntities db)
        {
            var list = new List<SelectListItem> { new SelectListItem { Text = "Без группы", Value = "null"} };
            var dbList = LoadAll(db);
            foreach (var item in dbList)
            {
                list.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString()});
            }
            return list;
        }

        public static List<SelectListItem> LoadPossibleParents(DbEntities db, int id)
        {
            var list = new List<SelectListItem> { new SelectListItem { Text = "Нет родительской группы", Value = "null" } };
            var dbList = LoadAll(db);
            if (id != 0) dbList = dbList.Where(p => p.ID != id).ToList();
            foreach (var item in dbList)
            {
                list.Add(new SelectListItem { Text = item.Name, Value = item.ID.ToString() });
            }
            return list;
        }
    }
}