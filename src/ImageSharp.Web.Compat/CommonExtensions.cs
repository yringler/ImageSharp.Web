// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Text;
using System.Web;

namespace SixLabors.ImageSharp.Web
{
    /// <summary>
    /// Extension methods for commonly required types.
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// Returns the combined components of the request URL in a fully un-escaped form (except for the QueryString)
        /// suitable only for display. This format should not be used in HTTP headers or other HTTP operations.
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns>The <see cref="string"/></returns>
        public static string GetDisplayUrl(this HttpRequest request)
        {
            const string schemeDelimiter = "://";
            string host = request.Url.Host;
            string pathBase = request.ApplicationPath ?? string.Empty;
            string path = request.Path;

            // This is actually a HttpValueCollection so ToString() returns the value we need.
            string queryString = $"?{request.QueryString}";

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            int length = request.Url.Scheme.Length + schemeDelimiter.Length + host.Length
                         + pathBase.Length + path.Length + queryString.Length;

            return new StringBuilder(length)
                .Append(request.Url.Scheme)
                .Append(schemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }
    }
}
