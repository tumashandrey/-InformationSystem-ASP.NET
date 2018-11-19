using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kabinet.Models;

namespace Kabinet.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        private DbEntities _db = null;

        /// <summary>
        /// Контекст подключения к базе данных
        /// </summary>
        protected DbEntities Db => _db ?? (_db = new DbEntities());

        /// <summary>
        /// Тут храним данные залогиненного пользователя
        /// </summary>
        protected virtual new CustomPrincipal User => HttpContext.User as CustomPrincipal;
    }
}