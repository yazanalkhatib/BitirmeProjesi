using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISecurity _security;

        public UserRolesController(BitirmeContext context,
            IWebHostEnvironment webHostEnvironment,
            ISecurity security)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _security = security;
        }


        // GET: UserRoles
        public async Task<IActionResult> Index(string searchString)
        {
            if (!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            ViewData["currentFilter"] = searchString;

            IQueryable<UserRole> userRoles = string.IsNullOrWhiteSpace(searchString)
                ? _context.UserRoles
                : _context.UserRoles.Where(c => c.NameAr.Contains(searchString));

            return View(await userRoles.ToListAsync());
        }


        // GET: UserRoles/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            return View();
        }
        // POST: UserRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRole userRole)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(userRole);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    string errorMessage = string.Format("Ad kullanmaktadır . başka bir adı seçiniz");
                    ModelState.AddModelError("NameAr", errorMessage);
                    return View();
                }
            }

            return View(userRole);
        }


        // GET: UserRoles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_security.Authenticate() || id == 1)
            {
                return View("AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles.FindAsync(id);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(userRole);
        }
        // POST: UserRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserRole userRole)
        {
            if (!_security.Authenticate() || userRole.Id == 1)
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserRoleExists(userRole.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException)
                {
                    string errorMessage = string.Format("Ad kullanmaktadır . başka bir adı seçiniz");
                    ModelState.AddModelError("NameAr", errorMessage);
                    return View();
                }
                return RedirectToAction(nameof(Index));
            }

            return View(userRole);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (!_security.AuthorizeAdmin() || id == 1)
            {
                return View("AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userRole == null)
            {
                return NotFound();
            }

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool UserRoleExists(int id)
        {
            return _context.UserRoles.Any(e => e.Id == id);
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