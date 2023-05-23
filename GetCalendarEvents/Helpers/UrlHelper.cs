using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public static class UrlHelper
    {
        public static string? GetQueryParam(string url, string key)
        {
            if (string.IsNullOrEmpty(url)) return null;
            Uri uri = new Uri(url);

            // Get the query string parameters  
            Dictionary<string,string> queryParams = uri.Query
                .TrimStart('?')
                .Split('&')              
                .Select(q => q.Split('='))
                .ToDictionary(k => k[0], v => v[1]);
            string? value = null;
            queryParams.TryGetValue(key, out value); 


            return value ;
        }
    }
}
