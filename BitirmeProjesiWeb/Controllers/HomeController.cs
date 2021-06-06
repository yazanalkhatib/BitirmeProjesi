using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Home;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesiWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BitirmeContext _context;

        public HomeController(ILogger<HomeController> logger, BitirmeContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel()
            {
                Categories = await _context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync(),

                Certificates = await _context.Certificates.ToListAsync(),

                Products = await _context.Products
                           .Include(p => p.Photos)
                           .ToListAsync(),

                Partners = await _context.Partners.ToListAsync()
            };

            return View(indexViewModel);
        }

        public async Task<IActionResult> Posts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            PostsViewModel postsViewModel = new PostsViewModel()
            {
                Categories = await _context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync(),

                Products = await _context.Products
                           .Include(p => p.Photos)
                           .ToListAsync(),

                Partners = await _context.Partners.ToListAsync(),

                Posts = await _context.Posts.Where(p => p.SubCategoryId == id).Include(p => p.Photos).ToListAsync()
            };

            //if (posts.Count == 0)
            //{
            //    //İçerik yok diye yönlendir
            //    return NotFound();
            //}

            return View(postsViewModel);
        }

        public async Task<IActionResult> PostDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                // İçerik yok diye yönlendir
                //return NotFound();
            }

            post.Photos = await _context.Photos.Where(p => p.PostId == post.Id).ToListAsync();

            PostDetailsViewModel postDetailsViewModel = new PostDetailsViewModel()
            {
                Post = post,

                Categories = await _context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync(),

                Products = await _context.Products
                           .Include(p => p.Photos)
                           .ToListAsync(),

                Partners = await _context.Partners.ToListAsync()
            };

            return View(postDetailsViewModel);
        }

        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                // İçerik yok diye yönlendir
                //return NotFound();
            }

            product.Photos = await _context.ProductPhotos.Where(p => p.ProductId == product.Id).ToListAsync();

            ProductDetailsViewModel productDetailsViewModel = new ProductDetailsViewModel()
            {
                Product = product,

                Categories = await _context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync(),

                Products = await _context.Products
                           .Include(p => p.Photos)
                           .ToListAsync(),

                Partners = await _context.Partners.ToListAsync()
            };

            return View(productDetailsViewModel);
        }

        public async Task<IActionResult> Contact()
        {
            ContactViewModel contactViewModel = new ContactViewModel()
            {
                Categories = await _context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync(),

                Products = await _context.Products
                           .Include(p => p.Photos)
                           .ToListAsync(),

                Partners = await _context.Partners.ToListAsync(),

                Mail = new Utilities.Mail(),

                Users = await _context.Users.Where(u => u.DisplayForClients).Include(u => u.UserRole).ToListAsync()
            };

            return View(contactViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel contactViewModel)
        {
            if (ModelState.IsValid)
            {
                contactViewModel.Mail.Sent = await contactViewModel.Mail.Send();
            }

            contactViewModel.Users = await _context.Users.Where(u => u.DisplayForClients).Include(u => u.UserRole).ToListAsync();

            contactViewModel.Categories = await _context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync();

            contactViewModel.Products = await _context.Products
                           .Include(p => p.Photos)
                           .ToListAsync();

            contactViewModel.Partners = await _context.Partners.ToListAsync();

            return View(nameof(Contact), contactViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
