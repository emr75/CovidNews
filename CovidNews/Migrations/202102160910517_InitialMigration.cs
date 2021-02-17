namespace CovidNews.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IniialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        ArticleID = c.Int(nullable: false, identity: true),
                        ArticleName = c.String(),
                        Publisher = c.String(),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleID)
                .ForeignKey("dbo.Countries", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryName = c.String(),
                        Population = c.Int(nullable: false),
                        Infected = c.Int(nullable: false),
                        Vaccinated = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CountryID);
            
            CreateTable(
                "dbo.Variants",
                c => new
                    {
                        VariantID = c.Int(nullable: false, identity: true),
                        VariantName = c.String(),
                    })
                .PrimaryKey(t => t.VariantID);
            
            CreateTable(
                "dbo.VariantCountries",
                c => new
                    {
                        Variant_VariantID = c.Int(nullable: false),
                        Country_CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Variant_VariantID, t.Country_CountryID })
                .ForeignKey("dbo.Variants", t => t.Variant_VariantID, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_CountryID, cascadeDelete: true)
                .Index(t => t.Variant_VariantID)
                .Index(t => t.Country_CountryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Articles", "CountryID", "dbo.Countries");
            DropForeignKey("dbo.VariantCountries", "Country_CountryID", "dbo.Countries");
            DropForeignKey("dbo.VariantCountries", "Variant_VariantID", "dbo.Variants");
            DropIndex("dbo.VariantCountries", new[] { "Country_CountryID" });
            DropIndex("dbo.VariantCountries", new[] { "Variant_VariantID" });
            DropIndex("dbo.Articles", new[] { "CountryID" });
            DropTable("dbo.VariantCountries");
            DropTable("dbo.Variants");
            DropTable("dbo.Countries");
            DropTable("dbo.Articles");
        }
    }
}
