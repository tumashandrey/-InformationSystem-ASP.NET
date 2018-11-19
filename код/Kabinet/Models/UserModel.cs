using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Kabinet.Models
{
    public class UserModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Заполните это поле")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Заполните это поле")]
        public string EnteredPassword { get; set; }

        //[Required(ErrorMessage = "Подтвердите пароль")]
        //[Compare("EnteredPassword", ErrorMessage = "Пароль и подтверждение не совпадают!")]
        public string EnteredPassword2 { get; set; }

        public string OldPassword { get; set; }

        //[Required(ErrorMessage = "Заполните это поле")]
        public string UserName { get; set; }
        public bool IsAdmin { get; set; }
    }
}