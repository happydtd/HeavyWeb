using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heavy.Web.TagHelpers
{
    //元素级或属性级tag helper
    [HtmlTargetElement(Attributes = "bold")]
    [HtmlTargetElement("bold")]
    public class BoldTagHelper :TagHelper
    {
        //把color属性和mycolor绑定
        [HtmlAttributeName("color")]
        public string MyColor { get; set; }
        public override void Process(
            TagHelperContext context,
            TagHelperOutput output)
        {
            output.Attributes.RemoveAll("bold"); 
            output.PreContent.SetHtmlContent("<strong>");
            output.PostContent.SetHtmlContent("</strong>");
            output.Attributes.SetAttribute("style", $"color: {MyColor}");
        }
    }
}
