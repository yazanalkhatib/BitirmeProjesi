using BitirmeProjesiWeb.Models;
using System.Collections.Generic;

namespace BitirmeProjesiWeb.ViewModels.Posts
{
    public class EditViewModel : CreateViewModel
    {
        public int Id { get; set; }
        public string ExistingCoverPhotoPath { get; set; }
        public List<Photo> ExistingPhotos { get; set; }
    }
}