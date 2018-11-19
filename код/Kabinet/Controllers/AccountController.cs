using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Kabinet.Models;

namespace Kabinet.Controllers
{
    public class AccountController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (Request.IsAuthenticated) FormsAuthentication.SignOut(); //если уже залогинены - выходим
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authUser = Users.Authenticate(model.Login, model.Password);
            if (authUser == null) //не удалось залогиниться
            {
                ModelState.AddModelError("Password", "Неправильный логин или пароль");
                return View(model);
            }
            else
            {
                //FormsAuthentication.SetAuthCookie(model.Login, true);
                var userModel = new UserModel
                {
                    ID = authUser.ID,
                    Email = authUser.Login,
                    IsAdmin = authUser.IsAdmin
                };
                AuthUser(userModel);
                return RedirectToAction("Index", "Products");
                return RedirectToLocal(returnUrl);
                //return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Аутентификация в системе
        /// </summary>
        /// <param name="userModel"></param>
        private void AuthUser(UserModel userModel)
        {
            var serializer = new JavaScriptSerializer();
            string userData = serializer.Serialize(userModel);
            var authTicket = new FormsAuthenticationTicket(
                1,
                userModel.Email,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                true,
                userData);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            Response.Cookies.Add(faCookie);
        }

        /// <summary>
        /// Разлогиниваемся
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Разлогиниваемся
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new UserModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string btnSubmit, UserModel model)
        {

            if (btnSubmit == "Save")
            {
                if (!string.IsNullOrEmpty(model.Email))
                {
                    var email = model.Email.Trim().ToLower();
                    var userInDb = Db.Users.Any(p => p.Login == email);
                    if (userInDb) ModelState.AddModelError("Email", "Пользователь с таким email уже зарегистрирован");
                    if (string.IsNullOrEmpty(model.EnteredPassword)) ModelState.AddModelError("EnteredPassword", "Заполните это поле");
                    if (string.IsNullOrEmpty(model.EnteredPassword2)) ModelState.AddModelError("EnteredPassword2", "Заполните это поле");
                    if ((!string.IsNullOrEmpty(model.EnteredPassword)) &&
                        (!string.IsNullOrEmpty(model.EnteredPassword2)))
                    {
                        if (model.EnteredPassword != model.EnteredPassword2) ModelState.AddModelError("EnteredPassword2", "Пароли не совпадают");
                    }
                    if (ModelState.IsValid)
                    {
                        var newUser = new Users
                        {
                            Login = email,
                            Password = model.EnteredPassword,
                        };
                        Db.Users.Add(newUser); //добавляем пользователя в базу
                        Db.SaveChanges();
                        model.ID = newUser.ID;
                        model.Email = email;
                        model.IsAdmin = newUser.IsAdmin;
                        AuthUser(model);
                        //return RedirectToAction("Login", "Account");
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(model);
        }
    }
}