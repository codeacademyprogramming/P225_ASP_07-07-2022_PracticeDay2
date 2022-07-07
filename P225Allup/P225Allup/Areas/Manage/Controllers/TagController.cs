using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tags.Where(b => !b.IsDeleted).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Tag Tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _context.Tags.AnyAsync(b => !b.IsDeleted && b.Name.ToLower() == Tag.Name.ToLower().Trim()))
            {
                ModelState.AddModelError("Name", $"That {Tag.Name} Already Exists");
                return View();
            }

            Tag.Name = Tag.Name.Trim();
            Tag.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Tags.AddAsync(Tag);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Tag Tag = await _context.Tags.FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

            if (Tag == null) return NotFound();

            return View(Tag);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Tag Tag)
        {
            if (!ModelState.IsValid) return View();

            if (id == null) return BadRequest();

            if (id != Tag.Id) return BadRequest();

            Tag dbTag = await _context.Tags.FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == Tag.Id);

            if (dbTag == null) return NotFound();

            if (await _context.Tags.AnyAsync(b => !b.IsDeleted && b.Id != Tag.Id && b.Name.ToLower() == Tag.Name.Trim().ToLower()))
            {
                ModelState.AddModelError("Name", $"That {Tag.Name} Already Exists");
                return View();
            }

            if (dbTag.Name.ToLower() == Tag.Name.Trim().ToLower())
            {
                return RedirectToAction("index");
            }

            dbTag.Name = Tag.Name.Trim();
            dbTag.UpdatedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Tag Tag = await _context.Tags.FirstOrDefaultAsync(b => !b.IsDeleted && b.Id == id);

            if (Tag == null) return NotFound();

            //_context.Tags.Remove(Tag);
            Tag.IsDeleted = true;
            Tag.DeletedAt = DateTime.UtcNow.AddHours(4);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
