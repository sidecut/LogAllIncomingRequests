using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;

namespace WebApplication7LogAllRequests.Utility
{
    public static class RequestHelper
    {
        public static string ReadBodyIntoString(HttpRequest request)
        {
            using (var reader = new StreamReader(request.Body))
            {
                string body = reader.ReadToEnd();
                return body;
            }
        }

        public static IEnumerable<(string, string)> GetAllHeaders(HttpRequest request)
        {
            return request.Headers
                .Select(requestHeader => (requestHeader.Key, requestHeader.Value.ToString()));
        }

        public static string GetRequestMethodAndUrl(HttpRequest request)
        {
            return $"{request.Method} {request.GetUri()} {request.Protocol}";
        }
    }
}
