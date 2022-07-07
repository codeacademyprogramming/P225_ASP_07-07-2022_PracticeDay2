using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.ViewModels.AccountViewModels
{
    public class LoginVM
    {
        [Required]
        [StringLength(maximumLength: 255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 255, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RemindMe { get; set; }
    }
}
