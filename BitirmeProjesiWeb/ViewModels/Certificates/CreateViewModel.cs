using BitirmeProjesiWeb.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BitirmeProjesiWeb.ViewModels.Certificates
{
    public class CreateViewModel
    {
        
        [DisplayName("اسم الشهادة")]
        public string Name { get; set; }
        [DisplayName("صورة الشهادة")]
        public IFormFile Photo { get; set; }
    }
}