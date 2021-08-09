using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CFSWebService.Models
{

    public class UserInfoViewModel
    {
        public string UserName { get; set; }
        public bool HasRegistered { get; set; }
        public string LoginProvider { get; set; }
    }

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string State { get; set; }
    }
    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }


}