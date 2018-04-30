// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
#if COMPAT
using System.Collections.Specialized;
using System.Web;
#else
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#endif

namespace SixLabors.ImageSharp.Web.Commands
{
    /// <summary>
    /// Parses commands from the request querystring.
    /// </summary>
    public class QueryCollectionRequestParser : IRequestParser
    {
        /// <inheritdoc/>
        public IDictionary<string, string> ParseRequestCommands(HttpContext context)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

#if COMPAT
            NameValueCollection query = context.Request.QueryString;
            if (query.Count > 0)
            {
                foreach (string key in query.AllKeys)
                {
                    values.Add(key, query[key]);
                }
            }
#else
            IQueryCollection query = context.Request.Query;
            if (query.Any())
            {
                foreach (KeyValuePair<string, StringValues> pair in query)
                {
                    values.Add(pair.Key, pair.Value.ToString());
                }
            }
#endif
            return values;
        }
    }
}