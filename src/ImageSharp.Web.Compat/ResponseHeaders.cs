using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SixLabors.ImageSharp.Web
{
    internal class ResponseHeaders
    {
        public ResponseHeaders(NameValueCollection headers)
        {
            Guard.NotNull(headers, nameof(headers));

            this.Headers = headers;
        }

        public NameValueCollection Headers { get; }

        public CacheControlHeaderValue CacheControl
        {
            get
            {
                CacheControlHeaderValue.TryParse(this.Headers.Get(HeaderNames.CacheControl), out CacheControlHeaderValue control);
                return control;
            }
            set => this.Headers.Set(HeaderNames.CacheControl, value.ToString());
        }

        public long? ContentLength
        {
            get
            {
                long.TryParse(this.Headers.Get(HeaderNames.ContentLength), out long length);
                return length;
            }
            set => this.Headers.Set(HeaderNames.ContentLength, value.ToString());
        }

        public EntityTagHeaderValue ETag
        {
            get
            {
                EntityTagHeaderValue.TryParse(this.Headers.Get(HeaderNames.ETag), out EntityTagHeaderValue etag);
                return etag;
            }
            set => this.Headers.Set(HeaderNames.ETag, value.ToString());
        }

        public DateTimeOffset? LastModified
        {
            get
            {
                DateTimeOffset.TryParse(this.Headers.Get(HeaderNames.LastModified), out DateTimeOffset modified);
                return modified;
            }
            set => this.Headers.Set(HeaderNames.LastModified, value.Value.ToRfc1123String());
        }
    }
}