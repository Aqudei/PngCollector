using System.Data.Entity.Migrations;
using Database = PngCollector.Models.Database;

namespace PngCollector.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<Database>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Database context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
