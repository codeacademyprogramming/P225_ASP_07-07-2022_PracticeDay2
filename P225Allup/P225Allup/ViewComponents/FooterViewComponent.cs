using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P225Allup.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public FooterViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(IDictionary<string, string> settings)
        {
            //IDictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(x => x.Key, x => x.Value);

            return View(await Task.FromResult(settings));
        }
    }
}
