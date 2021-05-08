namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faqphotopart2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Faqs",
                c => new
                    {
                        faq_id = c.Int(nullable: false, identity: true),
                        question = c.String(),
                        answer = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.faq_id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        photo_id = c.Int(nullable: false, identity: true),
                        photo_path = c.String(nullable: false),
                        description = c.String(),
                        photographer_fname = c.String(),
                        photographer_lname = c.String(),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.photo_id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Faqs", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Photos", new[] { "ApplicationUserId" });
            DropIndex("dbo.Faqs", new[] { "ApplicationUserId" });
            DropTable("dbo.Photos");
            DropTable("dbo.Faqs");
        }
    }
}
