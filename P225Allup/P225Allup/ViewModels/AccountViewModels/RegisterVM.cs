using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.ViewModels.AccountViewModels
{
    public class RegisterVM
    {
        [Required]
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        public string SurName { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        public string FatherName { get; set; }
        [Required]
        public byte Age { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        public string UserName { get; set; }
        [Required]
        [StringLength(maximumLength:255,MinimumLength =8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [StringLength(maximumLength: 255, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
