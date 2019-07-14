using System.Data.Entity;

namespace PngCollector.Models
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
