using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace P225Allup.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength:255)]
        public string Name { get; set; }
        [StringLength(maximumLength: 255)]
        public string Image { get; set; }
        public bool IsMain { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }
        //[ForeignKey(nameof(Parent))]
        //[ForeignKey("Parent")]
        public Nullable<int> ParentId { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }

        public Category Parent { get; set; }
        public IEnumerable<Category> Children { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
