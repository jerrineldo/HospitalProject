namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrationFAQMarch282021 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Faqs", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Photos", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Faqs", new[] { "ApplicationUserId" });
            DropIndex("dbo.Photos", new[] { "ApplicationUserId" });
            AddColumn("dbo.Faqs", "DepartmentId", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "DepartmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.Faqs", "DepartmentId");
            CreateIndex("dbo.Photos", "DepartmentId");
            AddForeignKey("dbo.Faqs", "DepartmentId", "dbo.DepartmentsModels", "DepartmentId", cascadeDelete: true);
            AddForeignKey("dbo.Photos", "DepartmentId", "dbo.DepartmentsModels", "DepartmentId", cascadeDelete: true);
            DropColumn("dbo.Faqs", "ApplicationUserId");
            DropColumn("dbo.Photos", "ApplicationUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "ApplicationUserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Faqs", "ApplicationUserId", c => c.String(maxLength: 128));
            DropForeignKey("dbo.Photos", "DepartmentId", "dbo.DepartmentsModels");
            DropForeignKey("dbo.Faqs", "DepartmentId", "dbo.DepartmentsModels");
            DropIndex("dbo.Photos", new[] { "DepartmentId" });
            DropIndex("dbo.Faqs", new[] { "DepartmentId" });
            DropColumn("dbo.Photos", "DepartmentId");
            DropColumn("dbo.Faqs", "DepartmentId");
            CreateIndex("dbo.Photos", "ApplicationUserId");
            CreateIndex("dbo.Faqs", "ApplicationUserId");
            AddForeignKey("dbo.Photos", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Faqs", "ApplicationUserId", "dbo.AspNetUsers", "Id");
        }
    }
}
