using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitirmeProjesiWeb.Models
{
    public class Photo
    {
        public int Id { get; set; }

        //Foreign key for Post
        public int PostId { get; set; }
        public Post Post { get; set; }

        public string Path { get; set; }
        public bool AssignedToDelete { get; set; }
    }
}