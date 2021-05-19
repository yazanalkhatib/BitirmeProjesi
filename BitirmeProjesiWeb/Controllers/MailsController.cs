using System.Linq;
using System.Threading.Tasks;
using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesiWeb.Controllers
{
    public class MailsController : Controller
    {
        private readonly BitirmeContext context;

        public MailsController(BitirmeContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Send(ContactViewModel contactViewModel)
        {
            if (ModelState.IsValid)
            {
                contactViewModel.Mail.Sent = await contactViewModel.Mail.Send();
            }

            contactViewModel.Users = await context.Users.Where(u => u.DisplayForClients).Include(u => u.UserRole).ToListAsync();
            
            contactViewModel.Categories = await context.Categories
                            .Include(c => c.SubCategories)
                            .ToListAsync();

            contactViewModel.Products = await context.Products
                           .Include(p => p.Photos)
                           .ToListAsync();

            contactViewModel.Partners = await context.Partners.ToListAsync();
            
            return View("~/Views/Home/Contact.cshtml", contactViewModel);
        }
    }
}