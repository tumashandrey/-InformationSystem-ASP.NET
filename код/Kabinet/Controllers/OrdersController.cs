using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kabinet.Models;
using Newtonsoft.Json;

namespace Kabinet.Controllers
{
    public class OrdersController : BaseController
    {
        /// <summary>
        /// Список продаж (заглавная страница)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region Продажи

        /// <summary>
        /// Список продаж
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public PartialViewResult OrdersList(int? sortBy)
        {
            var model = Db.Orders.ToList();
            return PartialView("OrdersListPartial", model);
        }

        public PartialViewResult NewOrder()
        {
            var model = new Orders
            {
                CreatedDate = DateTime.Now,
                UserID = User.ID,
                UsersList = Users.LoadAllAsList(Db),
                ProductsList = Products.LoadAllAsList(Db)
            };
            return PartialView("OrderPartial", model);
        }

        /// <summary>
        /// Редактируем продажу
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult EditOrder(int id)
        {
            var model = Db.Orders.First(p => p.ID == id);
            model.UsersList = Users.LoadAllAsList(Db);
            model.ProductsList = Products.LoadAllAsList(Db);
            //model.GroupsListWithEmpty = ProductGroups.LoadAllWithEmpty(Db);
            return PartialView("OrderPartial", model);
        }

        /// <summary>
        /// Сохраняем продажу в базу
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveOrder(Orders model)
        {
            ModelState.Clear();
            //model.UsersList = Users.LoadAll(Db)

            //if (string.IsNullOrEmpty(model.Name)) ModelState.AddModelError("Name", "Заполните это поле");

            if (ModelState.IsValid) //если все ввели верно
            {
                if (model.ID == 0)
                {
                    Db.Orders.Add(model);
                }
                else
                {
                    Db.Entry(model).State = EntityState.Modified;
                    Db.SaveChanges();
                }
                Db.SaveChanges();
                return JavaScript("CloseOrderForm();RefreshOrdersList();"); //через ajax обновляем список
            }
            return PartialView("OrderPartial", model);
        }

        /// <summary>
        /// Удаеляем продажу (спросит подтверждение)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = Db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        /// <summary>
        /// Удаление подтверждено
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = Db.Orders.Find(id);
            Db.Orders.Remove(orders); //удаляем из базы
            Db.SaveChanges(); 
            return RedirectToAction("Index");
        }
        #endregion

        /// <summary>
        /// Метод api - получаем список продаж за заданный срок
        /// </summary>
        /// <param name="startDate">от</param>
        /// <param name="endDate">до</param>
        /// <returns>список продаж</returns>
        [AllowAnonymous]
        public string GetOrders(DateTime startDate, DateTime endDate)
        {
            var items = Db.Orders.Where(p => (p.CreatedDate >= startDate) && (p.CreatedDate <= endDate)).ToList();
            return JsonConvert.SerializeObject(items);
        }
    }
}