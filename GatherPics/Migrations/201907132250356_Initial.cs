namespace GatherPics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        FolderId = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false, maxLength: 1024),
                    })
                .PrimaryKey(t => t.FolderId)
                .Index(t => t.Path, unique: true);
            
            CreateTable(
                "dbo.PicFiles",
                c => new
                    {
                        PicFileId = c.Int(nullable: false, identity: true),
                        OriginalLocation = c.String(maxLength: 4000),
                        CurrentLocation = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.PicFileId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Folders", new[] { "Path" });
            DropTable("dbo.PicFiles");
            DropTable("dbo.Folders");
        }
    }
}
