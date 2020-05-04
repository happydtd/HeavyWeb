using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heavy.Web.TagHelpers
{
    public class EmailTagHelper :TagHelper
    {
        public string MailTo { get; set; }
        //public override void Process(
        //    TagHelperContext context, 
        //    TagHelperOutput output)
        //{
        //    output.TagName = "a"; //replace <email> with <a> tag
        //    output.Attributes.SetAttribute("href", $"mailto:{MailTo}");
        //    output.Content.SetContent(MailTo);
        //}


        public async override Task ProcessAsync(
            TagHelperContext context,
            TagHelperOutput output)
        {
            output.TagName = "a"; //replace <email> with <a> tag
            var content = await output.GetChildContentAsync();
            var target = content.GetContent();
            output.Attributes.SetAttribute("href", $"mailto:{target}");
            output.Content.SetContent(target);
        }
    }
}
