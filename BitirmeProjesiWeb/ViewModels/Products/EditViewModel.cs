using BitirmeProjesiWeb.Models;
using System.Collections.Generic;

namespace BitirmeProjesiWeb.ViewModels.Products
{
    public class EditViewModel : CreateViewModel
    {
        public int Id { get; set; }
        public string ExistingCoverPhotoPath { get; set; }
        public List<ProductPhoto> ExistingPhotos { get; set; }
    }
}