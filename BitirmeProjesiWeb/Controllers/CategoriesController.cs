using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Categories;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISecurity _security;

        public CategoriesController(BitirmeContext context,
            IWebHostEnvironment webHostEnvironment,
            ISecurity security)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _security = security;
        }


        // GET: Categories
        public async Task<IActionResult> Index(string searchString)
        {
            if(!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            ViewData["currentFilter"] = searchString;

            IQueryable<Category> categories = string.IsNullOrWhiteSpace(searchString) 
                ? _context.Categories 
                : _context.Categories.Where(c => c.NameAr.Contains(searchString));

            return View(await categories.ToListAsync());
        }


        // GET: Categories/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            return View();
        }
        // POST: Categories/Create
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
                if (model.Photo == null)
                {
                    ModelState.AddModelError("Photo", "Resim seçiniz");
                    return View();
                }

                Category category = new Category
                {
                    NameAr = model.NameAr,
                    NameEn = model.NameEn,
                    NameTr = model.NameTr,
                    CoverPhotoPath = await ProcessUploadedFile(model)
                };

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: Categories/Edit/5
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

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            EditViewModel editViewModel = new EditViewModel
            {
                Id = category.Id,
                NameAr = category.NameAr,
                NameEn = category.NameEn,
                NameTr = category.NameTr,
                ExistingPhotoPath = category.CoverPhotoPath
            };

            return View(editViewModel);
        }
        // POST: Categories/Edit/5
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
                    Category category = _context.Categories.Find(editViewModel.Id);
                    category.NameAr = editViewModel.NameAr;
                    category.NameEn = editViewModel.NameEn;
                    category.NameTr = editViewModel.NameTr;

                    // Delete existing photo and upload new photo
                    if (editViewModel.Photo != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        "images", category.CoverPhotoPath);
                        System.IO.File.Delete(filePath);

                        category.CoverPhotoPath = await ProcessUploadedFile(editViewModel);
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(editViewModel.Id))
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

            editViewModel.ExistingPhotoPath = _context.Categories.Find(editViewModel.Id).CoverPhotoPath;
            return View(editViewModel);
        }


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

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
            "images", category.CoverPhotoPath);
            System.IO.File.Delete(filePath);

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private async Task<string> ProcessUploadedFile(CreateViewModel model)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Photo.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }


        // GET: Categories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}
    }
}