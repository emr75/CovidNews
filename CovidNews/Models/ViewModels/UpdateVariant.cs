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
        //all countries that this variant is in
        public IEnumerable<CountryDto> variantcountries { get; set; }
        //countries which could have the varaint 
        public IEnumerable<CountryDto> allcountries { get; set; }
    }
}