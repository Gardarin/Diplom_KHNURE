namespace Diplom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Algorithms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InputDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Researchers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Name = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Researches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        State = c.String(),
                        CurentAlgorithm_Id = c.Int(),
                        CurentResult_Id = c.Int(),
                        Researcher_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Algorithms", t => t.CurentAlgorithm_Id)
                .ForeignKey("dbo.Results", t => t.CurentResult_Id)
                .ForeignKey("dbo.Researchers", t => t.Researcher_Id)
                .Index(t => t.CurentAlgorithm_Id)
                .Index(t => t.CurentResult_Id)
                .Index(t => t.Researcher_Id);
            
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Researches", "Researcher_Id", "dbo.Researchers");
            DropForeignKey("dbo.Researches", "CurentResult_Id", "dbo.Results");
            DropForeignKey("dbo.Researches", "CurentAlgorithm_Id", "dbo.Algorithms");
            DropIndex("dbo.Researches", new[] { "Researcher_Id" });
            DropIndex("dbo.Researches", new[] { "CurentResult_Id" });
            DropIndex("dbo.Researches", new[] { "CurentAlgorithm_Id" });
            DropTable("dbo.Results");
            DropTable("dbo.Researches");
            DropTable("dbo.Researchers");
            DropTable("dbo.InputDatas");
            DropTable("dbo.Algorithms");
        }
    }
}
