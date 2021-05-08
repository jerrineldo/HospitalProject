namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class blog_feedback_migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BlogModels",
                c => new
                    {
                        blog_id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        photo_path = c.String(),
                        title = c.String(),
                        text = c.String(),
                        email = c.String(),
                    })
                .PrimaryKey(t => t.blog_id);
            
            CreateTable(
                "dbo.FeedbackModels",
                c => new
                    {
                        feedback_id = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        fname = c.String(),
                        lname = c.String(),
                        email = c.String(),
                        title = c.String(),
                        text = c.String(),
                        DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.feedback_id)
                .ForeignKey("dbo.DepartmentsModels", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FeedbackModels", "DepartmentId", "dbo.DepartmentsModels");
            DropIndex("dbo.FeedbackModels", new[] { "DepartmentId" });
            DropTable("dbo.FeedbackModels");
            DropTable("dbo.BlogModels");
        }
    }
}
