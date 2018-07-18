using Microsoft.AspNetCore.Razor.TagHelpers;
using mphdict.Models.morph;
using mphdict.Models.SynonymousSets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mphdeck.TagHelpers
{
    public class synsetTagHelper : TagHelper
    {
        public synsets SynSet { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "syn-entry");
            string templ = Models.synsetEntry.formEntry(SynSet);
            output.Content.SetHtmlContent(templ);
        }
    }
}
