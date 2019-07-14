using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherPics.Models
{
    class Database : DbContext
    {
        public DbSet<PicFile> PicFiles { get; set; }
        public DbSet<Folder> Folders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Folder>()
                .Property(folder => folder.Path)
                .IsRequired();
        }
    }
}
