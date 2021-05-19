using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Partners;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class PartnersController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISecurity _security;

        public PartnersController(BitirmeContext context,
            IWebHostEnvironment webHostEnvironment,
            ISecurity security)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _security = security;
        }


        // GET: Partners
        public async Task<IActionResult> Index(string searchString)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["currentFilter"] = searchString;

            IQueryable<Partner> partners = string.IsNullOrWhiteSpace(searchString)
                ? _context.Partners
                : _context.Partners.Where(c => c.Name.Contains(searchString));

            return View(await partners.ToListAsync());
        }


        // GET: Partners/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            return View();
        }
        // POST: Partners/Create
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
                if (model.Logo == null)
                {
                    ModelState.AddModelError("Photo", "bir resim seçiniz");
                    return View();
                }

                Partner partner = new Partner
                {
                    Name = model.Name,
                    WebsiteLink = model.WebsiteLink,
                    LogoPath = await ProcessUploadedFile(model)
                };

                _context.Add(partner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // GET: Partners/Edit/5
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

            var partner = await _context.Partners.FindAsync(id);
            if (partner == null)
            {
                return NotFound();
            }

            EditViewModel editViewModel = new EditViewModel
            {
                Id = partner.Id,
                Name = partner.Name,
                WebsiteLink = partner.WebsiteLink,
                ExistingLogoPath = partner.LogoPath
            };

            return View(editViewModel);
        }
        // POST: Partners/Edit/5
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
                    Partner partner = _context.Partners.Find(editViewModel.Id);
                    partner.Name = editViewModel.Name;
                    partner.WebsiteLink = editViewModel.WebsiteLink;

                    // Delete existing photo and upload new photo
                    if (editViewModel.Logo != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        "images", partner.LogoPath);
                        System.IO.File.Delete(filePath);

                        partner.LogoPath = await ProcessUploadedFile(editViewModel);
                    }

                    _context.Update(partner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartnerExists(editViewModel.Id))
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

            editViewModel.ExistingLogoPath = _context.Partners.Find(editViewModel.Id).LogoPath;
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

            var partner = await _context.Partners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (partner == null)
            {
                return NotFound();
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
            "images", partner.LogoPath);
            System.IO.File.Delete(filePath);

            _context.Partners.Remove(partner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool PartnerExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        private async Task<string> ProcessUploadedFile(CreateViewModel model)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Logo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Logo.CopyToAsync(fileStream);
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