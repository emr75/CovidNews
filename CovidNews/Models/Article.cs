using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CovidNews.Models
{
    //This class describes an article entity.
    //It is used for generating a database table
    public class Article
    {
        [Key]
        public int ArticleID { get; set; }

        public string ArticleName { get; set; }

        public string Publisher { get; set; }

        //Foreign keys in Entity Framework
        /// https://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx

        //An article is associated with one country
        [ForeignKey("Country")]
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }
    }

    //This class can be used to transfer information about an article.
    //also known as a "Data Transfer Object"
    //https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
    //You can think of this class as the 'Model' that was used in 5101.
    //It is simply a vessel of communication
    public class ArticleDto
    {
        public int ArticleID { get; set; }
        public string ArticleName { get; set; }
        public string Publisher { get; set; }


    }
}