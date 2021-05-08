namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_models_from_team_members_to_db : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Donors", "DonationId", "dbo.Donations");
            DropForeignKey("dbo.Donations", "Donor_DonorId", "dbo.Donors");
            DropIndex("dbo.Donations", new[] { "Donor_DonorId" });
            DropIndex("dbo.Donors", new[] { "DonationId" });
            RenameColumn(table: "dbo.Donations", name: "Donor_DonorId", newName: "DonorId");
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        contact_id = c.Int(nullable: false, identity: true),
                        contact_fname = c.String(),
                        contact_lname = c.String(),
                        contact_email = c.String(),
                        contact_phone = c.String(),
                        contact_message = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        DepartmentsModel_DepartmentId = c.Int(),
                    })
                .PrimaryKey(t => t.contact_id)
                .ForeignKey("dbo.DepartmentsModels", t => t.DepartmentsModel_DepartmentId)
                .ForeignKey("dbo.DepartmentsModels", t => t.DepartmentId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.DepartmentsModel_DepartmentId);
            
            CreateTable(
                "dbo.Testimonials",
                c => new
                    {
                        testimonial_Id = c.Int(nullable: false, identity: true),
                        testimonial_Content = c.String(),
                        first_Name = c.String(),
                        last_Name = c.String(),
                        email = c.String(),
                        phone_Number = c.Int(nullable: false),
                        Has_Pic = c.Boolean(nullable: false),
                        Pic_Extension = c.String(),
                        posted_Date = c.DateTime(nullable: false),
                        Approved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.testimonial_Id);
            
            CreateTable(
                "dbo.TestimonialDepartmentsModels",
                c => new
                    {
                        Testimonial_testimonial_Id = c.Int(nullable: false),
                        DepartmentsModel_DepartmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Testimonial_testimonial_Id, t.DepartmentsModel_DepartmentId })
                .ForeignKey("dbo.Testimonials", t => t.Testimonial_testimonial_Id, cascadeDelete: true)
                .ForeignKey("dbo.DepartmentsModels", t => t.DepartmentsModel_DepartmentId, cascadeDelete: true)
                .Index(t => t.Testimonial_testimonial_Id)
                .Index(t => t.DepartmentsModel_DepartmentId);
            
            AddColumn("dbo.DepartmentsModels", "Contact_contact_id", c => c.Int());
            AlterColumn("dbo.Donations", "DonorId", c => c.Int(nullable: false));
            CreateIndex("dbo.DepartmentsModels", "Contact_contact_id");
            CreateIndex("dbo.Donations", "DonorId");
            AddForeignKey("dbo.DepartmentsModels", "Contact_contact_id", "dbo.Contacts", "contact_id");
            AddForeignKey("dbo.Donations", "DonorId", "dbo.Donors", "DonorId", cascadeDelete: true);
            DropColumn("dbo.Donors", "DonationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Donors", "DonationId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Donations", "DonorId", "dbo.Donors");
            DropForeignKey("dbo.DepartmentsModels", "Contact_contact_id", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "DepartmentId", "dbo.DepartmentsModels");
            DropForeignKey("dbo.TestimonialDepartmentsModels", "DepartmentsModel_DepartmentId", "dbo.DepartmentsModels");
            DropForeignKey("dbo.TestimonialDepartmentsModels", "Testimonial_testimonial_Id", "dbo.Testimonials");
            DropForeignKey("dbo.Contacts", "DepartmentsModel_DepartmentId", "dbo.DepartmentsModels");
            DropIndex("dbo.TestimonialDepartmentsModels", new[] { "DepartmentsModel_DepartmentId" });
            DropIndex("dbo.TestimonialDepartmentsModels", new[] { "Testimonial_testimonial_Id" });
            DropIndex("dbo.Donations", new[] { "DonorId" });
            DropIndex("dbo.DepartmentsModels", new[] { "Contact_contact_id" });
            DropIndex("dbo.Contacts", new[] { "DepartmentsModel_DepartmentId" });
            DropIndex("dbo.Contacts", new[] { "DepartmentId" });
            AlterColumn("dbo.Donations", "DonorId", c => c.Int());
            DropColumn("dbo.DepartmentsModels", "Contact_contact_id");
            DropTable("dbo.TestimonialDepartmentsModels");
            DropTable("dbo.Testimonials");
            DropTable("dbo.Contacts");
            RenameColumn(table: "dbo.Donations", name: "DonorId", newName: "Donor_DonorId");
            CreateIndex("dbo.Donors", "DonationId");
            CreateIndex("dbo.Donations", "Donor_DonorId");
            AddForeignKey("dbo.Donations", "Donor_DonorId", "dbo.Donors", "DonorId");
            AddForeignKey("dbo.Donors", "DonationId", "dbo.Donations", "DonationId", cascadeDelete: true);
        }
    }
}
