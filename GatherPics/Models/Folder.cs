using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherPics.Models
{
    class Folder
    {
        [StringLength(1024)]
        [Index(IsUnique = true)]
        public string Path { get; set; }

        public int FolderId { get; set; }
    }
}
