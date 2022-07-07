using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BrandController : Controller
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(bool? status)
        {
            IQueryable<Brand> query = _context.Brands.Include(b=>b.Products);

            if (status != null)
            {
                query = query.Where(b => b.IsDeleted == status);
            }

            ViewBag.Status = status;

            return View(await query.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Brands.AnyAsync(b=> b.Name.ToLower() == brand.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", $"That {brand.Name} Already Exists");
                return View();
            }

            brand.Name = brand.Name.Trim();
            brand.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{brand.Name} Adli Brand Ugurla Yaradildi";

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null) return NotFound();

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Brand brand)
        {
            if (!ModelState.IsValid) return View();

            if (id == null) return BadRequest();

            if (id != brand.Id) return BadRequest();

            Brand dbBrand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == brand.Id);

            if (dbBrand == null) return NotFound();

            if (await _context.Brands.AnyAsync(b=> b.Id != brand.Id && b.Name.ToLower() == brand.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"That {brand.Name} Already Exists");
                return View();
            }

            if (dbBrand.Name.ToLower() == brand.Name.Trim().ToLower())
            {
                return RedirectToAction("index");
            }

            dbBrand.Name = brand.Name.Trim();
            dbBrand.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int? id, bool? status)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

            if (brand == null) return NotFound();

            //_context.Brands.Remove(brand);
            brand.IsDeleted = true;
            brand.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            IQueryable<Brand> query = _context.Brands.Include(b => b.Products);

            if (status != null)
            {
                query = query.Where(b => b.IsDeleted == status);
            }

            ViewBag.Status = status;

            return PartialView("_BrandIndexPartial",await query.ToListAsync());
        }

        public async Task<IActionResult> Restore(int? id, bool? status)
        {
            if (id == null) return BadRequest();

            Brand brand = await _context.Brands.FirstOrDefaultAsync(b => b.IsDeleted && b.Id == id);

            if (brand == null) return NotFound();

            //_context.Brands.Remove(brand);
            brand.IsDeleted = false;
            brand.DeletedAt = null;
            await _context.SaveChangesAsync();

            IQueryable<Brand> query = _context.Brands.Include(b => b.Products);

            if (status != null)
            {
                query = query.Where(b => b.IsDeleted == status);
            }

            ViewBag.Status = status;

            return PartialView("_BrandIndexPartial", await query.ToListAsync());
        }
    }
}
