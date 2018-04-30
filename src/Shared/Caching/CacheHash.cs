// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System.Buffers;
using System.Security.Cryptography;
using System.Text;
#if !COMPAT
using Microsoft.Extensions.Options;
#endif
using SixLabors.ImageSharp.Web.Helpers;
using SixLabors.ImageSharp.Web.Middleware;

namespace SixLabors.ImageSharp.Web.Caching
{
    /// <summary>
    /// Creates hashed keys for the given inputs hashing them to string of length ranging from 2 to 64.
    /// Hashed keys are the result of the SHA256 computation of the input value for the given length.
    /// This ensures low collision rates with a shorter file name.
    /// </summary>
    public sealed class CacheHash : ICacheHash
    {
        private readonly ImageSharpMiddlewareOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheHash"/> class.
        /// </summary>
        /// <param name="options">The middleware configuration options</param>
#if COMPAT
        public CacheHash(ImageSharpMiddlewareOptions options)
        {
            this.options = options;
        }
#else
        public CacheHash(IOptions<ImageSharpMiddlewareOptions> options)
        {
            this.options = options.Value;
        }
#endif

        /// <inheritdoc/>
        public string Create(string value, byte length)
        {
            Guard.MustBeBetweenOrEqualTo<byte>(length, 2, 64, nameof(length));

            using (var hashAlgorithm = SHA256.Create())
            {
                // Concatenate the hash bytes into one long string.
                int byteCount = Encoding.ASCII.GetByteCount(value);
                byte[] buffer = ArrayPool<byte>.Shared.Rent(byteCount);
                Encoding.ASCII.GetBytes(value, 0, value.Length, buffer, 0);
                byte[] hash = hashAlgorithm.ComputeHash(buffer, 0, byteCount);
                ArrayPool<byte>.Shared.Return(buffer);

                // PERF: Use length to allocate correct buffer size for StringBuilder.
                var sb = new StringBuilder(length);
                for (int i = 0; i < length / 2; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                sb.AppendFormat(".{0}", FormatHelpers.GetExtensionOrDefault(this.options.Configuration, value));
                return sb.ToString();
            }
        }
    }
}