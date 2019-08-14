using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DCRSystem.DA
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BagdeNo { get; set; }
        public string Roles { get; set; }
        [Required]
        //[StringLength(18, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}