using BitirmeProjesiWeb.CustomAttributes;
using System.ComponentModel;

namespace BitirmeProjesiWeb.Models
{
    public class Certificate
    {
        [CustomRequired]
        public int Id { get; set; }
        [DisplayName("Sertifika Adı")]
        public string Name { get; set; }
        [DisplayName("Sertifika resmi")]
        public string PhotoPath { get; set; }
    }
}