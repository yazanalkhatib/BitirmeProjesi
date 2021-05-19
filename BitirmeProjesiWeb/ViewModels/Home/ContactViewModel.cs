using BitirmeProjesiWeb.Models;
using BitirmeProjesiWeb.Utilities;
using System.Collections.Generic;

namespace BitirmeProjesiWeb.ViewModels.Home
{
    public class ContactViewModel : BaseViewModel
    {
        public Mail Mail { get; set; }
        public List<User> Users { get; set; }
    }
}