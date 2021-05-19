using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.Utilities;

namespace BitirmeProjesiWeb.Controllers
{
    public class SubCategoriesController : Controller
    {
        private readonly BitirmeContext _context;
        private readonly ISecurity _security;

        public SubCategoriesController(BitirmeContext context, ISecurity security)
        {
            _context = context;
            _security = security;
        }

        // GET: SubCategories
        public async Task<IActionResult> Index(string searchString)
        {
            if(!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            ViewData["currentFilter"] = searchString;

            IQueryable<SubCategory> subCategories = string.IsNullOrWhiteSpace(searchString)
                ? _context.SubCategories
                : _context.SubCategories.Where(c => c.NameAr.Contains(searchString) 
                                                 || c.Category.NameAr.Contains(searchString));

            return View(await subCategories.Include(s => s.Category).ToListAsync());
        }

 

        // GET: SubCategories/Create
        public IActionResult Create()
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr");
            return View();
        }
        // POST: SubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategory subCategory)
        {
            if (!_security.AuthorizeAdmin())
            {
                return View("AccessDenied");
            }

            if (ModelState.IsValid)
            {
                _context.Add(subCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", subCategory.CategoryId);
            return View(subCategory);
        }


        // GET: SubCategories/Edit/5
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

            var subCategory = await _context.SubCategories.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", subCategory.CategoryId);
            return View(subCategory);
        }
        // POST: SubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategory subCategory)
        {
            if (!_security.Authenticate())
            {
                return View("AccessDenied");
            }

            if (id != subCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubCategoryExists(subCategory.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "NameTr", subCategory.CategoryId);
            return View(subCategory);
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

            var subCategory = await _context.SubCategories
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubCategoryExists(int id)
        {
            return _context.SubCategories.Any(e => e.Id == id);
        }



        // GET: SubCategories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var subCategory = await _context.SubCategories
        //        .Include(s => s.Category)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (subCategory == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(subCategory);
        //}
    }
}