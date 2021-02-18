using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidNews.Models.ViewModels
{
    //The View Model required to update a article
    public class UpdateArticle
    {
        //Information about the article
        public ArticleDto article { get; set; }
        //Needed for a dropdownlist which presents the article with a choice of countries to play for
        public IEnumerable<CountryDto> allcountries { get; set; }
    }
}