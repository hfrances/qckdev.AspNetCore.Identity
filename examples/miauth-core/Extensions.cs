using Microsoft.AspNetCore.Http;
using System;

namespace miauthcore
{
    static class Extensions
    {

        public static Uri GetUri(this HttpRequest request)
        {
            var rdo = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host
            };

            if (request.Host.Port != null)
            {
                rdo.Port = request.Host.Port.Value;
            }
            if (request.Path.HasValue)
            {
                rdo.Path = request.Path.Value;
            }
            if (request.Query != null)
            {
                rdo.Query = request.QueryString.Value;
            }
            return rdo.Uri;
        }

    }
}
