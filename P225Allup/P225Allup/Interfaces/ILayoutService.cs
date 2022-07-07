using P225Allup.Models;
using P225Allup.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string, string>> GetSettingsAsync();

        Task<List<BasketVM>> GetBasketAsync();
        Task<AppUser> GetUserAsync();
    }
}
