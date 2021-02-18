using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CovidNews.Models.ViewModels
{
    public class UpdateVariant
    {
        //base information about the variant
        public VariantDto variant { get; set; }
        //display all countries that this variant is in
        public IEnumerable<CountryDto> variantincountries { get; set; }
        //display countries which could have the varaint in a dropdownlist
        public IEnumerable<CountryDto> allcountries { get; set; }
    }
}