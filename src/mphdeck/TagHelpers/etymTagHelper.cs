using Microsoft.AspNetCore.Razor.TagHelpers;
using mphdict.Models.morph;
using mphdict.Models.Etym;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdeck.TagHelpers
{
    public class etymTagHelper : TagHelper
    {
        public root Root { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "etym-entry");
            string templ = Models.etymEntry.formEntry(Root);
            output.Content.SetHtmlContent(templ);
        }
    }
}
