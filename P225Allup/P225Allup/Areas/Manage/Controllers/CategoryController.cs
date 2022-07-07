using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using P225Allup.Extensions;
using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(bool? status)
        {
            IQueryable<Category> query = _context.Categories.Include(b => b.Products);

            if (status != null)
            {
                query = query.Where(b => b.IsDeleted == status);
            }

            ViewBag.Status = status;

            return View(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.MainCategoris = await _context.Categories.Where(c => c.IsMain && !c.IsDeleted).ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            ViewBag.MainCategoris = await _context.Categories.Where(c => c.IsMain && !c.IsDeleted).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (category.IsMain)
            {
                if (category.File == null)
                {
                    ModelState.AddModelError("File", "Sekil Mutleqdi");
                    return View();
                }

                if (category.File.ChechFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("File", "Sekil Novi Yalniz .jpg Ola Biler");
                    return View();
                }

                if (category.File.CheckFileSize(20))
                {
                    ModelState.AddModelError("File", "Sekil Olcusu Mutleq 20 kb");
                    return View();
                }

                category.Image = await category.File.CreateFileAsync(_env, "assets", "images");

                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Ust Categoriya Mutleq Scilmelidir");
                    return View();
                }

                if (!await _context.Categories.AnyAsync(c=>c.IsMain && !c.IsDeleted && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Ust Categoriya Duzgun Scilmelidir");
                    return View();
                }

                category.Image = null;
            }

            if (await _context.Categories.AnyAsync(c=>c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"That {category.Name} Already Exists");
                return View();
            }

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (category == null) return NotFound();

            ViewBag.MainCategoris = await _context.Categories.Where(c => c.IsMain && !c.IsDeleted).ToListAsync();

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Category category)
        {
            ViewBag.MainCategoris = await _context.Categories.Where(c => c.IsMain && !c.IsDeleted).ToListAsync();

            if (id == null) return BadRequest();

            if (id != category.Id) return BadRequest();

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound();

            if (category.IsMain)
            {
                if (category.Image == null)
                {
                    ModelState.AddModelError("Image", "Sekil Mutleqdi");
                    return View();
                }

                category.ParentId = null;
            }
            else
            {
                if (category.ParentId == null)
                {
                    ModelState.AddModelError("ParentId", "Ust Categoriya Mutleq Scilmelidir");
                    return View();
                }

                if (!await _context.Categories.AnyAsync(c => c.IsMain && !c.IsDeleted && c.Id == category.ParentId))
                {
                    ModelState.AddModelError("ParentId", "Ust Categoriya Duzgun Scilmelidir");
                    return View();
                }

                category.Image = null;
            }

            if (await _context.Categories.AnyAsync(c => c.Id != category.Id && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"That {category.Name} Already Exists");
                return View();
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.IsMain = category.IsMain;
            dbCategory.Image = category.Image;
            dbCategory.ParentId = category.ParentId;
            category.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }
    }
}
