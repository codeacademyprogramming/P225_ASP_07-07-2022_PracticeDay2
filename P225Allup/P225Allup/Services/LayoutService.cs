using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P225Allup.DAL;
using P225Allup.Interfaces;
using P225Allup.Models;
using P225Allup.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Services
{
    public class LayoutService : ILayoutService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<List<BasketVM>> GetBasketAsync()
        {
            string coockie = _httpContextAccessor.HttpContext.Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(coockie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            foreach (BasketVM basketVM in basketVMs)
            {
                Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == basketVM.ProductId);

                basketVM.Image = product.MainImage;
                basketVM.Price = product.DiscountPrice > 0 ? product.DiscountPrice : product.Price;
                basketVM.ExTax = product.Extax;
                basketVM.Title = product.Title;
            }

            return basketVMs;
        }

        public async Task<IDictionary<string, string>> GetSettingsAsync()
        {
            IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);

            return settings;
        }

        public async Task<AppUser> GetUserAsync()
        {
            AppUser appUser = null;

            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                appUser = await _userManager.FindByNameAsync(_httpContextAccessor.HttpContext.User.Identity.Name);
            }

            return appUser;
        }
    }
}
