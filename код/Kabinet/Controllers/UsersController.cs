using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kabinet.Models;

namespace Kabinet.Controllers
{
    public class UsersController : BaseController
    {
        /// <summary>
        /// Список пользователей
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию
            return View(Db.Users.ToList());
        }

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Login,Password,IsAdmin")] Users users)
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию

            if (ModelState.IsValid)
            {
                Db.Users.Add(users);
                Db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(users);
        }

        /// <summary>
        /// Редактирование пользователя
        /// </summary>
        /// <param name="id">id из базы</param>
        /// <returns></returns>
        public ActionResult Edit(int? id)
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = Db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Login,Password,IsAdmin")] Users users)
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию

            if (ModelState.IsValid)
            {
                Db.Entry(users).State = EntityState.Modified;
                Db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(users);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = Db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            return View(users);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!User.IsAdmin) return null; //если не админ то нельзя использовать эту функцию

            Users users = Db.Users.Find(id);
            Db.Users.Remove(users);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
