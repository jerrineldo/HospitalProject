namespace Red_Lake_Hospital_Redesign_Team6.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EcardsFeatureKunalSailor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ecards",
                c => new
                    {
                        ecard_id = c.Int(nullable: false, identity: true),
                        photo_path = c.String(),
                        message = c.String(),
                        DepartmentId = c.Int(nullable: false),
                        DepartmentsModel_DepartmentId = c.Int(),
                    })
                .PrimaryKey(t => t.ecard_id)
                .ForeignKey("dbo.DepartmentsModels", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.DepartmentsModels", t => t.DepartmentsModel_DepartmentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.DepartmentsModel_DepartmentId);
            
            AddColumn("dbo.DepartmentsModels", "Ecards_ecard_id", c => c.Int());
            CreateIndex("dbo.DepartmentsModels", "Ecards_ecard_id");
            AddForeignKey("dbo.DepartmentsModels", "Ecards_ecard_id", "dbo.Ecards", "ecard_id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ecards", "DepartmentsModel_DepartmentId", "dbo.DepartmentsModels");
            DropForeignKey("dbo.DepartmentsModels", "Ecards_ecard_id", "dbo.Ecards");
            DropForeignKey("dbo.Ecards", "DepartmentId", "dbo.DepartmentsModels");
            DropIndex("dbo.Ecards", new[] { "DepartmentsModel_DepartmentId" });
            DropIndex("dbo.Ecards", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentsModels", new[] { "Ecards_ecard_id" });
            DropColumn("dbo.DepartmentsModels", "Ecards_ecard_id");
            DropTable("dbo.Ecards");
        }
    }
}
