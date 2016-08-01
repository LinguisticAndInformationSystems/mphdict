using Microsoft.AspNetCore.Razor.TagHelpers;
using mphdict.Models.morph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphweb.TagHelpers
{
    public class inflectionTagHelper : TagHelper
    {
        public word_param WordParam { get; set; }
        public int LangId { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "gd-entry");
            string templ = Models.mphEntry.formEntry(WordParam, LangId);
            output.Content.SetHtmlContent(templ);
        }
    }
}
