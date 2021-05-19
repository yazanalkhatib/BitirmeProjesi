using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Products;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISecurity _security;

        public ProductsController(BitirmeContext context, IWebHostEnvironment webHostEnvironment, ISecurity security)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _security = security;
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            if(!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            IQueryable<Product> products = string.IsNullOrWhiteSpace(searchString)
               ? _context.Products
               : _context.Products.Where(c => c.Name.Contains(searchString)
                                           || c.Category.NameAr.Contains(searchString)
                                           || c.DescriptionAr.Contains(searchString));

            ViewData["currentFilter"] = searchString;

            return View(await products.Include(p => p.Category).ToListAsync());
        }


        // GET: Products/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr");
            return View();
        }
        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                if (model.CoverPhoto == null)
                {
                    ModelState.AddModelError("CoverPhoto", "Resim seçiniz");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", model.CategoryId);
                    return View(model);
                }

                Product product = new Product
                {
                    CategoryId = model.CategoryId,
                    Name = model.Name,
                    DescriptionAr = model.DescriptionAr,
                    DescriptionEn = model.DescriptionEn,
                    DescriptionTr = model.DescriptionTr,
                    CoverPhotoPath = await ProcessUploadedFile(model.CoverPhoto),
                    Photos = new List<ProductPhoto>()
                };

                _context.Add(product);

                if (model.Photos != null)
                {
                    foreach (var photo in model.Photos)
                    {
                        product.Photos.Add(
                            new ProductPhoto
                            {
                                ProductId = product.Id,
                                Path = await ProcessUploadedFile(photo)
                            }
                        );
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", model.CategoryId);
            return View(model);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            EditViewModel editViewModel = new EditViewModel
            {
                Id = product.Id,
                CategoryId = product.CategoryId,
                Name = product.Name,
                DescriptionAr = product.DescriptionAr,
                DescriptionEn = product.DescriptionEn,
                DescriptionTr = product.DescriptionTr,
                ExistingCoverPhotoPath = product.CoverPhotoPath,
                ExistingPhotos = await _context.ProductPhotos.Where(p => p.ProductId == product.Id).ToListAsync()
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", product.CategoryId);
            return View(editViewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            if (!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Product product = await _context.Products.FindAsync(editViewModel.Id);
                    product.CategoryId = editViewModel.CategoryId;
                    product.Name = editViewModel.Name;
                    product.DescriptionAr = editViewModel.DescriptionAr;
                    product.DescriptionEn = editViewModel.DescriptionEn;
                    product.DescriptionTr = editViewModel.DescriptionTr;
                    product.Photos = await _context.ProductPhotos.Where(p => p.ProductId == product.Id).ToListAsync();

                    if(editViewModel.CoverPhoto != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        "images", product.CoverPhotoPath);
                        System.IO.File.Delete(filePath);

                        product.CoverPhotoPath = await ProcessUploadedFile(editViewModel.CoverPhoto);
                    }

                    ProductPhoto existPhoto;
                    //Delete selected photos
                    if (editViewModel.ExistingPhotos != null)
                    {
                        for (int i = 0; i < editViewModel.ExistingPhotos.Count; i++)
                        {
                            existPhoto = editViewModel.ExistingPhotos[i];

                            if (existPhoto.AssignedToDelete)
                            {
                                var toDelete = product.Photos.Where(p => p.Path == existPhoto.Path).First();
                                product.Photos.Remove(toDelete);
                                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath,
                                "images", existPhoto.Path));
                            }
                        }
                    }

                    // Add new photos if exists
                    if (editViewModel.Photos != null)
                    {
                        foreach (var photo in editViewModel.Photos)
                        {
                            product.Photos.Add(
                                new ProductPhoto
                                {
                                    ProductId = product.Id,
                                    Path = await ProcessUploadedFile(photo)
                                }
                            ); ;
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(editViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", editViewModel.CategoryId);
            return View(editViewModel);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath,
                            "images", product.CoverPhotoPath));

            var photos = await _context.ProductPhotos.Where(p => p.ProductId == product.Id).ToListAsync();

            foreach (var photo in photos)
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath,
                            "images", photo.Path));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        private async Task<string> ProcessUploadedFile(IFormFile formFile)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }


        // GET: Products/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Category)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}
    }
}