namespace Diplom.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InputData2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InputDatas", "Input", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InputDatas", "Input");
        }
    }
}
