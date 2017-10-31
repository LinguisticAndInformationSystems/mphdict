using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;

namespace mphweb.Providers
{
    public class UrlRequestCultureProvider: RequestCultureProvider
    {
        public UrlRequestCultureProvider(RequestLocalizationOptions Options)
        {
            this.Options = Options;
        }
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var url = httpContext.Request.Path;

            //Quick and dirty parsing of language from url path, which looks like /api/es-ES/hello-world
            var parts = httpContext.Request.Path.Value.Split('/').Where(p => !String.IsNullOrWhiteSpace(p)).ToList();
            if (parts.Count == 0)
            {
                //return Task.FromResult<ProviderCultureResult>(null);
                return Task.FromResult(new ProviderCultureResult("uk"));
            }

            var cultureSegmentIndex = parts.Contains("api") ? 1 : 0;
            var hasCulture = Regex.IsMatch(
            parts[cultureSegmentIndex], @"^[a-z]{2}(?:-[A-z]{2})?$");
            if (!hasCulture)
            {
                //return Task.FromResult<ProviderCultureResult>(null);
                return Task.FromResult(new ProviderCultureResult("uk"));
            }

            var culture = parts[cultureSegmentIndex];
            if(string.IsNullOrEmpty(culture)) return Task.FromResult(new ProviderCultureResult("uk"));
            return Task.FromResult(new ProviderCultureResult(culture));
        }
    }
}
