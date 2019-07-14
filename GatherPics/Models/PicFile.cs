using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherPics.Models
{
    class PicFile
    {
        public int PicFileId { get; set; }
        public string OriginalLocation { get; set; }
        public string CurrentLocation { get; set; }
    }
}
