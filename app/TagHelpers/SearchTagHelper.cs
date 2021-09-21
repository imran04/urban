using app.Models.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
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
        private IMemoryCache _cache;
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public SearchTagHelper(IConfiguration configuration, IHtmlHelper htmlHelper,IMemoryCache cache )
        {
            this.configuration = configuration;
            _htmlHelper = htmlHelper;
            _cache = cache;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            using (var connection = new MySqlConnection(configuration.GetConnectionString("default")))
            {
                var sql = @"select category_name CategoryName,image Image,display Display from service_category where status=1;     
                            ";
                List<ServiceCategoryVM> data;
                if (!_cache.TryGetValue("CategorySearch", out data))
                {
                    data = connection.Query<ServiceCategoryVM>(sql).ToList();
                    _cache.Set<List<ServiceCategoryVM>>("CategorySearch", data);
                }

                (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
                _htmlHelper.ViewBag.Data = data;

                output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("TagHelpers/Search/Index"));

                
            }
        }

    }
}
