namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class donation_2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Donors",
                c => new
                    {
                        DonorId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        OrgName = c.String(nullable: false),
                        Fname = c.String(nullable: false),
                        Lname = c.String(nullable: false),
                        Addressl1 = c.String(nullable: false),
                        Addressl2 = c.String(nullable: false),
                        City = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        Province = c.String(),
                        PostalCode = c.String(nullable: false),
                        DonationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DonorId)
                .ForeignKey("dbo.Donations", t => t.DonationId, cascadeDelete: true)
                .Index(t => t.DonationId);
            
            AddColumn("dbo.Donations", "Donor_DonorId", c => c.Int());
            CreateIndex("dbo.Donations", "Donor_DonorId");
            AddForeignKey("dbo.Donations", "Donor_DonorId", "dbo.Donors", "DonorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Donations", "Donor_DonorId", "dbo.Donors");
            DropForeignKey("dbo.Donors", "DonationId", "dbo.Donations");
            DropIndex("dbo.Donors", new[] { "DonationId" });
            DropIndex("dbo.Donations", new[] { "Donor_DonorId" });
            DropColumn("dbo.Donations", "Donor_DonorId");
            DropTable("dbo.Donors");
        }
    }
}
