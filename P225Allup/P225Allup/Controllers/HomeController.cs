using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using P225Allup.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            HomeVM homeVM = new HomeVM
            {
                Sliders = await _context.Sliders.ToListAsync(),
                Categories = await _context.Categories.Where(c => c.IsMain).ToListAsync(),
                BestSeller = await _context.Products.Where(p=>p.IsBestSeller).ToListAsync(),
                Feature = await _context.Products.Where(p => p.IsFeature).ToListAsync(),
                NewArrival = await _context.Products.Where(p => p.IsNewArrivel).ToListAsync()
            };

            return View(homeVM);
        }

        //public IActionResult CreateCoockie()
        //{
        //    HttpContext.Response.Cookies.Append("p225", "Hello World P225 Coockie");

        //    return RedirectToAction("index");
        //}

        //public IActionResult GetCoockie()
        //{
        //    string coockiecontent = HttpContext.Request.Cookies["p225"];

        //    return Content(coockiecontent);
        //}

        //public async Task<IActionResult> CreateSession()
        //{
        //    HttpContext.Session.SetString("p225", "Hello World P225 Session");

        //    return RedirectToAction("Index");
        //}

        //public async Task<IActionResult> GetSession()
        //{
        //    string sessioncontent = HttpContext.Session.GetString("p225");

        //    return Content(sessioncontent);
        //}
    }
}
