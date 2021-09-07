using app.Models.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace app.TagHelpers
{
    [HtmlTargetElement("search")]
    public class SearchTagHelper : TagHelper
    {
        IConfiguration configuration;
        private IHtmlHelper _htmlHelper;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public SearchTagHelper(IConfiguration configuration, IHtmlHelper htmlHelper)
        {
            this.configuration = configuration;
            _htmlHelper = htmlHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("default")))
            {
                var sql = @"select category_name CategoryName,image Image,display Display from service_category where status=1;
                            
                            ";
                var data = connection.Query<ServiceCategoryVM>(sql).ToList();

                (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
                _htmlHelper.ViewBag.Data = data;

                output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("TagHelpers/Search/Index"));

                
            }
        }

    }
}
