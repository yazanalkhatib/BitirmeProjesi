using BitirmeProjesiWeb.Models;
using System.Collections.Generic;

namespace BitirmeProjesiWeb.ViewModels.Home
{
    public class IndexViewModel : BaseViewModel
    {
        public List<Certificate> Certificates { get; set; }
    }
}