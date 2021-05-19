using BitirmeProjesiWeb.CustomAttributes;
using System.ComponentModel;

namespace BitirmeProjesiWeb.Models
{
    public class Partner
    {
        [CustomRequired]
        public int Id { get; set; }
        [DisplayName("firmanın adı")]
        public string Name { get; set; }
        [DisplayName("firmanın web sitesi")]
        public string WebsiteLink { get; set; }
        [DisplayName("Logo resmi")]
        public string LogoPath { get; set; }
    }
}