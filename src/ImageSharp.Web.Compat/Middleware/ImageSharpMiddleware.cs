// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Memory;
using SixLabors.ImageSharp.Web.Processors;
using SixLabors.ImageSharp.Web.Resolvers;

// [assembly: PreApplicationStartMethod(typeof(ImageSharpMiddleware), "Register")]
namespace SixLabors.ImageSharp.Web.Middleware
{
    /// <summary>
    /// Middleware for handling the processing of images via image requests.
    /// </summary>
    public class ImageSharpMiddleware : IHttpModule
    {
        /// <summary>
        /// The key-lock used for limiting identical requests
        /// </summary>
        private readonly IAsyncKeyLock asyncKeyLock;

        /// <summary>
        /// The configuration options
        /// </summary>
        private readonly ImageSharpMiddlewareOptions options;

        /// <summary>
        /// The buffer data pool.
        /// </summary>
        private readonly IBufferManager bufferManager;

        /// <summary>
        /// The parser for parsing commands from the current request.
        /// </summary>
        private readonly IRequestParser requestParser;

        /// <summary>
        /// The collection of image resolvers.
        /// </summary>
        private readonly IEnumerable<IImageResolver> resolvers;

        /// <summary>
        /// The collection of image processors.
        /// </summary>
        private readonly IEnumerable<IImageWebProcessor> processors;

        /// <summary>
        /// The image cache.
        /// </summary>
        private readonly IImageCache cache;

        /// <summary>
        /// The hashing implementation to use when generating cached file names.
        /// </summary>
        private readonly ICacheHash cacheHash;

        /// <summary>
        /// The collection of known commands gathered from the processors.
        /// </summary>
        private readonly IEnumerable<string> knownCommands;

        /// <inheritdoc />
        public void Init(HttpApplication context)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}