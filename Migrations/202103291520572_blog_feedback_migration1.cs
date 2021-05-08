namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blog_feedback_migration1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BlogModels", "ApplicationUserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.BlogModels", "ApplicationUserId");
            AddForeignKey("dbo.BlogModels", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BlogModels", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.BlogModels", new[] { "ApplicationUserId" });
            DropColumn("dbo.BlogModels", "ApplicationUserId");
        }
    }
}
