using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace P225Allup.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Image { get; set; }
        [Required]
        [StringLength(maximumLength:1000)]
        public string Description { get; set; }
        [Required]
        [StringLength(255)]
        public string MainTitle { get; set; }
        [Required]
        [StringLength(255)]
        public string SubTitle { get; set; }
        [Required]
        [StringLength(255)]
        public string RedirectUrl { get; set; }
    }
}
