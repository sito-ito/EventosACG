namespace EventosACG.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persona", "Sexo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persona", "Sexo");
        }
    }
}
