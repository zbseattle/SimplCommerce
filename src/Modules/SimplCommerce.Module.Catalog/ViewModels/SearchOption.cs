using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SimplCommerce.Module.Catalog.ViewModels
{
    public class SearchOption 
    {
        public string Query { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string Sort { get; set; }

        public int? MinPrice { get; set; }

        public int? MaxPrice { get; set; }

        /*
        From "https://stackoverflow.com/questions/12633471/mvc4-datatype-date-editorfor-wont-display-date-value-in-chrome-fine-in-interne":
        When you decorate a model property with [DataType(DataType.Date)] the default template in ASP.NET MVC 4 generates an input field
        of type="date". Browsers that support HTML5 such Google Chrome render this input field with a date picker.

        In order to correctly display the date, the value must be formatted as 2012-09-28. Quote from the specification:

            value: A valid full-date as defined in [RFC 3339], with the additional qualification that the year component is four
            or more digits representing a number greater than 0.

        You could enforce this format using the DisplayFormat attribute:
        */
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CompareDates("EndDate", ErrorMessage = "Start date must be before end date")]
        public DateTime StartDate {get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [CompareDates("StartDate", ErrorMessage = "Start date must be before end date")]
        public DateTime EndDate {get; set; }

        public Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(Query))
            {
                dict.Add("query", Query);
            }

            if (!string.IsNullOrWhiteSpace(Brand))
            {
                dict.Add("brand", Brand);
            }

            if (!string.IsNullOrWhiteSpace(Category))
            {
                dict.Add("category", Category);
            }

            if (MinPrice.HasValue)
            {
                dict.Add("minPrice", MinPrice.Value.ToString());
            }

            if (MaxPrice.HasValue)
            {
                dict.Add("maxPrice", MaxPrice.Value.ToString());
            }

            if (!string.IsNullOrWhiteSpace(Sort))
            {
                dict.Add("sort", Sort);
            }

            DateTime temp;
            if (DateTime.TryParse(StartDate.ToString(), out temp))
            {
                dict.Add("startDate", StartDate.Date.ToString());
            }

            if (DateTime.TryParse(EndDate.ToString(), out temp))
            {
                dict.Add("endDate", EndDate.Date.ToString());
            }

            return dict;
        }

        public IList<string> GetBrands()
        {
            return string.IsNullOrWhiteSpace(Brand) ? new List<string>() : Brand.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public IList<string> GetCategories()
        {
            return string.IsNullOrWhiteSpace(Category) ? new List<string>() : Category.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public string ToJson()
        {
            var jsonSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            return JsonConvert.SerializeObject(this, jsonSetting);
        }

    }

    public class CompareDatesAttribute : ValidationAttribute, IClientModelValidator
    {
        private string _otherPropertyName;

        public CompareDatesAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = ValidationResult.Success;
            if (value != null)
            {
                if (!_otherPropertyName.ToLower().Contains("start"))
                {
                    if ((DateTime)value > ((SearchOption)validationContext.ObjectInstance).EndDate)
                    {
                        result = new ValidationResult(ErrorMessage);
                    }
                }
                else
                {
                    if ((DateTime)value < ((SearchOption)validationContext.ObjectInstance).StartDate)
                    {
                        result = new ValidationResult(ErrorMessage);
                    }
                }
            }
            return result;
        }        

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-comparedates", "Start date must be equal or later than end date");
            context.Attributes.Add("data-val-comparedates-otherdate", _otherPropertyName);
       }
    }
    
}
