namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContactUsFeatureKunalSailor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "Firstname", c => c.String());
            AddColumn("dbo.Contacts", "Lastname", c => c.String());
            AddColumn("dbo.Contacts", "Email", c => c.String());
            AddColumn("dbo.Contacts", "Phone", c => c.String());
            AddColumn("dbo.Contacts", "Message", c => c.String());
            DropColumn("dbo.Contacts", "contact_fname");
            DropColumn("dbo.Contacts", "contact_lname");
            DropColumn("dbo.Contacts", "contact_email");
            DropColumn("dbo.Contacts", "contact_phone");
            DropColumn("dbo.Contacts", "contact_message");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Contacts", "contact_message", c => c.String());
            AddColumn("dbo.Contacts", "contact_phone", c => c.String());
            AddColumn("dbo.Contacts", "contact_email", c => c.String());
            AddColumn("dbo.Contacts", "contact_lname", c => c.String());
            AddColumn("dbo.Contacts", "contact_fname", c => c.String());
            DropColumn("dbo.Contacts", "Message");
            DropColumn("dbo.Contacts", "Phone");
            DropColumn("dbo.Contacts", "Email");
            DropColumn("dbo.Contacts", "Lastname");
            DropColumn("dbo.Contacts", "Firstname");
        }
    }
}
