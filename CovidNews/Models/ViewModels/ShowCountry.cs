using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidNews.Models.ViewModels
{
    public class ShowCountry
    {
        //Information about the country
        public CountryDto country { get; set; }

        //Information about all articles about that country
        public IEnumerable<ArticleDto> countryarticles { get; set; }

        //Information about all variants for that country
        public IEnumerable<VariantDto> countriesvariants { get; set; }
    }
}