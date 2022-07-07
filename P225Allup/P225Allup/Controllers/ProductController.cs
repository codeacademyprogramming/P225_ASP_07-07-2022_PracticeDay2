using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Controllers
{
    //[Authorize(Roles = "SuperAdmin,Member")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public async Task<IActionResult> Index(int pageIndex = 1)
        {
            List<Product> products = await _context.Products.ToListAsync();

            ViewBag.ProductCount = (int)Math.Ceiling((decimal)products.Count / 6);
            ViewBag.PageIndex = pageIndex;
            ViewBag.PageIndexCount = 3;

            return View(products.OrderByDescending(p=>p.Id).Skip((pageIndex-1)*6).Take(6).ToList());
        }

        public async Task<IActionResult> LoadMore(int page = 1)
        {
            List<Product> products = await _context.Products.OrderByDescending(p => p.Id).Skip((page-1)*6).Take(6).ToListAsync();
            ViewBag.PageIndex = page;
            return PartialView("_ProductIndexPartial", products);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products
                .Include(p=>p.ProductImages)
                .Include(p=>p.Brand)
                .Include(p=>p.ProductTags).ThenInclude(pt=>pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public async Task<IActionResult> DetailModal(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Product product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Brand)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return PartialView("_ProductModalPartial", product);
        }

        public async Task<IActionResult> Search(string search)
        {
            List<Product> products = await _context.Products
                .Where(p => p.Title.Trim().ToLower().Contains(search.Trim().ToLower()) || 
                        p.Brand.Name.Trim().ToLower().Contains(search.Trim().ToLower()))
                .OrderByDescending(p=>p.Id).Take(4)
                .ToListAsync();

            //return Json(products);
            return PartialView("_SearchPartial", products);
        }
    }
}
