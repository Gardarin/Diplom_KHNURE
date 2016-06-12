namespace Diplom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InputData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Algorithms", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Researches", "InputData_Id", c => c.Int());
            CreateIndex("dbo.Researches", "InputData_Id");
            AddForeignKey("dbo.Researches", "InputData_Id", "dbo.InputDatas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Researches", "InputData_Id", "dbo.InputDatas");
            DropIndex("dbo.Researches", new[] { "InputData_Id" });
            DropColumn("dbo.Researches", "InputData_Id");
            DropColumn("dbo.Algorithms", "Discriminator");
        }
    }
}
