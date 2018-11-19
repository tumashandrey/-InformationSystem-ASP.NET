using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kabinet.Models
{
    public class OrdersList
    {
        public string SearchString { get; set; }
        public byte SortBy { get; set; }

        public List<SelectListItem> SortByItems => new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "от А до Я"},
            new SelectListItem { Value = "1", Text = "от Я до А"},
            new SelectListItem { Value = "2", Text = "Цена ↓"},
            new SelectListItem { Value = "3", Text = "Цена ↑"},
        };
    }
}