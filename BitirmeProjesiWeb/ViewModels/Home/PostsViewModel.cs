using BitirmeProjesiWeb.Models;
using System.Collections.Generic;

namespace BitirmeProjesiWeb.ViewModels.Home
{
    public class PostsViewModel : BaseViewModel
    {
        public List<Post> Posts { get; set; }
    }
}