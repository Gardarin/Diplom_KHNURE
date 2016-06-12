namespace Diplom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Result : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Results", "Name", c => c.String());
            AddColumn("dbo.Results", "Data", c => c.Binary());
            AddColumn("dbo.Results", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Results", "Discriminator");
            DropColumn("dbo.Results", "Data");
            DropColumn("dbo.Results", "Name");
        }
    }
}
