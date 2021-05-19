using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.Utilities;
using Microsoft.AspNetCore.Http;
using BitirmeProjesiWeb.ViewModels;
using Microsoft.AspNetCore.Authentication;

namespace BitirmeProjesiWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly ISecurity _security;
        private readonly IEncryptor _encryptor;

        public UsersController(BitirmeContext context, ISecurity security, IEncryptor encryptor)
        {
            _context = context;
            _security = security;
            _encryptor = encryptor;
        }

        // GET: Login
        public IActionResult Login()
        {
            if (!_security.Authenticate())
            {
                return View();
            }

            return RedirectToAction(nameof(Index), "Posts");
        }
        // POST: Login
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userdetails = await _context.Users.Include(u => u.UserRole)
                    .SingleOrDefaultAsync(m => m.Email == model.Email && m.Password == _encryptor.Encrypt(model.Password));

                if (userdetails == null)
                {
                    ModelState.AddModelError("Password", "Lütfen bilgilerin doğru olup olmadığını kontrol edin ve tekrar deneyin");
                    return View();
                }

                HttpContext.Session.SetString("userId", userdetails.Id.ToString());
                HttpContext.Session.SetString("userFullName", userdetails.FullName);
                HttpContext.Session.SetString("userRoleId", userdetails.UserRole.Id.ToString());

                return RedirectToAction(nameof(Index), "Posts");
            }

            return View();
        }


        public async Task<IActionResult> Index(string searchString)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["currentFilter"] = searchString;

            IQueryable<User> users = string.IsNullOrWhiteSpace(searchString) 
                ? _context.Users
                : _context.Users
                .Where(u => u.FullName.Contains(searchString)
                         || u.Email.Contains(searchString)
                         || u.UserRole.Name.Contains(searchString));

            foreach (var user in users)
            {
                if(user.Id != 1)
                {
                    user.Password = _encryptor.Decrypt(user.Password);
                }
            }

            users = users.Include(u => u.UserRole);

            return View(await users.ToListAsync());
        }


        // GET: Users/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["UserRoleId"] = new SelectList(_context.UserRoles, "Id", "NameTr");
            return View();
        }
        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = _encryptor.Encrypt(user.Password);
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    string errorMessage = string.Format("Email kullanmaktadır . başka bir Email  seçiniz");
                    ModelState.AddModelError("Email", errorMessage);
                    ViewData["UserRoleId"] = new SelectList(_context.UserRoles, "Id", "Name");
                    return View();
                }
            }

            ViewData["UserRoleId"] = new SelectList(_context.UserRoles, "Id", "NameTr", user.UserRoleId);
            return View(user);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_security.AuthorizeAdmin() || id == 1)
            {
                return View("AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = _encryptor.Decrypt(user.Password);
            ViewData["UserRoleId"] = new SelectList(_context.UserRoles, "Id", "NameTr", user.UserRoleId);
            return View(user);
        }
        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (!_security.AuthorizeAdmin() || id == 1)
            {
                return View("AccessDenied");
            }

            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = _encryptor.Encrypt(user.Password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
                    string errorMessage = string.Format("Email kullanmaktadır . başka bir Email  seçiniz");
                    ModelState.AddModelError("Email", errorMessage);
                    ViewData["UserRoleId"] = new SelectList(_context.UserRoles, "Id", "Name");
                    return View();
                }
            }
            ViewData["UserRoleId"] = new SelectList(_context.UserRoles, "Id", "NameTr", user.UserRoleId);
            return View(user);
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

            var user = await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Logout()
        {
            if (!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            if (HttpContext.Session.GetString("userId") != null)
            {
                HttpContext.Session.Clear();
            }

            return RedirectToAction(nameof(Login));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }


        // GET: Users/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _context.Users
        //        .Include(u => u.UserRole)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    user.Password = _encryptor.Decrypt(user.Password);
        //    return View(user);
        //}
    }
}