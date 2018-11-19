using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Kabinet.Models;
using Newtonsoft.Json;

namespace Kabinet
{
    /// <summary>
    /// ASMX-сервис
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        /// <summary>
        /// Получаем список товаров
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string GetProducts()
        {
            var items = new DbEntities().Products.ToList();
            return JsonConvert.SerializeObject(items);
        }
    }
}
