namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Jerrin : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TestimonialDepartmentsModels", newName: "DepartmentsModelTestimonials");
            DropPrimaryKey("dbo.DepartmentsModelTestimonials");
            AddColumn("dbo.Testimonials", "Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.Testimonials", "testimonial_Content", c => c.String(nullable: false));
            AlterColumn("dbo.Testimonials", "first_Name", c => c.String(nullable: false));
            AlterColumn("dbo.Testimonials", "last_Name", c => c.String(nullable: false));
            AlterColumn("dbo.Testimonials", "email", c => c.String(nullable: false));
            AddPrimaryKey("dbo.DepartmentsModelTestimonials", new[] { "DepartmentsModel_DepartmentId", "Testimonial_testimonial_Id" });
            CreateIndex("dbo.Testimonials", "Id");
            AddForeignKey("dbo.Testimonials", "Id", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Testimonials", "Id", "dbo.AspNetUsers");
            DropIndex("dbo.Testimonials", new[] { "Id" });
            DropPrimaryKey("dbo.DepartmentsModelTestimonials");
            AlterColumn("dbo.Testimonials", "email", c => c.String());
            AlterColumn("dbo.Testimonials", "last_Name", c => c.String());
            AlterColumn("dbo.Testimonials", "first_Name", c => c.String());
            AlterColumn("dbo.Testimonials", "testimonial_Content", c => c.String());
            DropColumn("dbo.Testimonials", "Id");
            AddPrimaryKey("dbo.DepartmentsModelTestimonials", new[] { "Testimonial_testimonial_Id", "DepartmentsModel_DepartmentId" });
            RenameTable(name: "dbo.DepartmentsModelTestimonials", newName: "TestimonialDepartmentsModels");
        }
    }
}
