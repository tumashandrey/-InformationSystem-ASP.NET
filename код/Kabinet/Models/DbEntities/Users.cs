using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kabinet.Models
{
    public partial class Users
    {
        public static Users Authenticate(string login, string password)
        {
            if (string.IsNullOrEmpty(login)) return null;
            //if (string.IsNullOrEmpty(password)) return null;

            Users result = null;
            using (var db = new DbEntities())
            {
                var q = db.Users.Where(p => (p.Login == login) && (p.Password == password));
                if (q.Any()) result = q.First();
            }
            return result;
        }

        public static List<Users> LoadAll(DbEntities db)
        {
            return db.Users.ToList();
        }

        public static List<SelectListItem> LoadAllAsList(DbEntities db)
        {
            var list = new List<SelectListItem>();
            var dbList = LoadAll(db);
            foreach (var item in dbList)
            {
                list.Add(new SelectListItem { Text = item.Login, Value = item.ID.ToString() });
            }
            return list;
        }
    }
}