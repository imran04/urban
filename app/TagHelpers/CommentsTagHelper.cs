using app.Models.ViewModels;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace app.TagHelpers
{
    public class CommentsTagHelper : TagHelper
    {
        IConfiguration configuration;
        private IHtmlHelper _htmlHelper;
        private IMemoryCache _cache;

        [HtmlAttributeName("booking-vm")]
        public BookingVm booking { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }
        public CommentsTagHelper(IConfiguration configuration, IHtmlHelper htmlHelper, IMemoryCache cache)
        {
            this.configuration = configuration;
            _htmlHelper = htmlHelper;
            _cache = cache;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("default")))
            {
                var sql = @"select * from booking_follow_up where booking_id=@id";
                List<BookingFollowUp> data;
                
                    data = connection.Query<BookingFollowUp>(sql,new { id = booking.booking_id}).OrderByDescending(i=> i.on_datetime).ToList();
                    
                (_htmlHelper as IViewContextAware).Contextualize(ViewContext);
                _htmlHelper.ViewBag.Data = data;
                _htmlHelper.ViewBag.Id = booking.booking_id;
                
                output.Content.SetHtmlContent(await _htmlHelper.PartialAsync("TagHelpers/Comments/Index"));


            }
        }

    }
}
