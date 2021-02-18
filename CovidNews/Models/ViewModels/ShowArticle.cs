using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidNews.Models.ViewModels
{
    public class ShowArticle
    {

        public ArticleDto article { get; set; }
        //information about the country the article is about
        public CountryDto country { get; set; }
    }
}