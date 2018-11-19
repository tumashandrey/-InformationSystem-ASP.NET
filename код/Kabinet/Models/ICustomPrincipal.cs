using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Kabinet.Models
{
    public interface ICustomPrincipal : IPrincipal
    {
        int ID { get; set; }
        string Email { get; set; }
        string UserName { get; set; }
        bool IsAdmin { get; set; }
    }
}