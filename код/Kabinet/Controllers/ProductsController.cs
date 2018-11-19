using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kabinet.Models;

namespace Kabinet.Controllers
{
    public class ProductsController : BaseController
    {
        /// <summary>
        /// Список товаров (заглавная страница)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new ProductsList();
            return View(model);
        }

        #region Группы

        /// <summary>
        /// Новая группа товаров
        /// </summary>
        /// <returns></returns>
        public PartialViewResult NewDepartment()
        {
            var model = new ProductGroups()
            {
                ParentsList = ProductGroups.LoadPossibleParents(Db, 0)
            };
            return PartialView("DepartmentPartial", model);
        }

        /// <summary>
        /// Редактируем группу товаров
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult EditDepartment(int id)
        {
            var model = Db.ProductGroups.First(p => p.ID == id);
            model.ParentsList = ProductGroups.LoadPossibleParents(Db, id);
            return PartialView("DepartmentPartial", model);
        }

        /// <summary>
        /// Сохраняем группу товаров
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveDepartment(ProductGroups model)
        {
            ModelState.Clear();
            model.ParentsList = ProductGroups.LoadPossibleParents(Db, model.ID);

            if (string.IsNullOrEmpty(model.Name)) ModelState.AddModelError("Name", "Заполните это поле");

            if (ModelState.IsValid)
            {
                if (model.ID == 0)
                {
                    Db.ProductGroups.Add(model);
                }
                else
                {
                    //Db.Entry(model).State = EntityState.Modified;
                    Db.Set<ProductGroups>().AddOrUpdate(model);
                    Db.SaveChanges();
                }
                Db.SaveChanges();
                return JavaScript("CloseDepartmentForm();RefreshProductsList();");
            }
            return PartialView("DepartmentPartial", model);
        }

        /// <summary>
        /// Удаляем группу товаров (спросит подтверждение)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteDepartment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductGroups departments = Db.ProductGroups.Find(id);
            if (departments == null)
            {
                return HttpNotFound();
            }
            return View(departments);
        }

        /// <summary>
        /// Удаление подтверждено
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("DeleteDepartment")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDepartmentConfirmed(int id)
        {
            ProductGroups departments = Db.ProductGroups.Find(id);
            Db.ProductGroups.Remove(departments);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Продукты

        /// <summary>
        /// Загружаем содержимое группы - товары и подгруппы
        /// </summary>
        /// <param name="department"></param>
        /// <param name="sortBy"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private DepartmentWithProducts LoadDepartmentChildsAndProducts(ProductGroups department, int? sortBy, int level)
        {
            var childDepartments = Db.ProductGroups.Where(p => p.ParentID == department.ID);
            var products = Db.Products.Where(p => p.GroupID == department.ID); 
            switch (sortBy) //сортировка
            {
                case null:
                case 0:
                    products = products.OrderBy(p => p.Name);
                    childDepartments = childDepartments.OrderBy(p => p.Name);
                    break;
                case 1:
                    products = products.OrderByDescending(p => p.Name);
                    childDepartments = childDepartments.OrderByDescending(p => p.Name);
                    break;
                case 2:
                    products = products.OrderBy(p => p.Price);
                    break;
                case 3:
                    products = products.OrderByDescending(p => p.Price);
                    break;
            }
            var childDepartmentsList = childDepartments.ToList();
            var res = new DepartmentWithProducts
            {
                DepartmentID = department.ID,
                DepartmentName = department.Name,
                ParentID = department.ParentID,
                Level = level,
                Products = products.ToList(),
            };
            if (childDepartmentsList.Any())
            {
                res.Childs = new List<DepartmentWithProducts>();
                foreach (var item in childDepartmentsList)
                {
                    var child = LoadDepartmentChildsAndProducts(item, sortBy, level + 1);
                    res.Childs.Add(child);
                }
            }
            return res;
        }

        /// <summary>
        /// Список продуктов и групп
        /// </summary>
        /// <param name="sortBy"></param>
        /// <returns></returns>
        public PartialViewResult ProductsList(int? sortBy)
        {
            var model = new List<DepartmentWithProducts>();
            var productsWithoutDepartment = Db.Products.Where(p => p.GroupID == null); //товары без группы
            switch (sortBy) //сортировка
            {
                case null:
                case 0:
                    productsWithoutDepartment = productsWithoutDepartment.OrderBy(p => p.Name);
                    break;
                case 1:
                    productsWithoutDepartment = productsWithoutDepartment.OrderByDescending(p => p.Name);
                    break;
                case 2:
                    productsWithoutDepartment = productsWithoutDepartment.OrderBy(p => p.Price);
                    break;
                case 3:
                    productsWithoutDepartment = productsWithoutDepartment.OrderByDescending(p => p.Price);
                    break;
            }
            if (productsWithoutDepartment.Any())
                model.Add(new DepartmentWithProducts
                {
                    DepartmentID = 0,
                    ParentID = null,
                    Photo = null,
                    Level = 0,
                    DepartmentName = "Без группы",
                    Products = productsWithoutDepartment.ToList()
                });

            var departmentsTop = Db.ProductGroups.Where(p => p.ParentID == null).OrderBy(p => p.Name).ToList(); //группы верхнего уровня
            foreach (var item in departmentsTop)
            {
                var newItem = LoadDepartmentChildsAndProducts(item, sortBy, 0);
                model.Add(newItem);
            }
            return PartialView("ProductsListPartial", model);
        }

        /// <summary>
        /// Новый товар
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public PartialViewResult NewProduct(int? departmentId)
        {
            var model = new Products
            {
                GroupsListWithEmpty = ProductGroups.LoadAllWithEmpty(Db),
                GroupID = departmentId
            };
            return PartialView("ProductPartial", model);
        }

        /// <summary>
        /// Редактируем товар
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PartialViewResult EditProduct(int id)
        {
            var model = Db.Products.First(p => p.ID == id);
            model.GroupsListWithEmpty = ProductGroups.LoadAllWithEmpty(Db);
            return PartialView("ProductPartial", model);
        }

        /// <summary>
        /// Сохраняем товар
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveProduct(Products model)
        {
            ModelState.Clear();
            model.GroupsListWithEmpty = ProductGroups.LoadAllWithEmpty(Db);

            if (!string.IsNullOrEmpty(model.PhotoUploaded)) //если загрузили фото товара
            {
                var pos = model.PhotoUploaded.IndexOf("base64,");
                var _photoUploaded =
                    model.PhotoUploaded.Substring(pos, model.PhotoUploaded.Length - pos).Replace("base64,", "");
                model.Photo = Convert.FromBase64String(_photoUploaded);
            }
            if (string.IsNullOrEmpty(model.Name)) ModelState.AddModelError("Name", "Заполните это поле");

            if (ModelState.IsValid) //все заполнили правильно
            {
                if (model.ID == 0)
                {
                    Db.Products.Add(model);
                }
                else
                {
                    Db.Entry(model).State = EntityState.Modified;
                    Db.SaveChanges();
                }
                Db.SaveChanges();
                return JavaScript("CloseProductForm();RefreshProductsList();");
            }
            return PartialView("ProductPartial", model);
        }

        
        public void DeleteProduct(int id)
        {
            var itemToDelete = Db.Products.FirstOrDefault(p => p.ID == id);
            if (itemToDelete != null)
            {
                Db.Products.Remove(itemToDelete);
                Db.SaveChanges();
            }
        }

        /// <summary>
        /// Удаляем товар (спросит подтверждение)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = Db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
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
            Products products = Db.Products.Find(id);
            Db.Products.Remove(products);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

    }
}