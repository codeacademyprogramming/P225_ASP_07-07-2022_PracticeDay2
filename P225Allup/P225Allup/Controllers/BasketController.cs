using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using P225Allup.DAL;
using P225Allup.Models;
using P225Allup.ViewModels.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string basket = Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            }
            else
            {
                basketVMs = new List<BasketVM>();
            }

            return View(await _getBasketAsync(basket));
        }

        public async Task<IActionResult> GetBasket()
        {
            string basket = Request.Cookies["basket"];

            if (string.IsNullOrWhiteSpace(basket))
            {
                return BadRequest();
            }

            return PartialView("_BasktePartial", await _getBasketAsync(basket));
        }

        public async Task<IActionResult> AddToBasket(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            List<BasketVM> basketVMs = null;

            string coockie = HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(coockie))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

                if (basketVMs.Exists(b=>b.ProductId == id))
                {
                    basketVMs.Find(b => b.ProductId == id).Count++;
                }
                else
                {
                    BasketVM basketVM = new BasketVM
                    {
                        ProductId = (int)id,
                        Count = 1
                    };

                    basketVMs.Add(basketVM);
                }
            }
            else
            {
                basketVMs = new List<BasketVM>();

                BasketVM basketVM = new BasketVM
                {
                    ProductId = (int)id,
                    Count = 1
                };

                basketVMs.Add(basketVM);
            }

            string item = JsonConvert.SerializeObject(basketVMs);

            HttpContext.Response.Cookies.Append("basket", item);

            return PartialView("_BasktePartial", await _getBasketAsync(item));
        }

        public async Task<IActionResult> UpdateFromCart(int? id, int count = 1)
        {

            string basket = Request.Cookies["basket"];

            List<BasketVM> basketVMs = null;

            if (!string.IsNullOrWhiteSpace(basket))
            {
                basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(basket);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id);

                if (basketVM == null) return NotFound();

                basketVM.Count = count;

                basket = JsonConvert.SerializeObject(basketVMs);

                Response.Cookies.Append("basket", basket);
            }
            else
            {
                return BadRequest();
            }

            return PartialView("_BasketIndexPartial", await _getBasketAsync(basket));
        }

        public async Task<IActionResult> DeleteFromBasket(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();

            string coockie = HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(coockie))
            {
                List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id);

                if (basketVM == null) return NotFound();

                basketVMs.Remove(basketVM);

                coockie = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", coockie);

                return PartialView("_BasktePartial",await _getBasketAsync(coockie));
            }
            else
            {
                return BadRequest();
            }
        }
        public async Task<IActionResult> DeleteFromCart(int? id)
        {
            if (id == null) return BadRequest();

            if (!await _context.Products.AnyAsync(p => p.Id == id)) return NotFound();

            string coockie = HttpContext.Request.Cookies["basket"];

            if (!string.IsNullOrWhiteSpace(coockie))
            {
                List<BasketVM> basketVMs = JsonConvert.DeserializeObject<List<BasketVM>>(coockie);

                BasketVM basketVM = basketVMs.FirstOrDefault(b => b.ProductId == id);

                if (basketVM == null) return NotFound();

                basketVMs.Remove(basketVM);

                coockie = JsonConvert.SerializeObject(basketVMs);

                HttpContext.Response.Cookies.Append("basket", coockie);

                return PartialView("_BasketIndexPartial", await _getBasketAsync(coockie));
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<List<BasketVM>> _getBasketAsync(string coockie)
        {
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
    }
}
