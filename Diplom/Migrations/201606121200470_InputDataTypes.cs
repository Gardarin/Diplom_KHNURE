namespace Diplom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InputDataTypes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InputDatas", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InputDatas", "Discriminator");
        }
    }
}
