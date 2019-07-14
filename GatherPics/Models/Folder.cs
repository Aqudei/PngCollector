using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PngCollector.Models
{
    class Folder
    {
        [StringLength(1024)]
        [Index(IsUnique = true)]
        public string Path { get; set; }

        public int FolderId { get; set; }
    }
}
