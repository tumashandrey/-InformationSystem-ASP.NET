using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Kabinet.Models
{
    public partial class Orders
    {
        public List<SelectListItem> UsersList { get; set; }
        public List<SelectListItem> ProductsList { get; set; }
    }
}