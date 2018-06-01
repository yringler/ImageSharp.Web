// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Web;

namespace SixLabors.ImageSharp.Web
{
    /// <summary>
    /// Extension methods for <see cref="HttpResponse"/>.
    /// </summary>
    public static class HttpResponseExtensions
    {
        internal static ResponseHeaders GetTypedHeaders(this HttpResponse response)
        {
            return new ResponseHeaders(response.Headers);
        }
    }
}