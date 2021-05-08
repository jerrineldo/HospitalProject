namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class donation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Donations",
                c => new
                    {
                        DonationId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OneTime = c.Boolean(nullable: false),
                        Msg = c.String(),
                        Dedication = c.String(),
                        DedicateName = c.String(),
                        Action = c.String(),
                        Anonymity = c.Boolean(nullable: false),
                        PaymentMethod = c.String(nullable: false),
                        PaymentNumber = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.DonationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Donations");
        }
    }
}
