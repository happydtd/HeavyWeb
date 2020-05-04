using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Heavy.Web.ViewModels
{
    public class RoleAddViewModel
    {
        [Required]
        [Display(Name = "ROLE NAME")]
        //远程验证
        [Remote("CheckRoleExist", "Role", ErrorMessage ="Role existed")]
        public string RoleName { get; set; }
    }
}
