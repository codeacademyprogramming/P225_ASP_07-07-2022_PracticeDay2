using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace P225Allup.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [Required]
        [Column(TypeName ="money")]
        public double Price { get; set; }
        [Column(TypeName = "money")]
        public double DiscountPrice { get; set; }
        [Required]
        [Column(TypeName = "money")]
        public double Extax { get; set; }
        public int Code { get; set; }
        [StringLength(4)]
        public string Seria { get; set; }
        public int Count { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        [StringLength(255)]
        public string MainImage { get; set; }
        [StringLength(255)]
        public string SecondImage { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public bool IsNewArrivel { get; set; }
        public bool IsBestSeller { get; set; }
        public bool IsFeature { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<DateTime> CreatedAt { get; set; }
        public Nullable<DateTime> UpdatedAt { get; set; }
        public Nullable<DateTime> DeletedAt { get; set; }

        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public IEnumerable<ProductTag> ProductTags { get; set; }

        [NotMapped]
        public IFormFile MainFile { get; set; }
        [NotMapped]
        public IFormFile SecondFile { get; set; }
        [NotMapped]
        public List<IFormFile> Files { get; set; }
        [NotMapped]
        public List<int> TagIds { get; set; }

    }
}
