using BitirmeProjesiWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitirmeProjesiWeb.ViewModels.Home
{
    public class BaseViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Partner> Partners { get; set; }
    }
}