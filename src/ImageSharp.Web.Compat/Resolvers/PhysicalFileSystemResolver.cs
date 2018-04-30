// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using SixLabors.ImageSharp.Web.Helpers;
using SixLabors.ImageSharp.Web.Memory;
using SixLabors.ImageSharp.Web.Middleware;

namespace SixLabors.ImageSharp.Web.Resolvers
{
    /// <summary>
    /// Returns images stored in the local physical file system.
    /// </summary>
    public class PhysicalFileSystemResolver : IImageResolver
    {
        /// <summary>
        /// The buffer manager.
        /// </summary>
        private readonly IBufferManager bufferManager;

        /// <summary>
        /// The middleware configuration options.
        /// </summary>
        private readonly ImageSharpMiddlewareOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalFileSystemResolver"/> class.
        /// </summary>
        /// <param name="bufferManager">An <see cref="IBufferManager"/> instance used to allocate arrays transporting encoded image data</param>
        /// <param name="options">The middleware configuration options</param>
        public PhysicalFileSystemResolver(
            IBufferManager bufferManager,
            ImageSharpMiddlewareOptions options)
        {
            Guard.NotNull(bufferManager, nameof(bufferManager));
            Guard.NotNull(options, nameof(options));

            this.bufferManager = bufferManager;
            this.options = options;
        }

        /// <inheritdoc/>
        public Func<HttpContext, bool> Match { get; set; } = _ => true;

        /// <inheritdoc/>
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        /// <inheritdoc/>
        public Task<bool> IsValidRequestAsync(HttpContext context)
        {
            return Task.FromResult(FormatHelpers.GetExtension(this.options.Configuration, context.Request.GetDisplayUrl()) != null);
        }

        /// <inheritdoc/>
        public async Task<IByteBuffer> ResolveImageAsync(HttpContext context)
        {
            // Path has already been correctly parsed before here.
            var fileInfo = new FileInfo(HostingEnvironment.MapPath(context.Request.Path));
            IByteBuffer buffer;

            // Check to see if the file exists.
            if (!fileInfo.Exists)
            {
                return null;
            }

            using (Stream stream = fileInfo.OpenRead())
            {
                // Buffer is returned to the pool in the middleware
                buffer = this.bufferManager.Allocate((int)stream.Length);
                await stream.ReadAsync(buffer.Array, 0, (int)stream.Length);
            }

            return buffer;
        }
    }
}