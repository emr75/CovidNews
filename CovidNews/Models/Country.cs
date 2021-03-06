﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CovidNews.Models
{
    //This class describes a country entity.
    //It is used for generating a database table
    public class Country
    {
        public int CountryID { get; set; }

        public string CountryName { get; set; }

        public int Population { get; set; }

        public int Infected { get; set; }

        public int Vaccinated { get; set; }

        //A country can have many articles about it
        public ICollection<Article> Articles { get; set; }

        //A country can have many Variants
        public ICollection<Variant> Variants { get; set; }
    }

    public class CountryDto
    {
        public int CountryID { get; set; }

        [DisplayName("Country Name")]
        public string CountryName { get; set; }
        public int Population { get; set; }
        public int Infected { get; set; }
        public int Vaccinated { get; set; }
        public ICollection<Variant> Variants { get; set; }
    }
}