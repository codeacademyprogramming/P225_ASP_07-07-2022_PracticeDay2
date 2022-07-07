using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace P225Allup.Models
{
    public class Brand
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Mecburidi")]
        [StringLength(255, ErrorMessage ="Maksiumum 255 simvol Ola Biler")]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }


        public IEnumerable<Product> Products { get; set; }
    }
}
