using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Certificates;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class CertificatesController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISecurity _security;

        public CertificatesController(BitirmeContext context,
            IWebHostEnvironment webHostEnvironment,
            ISecurity security)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _security = security;
        }


        // GET: Certificates
        public async Task<IActionResult> Index(string searchString)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["currentFilter"] = searchString;

            IQueryable<Certificate> certificates = string.IsNullOrWhiteSpace(searchString)
                ? _context.Certificates
                : _context.Certificates.Where(c => c.Name.Contains(searchString));

            return View(await certificates.ToListAsync());
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
        // POST: Certificates/Create
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

                Certificate certificate = new Certificate
                {
                    Name = model.Name,
                    PhotoPath = await ProcessUploadedFile(model)
                };

                _context.Add(certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            EditViewModel editViewModel = new EditViewModel
            {
                Id = certificate.Id,
                Name = certificate.Name,
                ExistingPhotoPath = certificate.PhotoPath
            };

            return View(editViewModel);
        }
        // POST: Certificates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel editViewModel)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Certificate certificate = _context.Certificates.Find(editViewModel.Id);
                    certificate.Name = editViewModel.Name;

                    // Delete existing photo and upload new photo
                    if (editViewModel.Photo != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        "images", certificate.PhotoPath);
                        System.IO.File.Delete(filePath);

                        certificate.PhotoPath = await ProcessUploadedFile(editViewModel);
                    }

                    _context.Update(certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateExists(editViewModel.Id))
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

            editViewModel.ExistingPhotoPath = _context.Certificates.Find(editViewModel.Id).PhotoPath;
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

            var certificate = await _context.Certificates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null)
            {
                return NotFound();
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
            "images", certificate.PhotoPath);
            System.IO.File.Delete(filePath);

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool CertificateExists(int id)
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