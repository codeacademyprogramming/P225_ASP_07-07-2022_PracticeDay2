using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P225Allup.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(maximumLength:255)]
        public string Name { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        public string SurName { get; set; }
        [Required]
        [StringLength(maximumLength: 255)]
        public string FatherName { get; set; }
        [Required]
        public byte Age { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeActive { get; set; }
        [NotMapped]
        public string Role { get; set; }
        public IEnumerable<Order> Orders { get; set; }
    }
}
