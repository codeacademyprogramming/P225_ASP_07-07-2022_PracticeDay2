using P225Allup.Models;
using P225Allup.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.ViewModels.OrderViewModels
{
    public class OrderVM
    {
        public Order  Order { get; set; }
        public List<BasketVM> BasketVMs { get; set; }
    }
}
