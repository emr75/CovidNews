using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CovidNews.Models
{
    //This class describes a variant entity.
    //It is used for actually generating a database table
    public class Variant
    {
        [Key]
        public int VariantID { get; set; }

        public string VariantName { get; set; }


        //Utilizes the inverse property to specify the "Many"
        //https://www.entityframeworktutorial.net/code-first/inverseproperty-dataannotations-attribute-in-code-first.aspx
        //One Variant Many Countries
        public ICollection<Country> Counties { get; set; }

        //This class can be used to transfer information about an article.
        //also known as a "Data Transfer Object"
        //https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
        //You can think of this class as the 'Model' that was used in 5101.
        //It is simply a vessel of communication
        public class VariantDto
        {
            public int VariantID { get; set; }
            public string VariantName { get; set; }

        }
    }
}