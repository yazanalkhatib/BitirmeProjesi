using BitirmeProjesiWeb.CustomAttributes;

namespace BitirmeProjesiWeb.Models
{
    public class ProductPhoto
    {
        public int Id { get; set; }

        //Foreign key for Product
        [CustomRequired]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string Path { get; set; }
        public bool AssignedToDelete { get; set; }
    }
}