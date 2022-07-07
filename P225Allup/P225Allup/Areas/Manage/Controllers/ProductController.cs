using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P225Allup.DAL;
using P225Allup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P225Allup.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace P225Allup.Areas.Manage.Controllers
{
    [Area("manage")]
    [Authorize(Roles ="SuperAdmin,Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index(bool? status)
        {
            IQueryable<Product> query = _context.Products;

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
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted && !c.IsMain).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t=>!t.IsDeleted).ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted && !c.IsMain).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

            if (!ModelState.IsValid) return View();

            if (!await _context.Categories.AnyAsync(c=>!c.IsDeleted && !c.IsMain && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Please Select A Correct Category");
                return View();
            }

            if (!await _context.Brands.AnyAsync(c => !c.IsDeleted &&  c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Please Select A Correct Brand");
                return View();
            }

            if (product.TagIds != null && product.TagIds.Count > 0)
            {
                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t=>!t.IsDeleted && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"Tag Id {tagId} Is InCorrec");
                        return View();
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };

                    productTags.Add(productTag);
                }

                product.ProductTags = productTags;
            }
            else
            {
                ModelState.AddModelError("TagIds", "Tags Is Requered");
                return View();
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.ChechFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Please Select Correct Image Type. Must Be .jpg or .jpeg");
                    return View();
                }

                if (product.MainFile.CheckFileSize(50))
                {
                    ModelState.AddModelError("MainFile", "Please Select Correct Image Size. Must Be Max 50kb");
                    return View();
                }

                product.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("MainFile", "Please Select Main Image");
                return View();
            }

            if (product.SecondFile != null)
            {
                if (product.SecondFile.ChechFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("SecondFile", "Please Select Correct Image Type. Must Be .jpg or .jpeg");
                    return View();
                }

                if (product.SecondFile.CheckFileSize(50))
                {
                    ModelState.AddModelError("SecondFile", "Please Select Correct Image Size. Must Be Max 50kb");
                    return View();
                }

                product.SecondImage = await product.SecondFile.CreateFileAsync(_env, "assets", "images", "product");
            }
            else
            {
                ModelState.AddModelError("SecondFile", "Please Select Second Image");
                return View();
            }

            if (product.Files != null && product.Files.Count > 0)
            {
                if (product.Files.Count > 5)
                {
                    ModelState.AddModelError("Files", "Can Select Maximum 5 Image");
                    return View();
                }

                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (file.ChechFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", "Please Select Correct Image Type. Must Be .jpg or .jpeg");
                        return View();
                    }

                    if (file.CheckFileSize(50))
                    {
                        ModelState.AddModelError("Files", "Please Select Correct Image Size. Must Be Max 50kb");
                        return View();
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "images", "product-quick")
                    };

                    productImages.Add(productImage);
                }

                product.ProductImages = productImages;
            }
            else
            {
                ModelState.AddModelError("Files", "Please Select Files");
                return View();
            }
            product.Title = product.Title.Trim();

            string seria = (_context.Brands.FirstOrDefault(b => b.Id == product.BrandId).Name.Substring(0, 2) + product.Title.Substring(0, 2)).ToLower();
            int code = _context.Products.OrderByDescending(p => p.Id).FirstOrDefault(p => !p.IsDeleted && p.Seria == seria) != null ? _context.Products.OrderByDescending(p => p.Id).FirstOrDefault(p => !p.IsDeleted && p.Seria == seria).Code += 1 : 1;

            product.Seria = seria;
            product.Code = code;
            product.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

            if (product == null) return NotFound();

            product.TagIds = product.ProductTags.Select(x => x.TagId).ToList();

            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted && !c.IsMain).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int? id, Product product)
        {
            ViewBag.Categories = await _context.Categories.Where(c => !c.IsDeleted && !c.IsMain).ToListAsync();
            ViewBag.Brands = await _context.Brands.Where(b => !b.IsDeleted).ToListAsync();
            ViewBag.Tags = await _context.Tags.Where(t => !t.IsDeleted).ToListAsync();

            if (!ModelState.IsValid) return View();

            if (id == null) return BadRequest();

            if (id != product.Id) return BadRequest();

            Product dbProduct = await _context.Products
                .Include(p=>p.ProductTags)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == id);

            if (dbProduct == null) return NotFound();

            if (!await _context.Categories.AnyAsync(c => !c.IsDeleted && !c.IsMain && c.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Please Select A Correct Category");
                return View();
            }

            if (!await _context.Brands.AnyAsync(c => !c.IsDeleted && c.Id == product.BrandId))
            {
                ModelState.AddModelError("BrandId", "Please Select A Correct Brand");
                return View();
            }

            if (product.TagIds != null && product.TagIds.Count > 0)
            {
                if (dbProduct.ProductTags != null && dbProduct.ProductTags.Count() > 0)
                {
                    _context.ProductTags.RemoveRange(dbProduct.ProductTags);
                }

                List<ProductTag> productTags = new List<ProductTag>();

                foreach (int tagId in product.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(t => !t.IsDeleted && t.Id == tagId))
                    {
                        ModelState.AddModelError("TagIds", $"Tag Id {tagId} Is InCorrec");
                        return View();
                    }

                    ProductTag productTag = new ProductTag
                    {
                        TagId = tagId
                    };

                    productTags.Add(productTag);
                }

                dbProduct.ProductTags = productTags;
            }
            else
            {
                ModelState.AddModelError("TagIds", "Tags Is Requered");
                return View();
            }

            int canselectimage = 5 - dbProduct.ProductImages.Count;

            if (product.Files != null && product.Files.Count > 0 && canselectimage < product.Files.Count)
            {
                ModelState.AddModelError("Files", $"Can Select Maximum {canselectimage} Image");
                return View(dbProduct);
            }

            if (product.MainFile != null)
            {
                if (product.MainFile.ChechFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("MainFile", "Please Select Correct Image Type. Must Be .jpg or .jpeg");
                    return View();
                }

                if (product.MainFile.CheckFileSize(50))
                {
                    ModelState.AddModelError("MainFile", "Please Select Correct Image Size. Must Be Max 50kb");
                    return View();
                }

                string fullpath = Path.Combine(_env.WebRootPath, "assets", "images", "product", dbProduct.MainImage);

                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }

                dbProduct.MainImage = await product.MainFile.CreateFileAsync(_env, "assets", "images", "product");
            }

            if (product.SecondFile != null)
            {
                if (product.SecondFile.ChechFileContentType("image/jpeg"))
                {
                    ModelState.AddModelError("SecondFile", "Please Select Correct Image Type. Must Be .jpg or .jpeg");
                    return View();
                }

                if (product.SecondFile.CheckFileSize(50))
                {
                    ModelState.AddModelError("SecondFile", "Please Select Correct Image Size. Must Be Max 50kb");
                    return View();
                }

                string fullpath = Path.Combine(_env.WebRootPath, "assets", "images", "product", dbProduct.SecondImage);

                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }

                dbProduct.SecondImage = await product.SecondFile.CreateFileAsync(_env, "assets", "images", "product");
            }

            if (product.Files != null && product.Files.Count > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.Files)
                {
                    if (file.ChechFileContentType("image/jpeg"))
                    {
                        ModelState.AddModelError("Files", "Please Select Correct Image Type. Must Be .jpg or .jpeg");
                        return View();
                    }

                    if (file.CheckFileSize(50))
                    {
                        ModelState.AddModelError("Files", "Please Select Correct Image Size. Must Be Max 50kb");
                        return View();
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Image = await file.CreateFileAsync(_env, "assets", "images", "product-quick")
                    };

                    productImages.Add(productImage);
                }
                if (dbProduct.ProductImages != null && dbProduct.ProductImages.Count() >= 0)
                {
                    dbProduct.ProductImages.AddRange(productImages);
                }
                else
                {
                    dbProduct.ProductImages = productImages;
                }
            }

            dbProduct.Title = product.Title.Trim();
            product.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteImage(int? id)
        {
            if (id == null) return BadRequest();

            ProductImage productImage = await _context.ProductImages
                .Include(p=>p.Product).ThenInclude(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productImage == null) return NotFound();

            if (productImage.Product.ProductImages.Count == 1)
            {
                return PartialView("_ProductImagePartial", productImage.Product.ProductImages.ToList());
            }

            string fullpath = Path.Combine(_env.WebRootPath, "assets","images", "product-quick",productImage.Image);

            if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }

            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();

            return PartialView("_ProductImagePartial", productImage.Product.ProductImages.ToList());
        }
    }
}
