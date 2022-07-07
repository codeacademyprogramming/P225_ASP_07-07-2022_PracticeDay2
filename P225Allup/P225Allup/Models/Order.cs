using P225Allup.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int No { get; set; }
        public string AppUserId { get; set; }
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime Date { get; set; }
        public double TotalPrice { get; set; }
        public string Comment { get; set; }

        public AppUser AppUser { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
