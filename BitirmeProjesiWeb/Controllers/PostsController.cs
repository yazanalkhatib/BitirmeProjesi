using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Posts;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class PostsController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISecurity _security;

        public PostsController(BitirmeContext context, IWebHostEnvironment webHostEnvironment, ISecurity security)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _security = security;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string searchString)
        {
            if(!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            IQueryable<Post> posts = string.IsNullOrWhiteSpace(searchString)
                ? _context.Posts
                : _context.Posts.Where(c => c.DescriptionAr.Contains(searchString)
                                         || c.TitleAr.Contains(searchString)
                                         || c.SubCategory.NameAr.Contains(searchString));

            ViewData["currentFilter"] = searchString;

            return View(await posts.Include(p => p.SubCategory).ToListAsync());
        }

        

        // GET: Posts/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "NameTr");
            return View();
        }
        // POST: Posts/Create
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
                    ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "NameTr", model.SubCategoryId);
                    return View(model);
                }

                Post post = new Post
                {
                    SubCategoryId = model.SubCategoryId,
                    TitleAr = model.TitleAr,
                    TitleEn = model.TitleEn,
                    TitleTr = model.TitleTr,
                    DescriptionAr = model.DescriptionAr,
                    DescriptionEn = model.DescriptionEn,
                    DescriptionTr = model.DescriptionTr,
                    CoverPhotoPath = await ProcessUploadedFile(model.CoverPhoto),
                    Photos = new List<Photo>()
                };

                _context.Add(post);

                if(model.Photos != null)
                {
                    foreach (var photo in model.Photos)
                    {
                        post.Photos.Add(
                            new Photo
                            {
                                PostId = post.Id,
                                Path = await ProcessUploadedFile(photo)
                            }
                        );
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "NameTr", model.SubCategoryId);
            return View(model);
        }


        // GET: Posts/Edit/5
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

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            EditViewModel editViewModel = new EditViewModel
            {
                Id = post.Id,
                SubCategoryId = post.SubCategoryId,
                TitleAr = post.TitleAr,
                TitleEn = post.TitleEn,
                TitleTr = post.TitleTr,
                DescriptionAr = post.DescriptionAr,
                DescriptionEn = post.DescriptionEn,
                DescriptionTr = post.DescriptionTr,
                ExistingCoverPhotoPath = post.CoverPhotoPath,
                ExistingPhotos = await _context.Photos.Where(p => p.PostId == post.Id).ToListAsync()
            };

            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "NameTr", post.SubCategoryId);
            return View(editViewModel);
        }

        // POST: Posts/Edit/5
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
                    Post post = await _context.Posts.FindAsync(editViewModel.Id);
                    post.SubCategoryId = editViewModel.SubCategoryId;
                    post.TitleAr = editViewModel.TitleAr;
                    post.TitleEn = editViewModel.TitleEn;
                    post.TitleTr = editViewModel.TitleTr;
                    post.DescriptionAr = editViewModel.DescriptionAr;
                    post.DescriptionEn = editViewModel.DescriptionEn;
                    post.DescriptionTr = editViewModel.DescriptionTr;
                    post.Photos = await _context.Photos.Where(p => p.PostId == post.Id).ToListAsync();

                    if (editViewModel.CoverPhoto != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath,
                        "images", post.CoverPhotoPath);
                        System.IO.File.Delete(filePath);

                        post.CoverPhotoPath = await ProcessUploadedFile(editViewModel.CoverPhoto);
                    }

                    Photo existPhoto;
                    //Delete selected photos
                    if(editViewModel.ExistingPhotos != null)
                    {
                        for (int i = 0; i < editViewModel.ExistingPhotos.Count; i++)
                        {
                            existPhoto = editViewModel.ExistingPhotos[i];

                            if (existPhoto.AssignedToDelete)
                            {
                                var toDelete = post.Photos.Where(p => p.Path == existPhoto.Path).First();
                                post.Photos.Remove(toDelete);
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
                            post.Photos.Add(
                                new Photo
                                {
                                    PostId = post.Id,
                                    Path = await ProcessUploadedFile(photo)
                                }
                            ); ;
                        }
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(editViewModel.Id))
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
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "Id", "NameTr", editViewModel.SubCategoryId);
            return View(editViewModel);
        }

        // GET: Posts/Delete/5
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

            var post = await _context.Posts
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath,
                            "images", post.CoverPhotoPath));

            var photos = await _context.Photos.Where(p => p.PostId == post.Id).ToListAsync();

            foreach (var photo in photos)
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.WebRootPath,
                            "images", photo.Path));
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        private async Task<string> ProcessUploadedFile(IFormFile photo)
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }


        // GET: Posts/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var post = await _context.Posts
        //        .Include(p => p.SubCategory)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (post == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(post);
        //}
    }
}