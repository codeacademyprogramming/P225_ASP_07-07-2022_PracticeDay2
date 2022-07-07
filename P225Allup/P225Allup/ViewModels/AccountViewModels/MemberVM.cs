using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.ViewModels.AccountViewModels
{
    public class MemberVM
    {
        public ProfileVM ProfileVM { get; set; }
        public List<Order> Orders { get; set; }
    }
}
