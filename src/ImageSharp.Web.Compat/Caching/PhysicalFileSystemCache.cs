// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using SixLabors.ImageSharp.Web.Memory;
using SixLabors.ImageSharp.Web.Middleware;

namespace SixLabors.ImageSharp.Web.Caching
{
    /// <summary>
    /// Implements a physical file system based cache.
    /// </summary>
    public class PhysicalFileSystemCache : IImageCache
    {
        /// <summary>
        /// The configuration key for determining the cache folder.
        /// </summary>
        public const string Folder = "CacheFolder";

        /// <summary>
        /// The default cache folder name.
        /// </summary>
        public const string DefaultCacheFolder = "is-cache";

        /// <summary>
        /// The configuration key for checking whether changes in source images should be accounted for when checking the cache.
        /// </summary>
        public const string CheckSourceChanged = "CheckSourceChanged";

        /// <summary>
        /// The default value for determining whether to check for changes in the source.
        /// </summary>
        public const string DefaultCheckSourceChanged = "false";

        /// <summary>
        /// The buffer manager.
        /// </summary>
        private readonly IBufferManager bufferManager;

        /// <summary>
        /// The middleware configuration options.
        /// </summary>
        private readonly ImageSharpMiddlewareOptions options;

        private readonly string absolutePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalFileSystemCache"/> class.
        /// </summary>
        /// <param name="bufferManager">An <see cref="IBufferManager"/> instance used to allocate arrays transporting encoded image data</param>
        /// <param name="options">The middleware configuration options</param>
        public PhysicalFileSystemCache(IBufferManager bufferManager, ImageSharpMiddlewareOptions options)
        {
            Guard.NotNull(bufferManager, nameof(bufferManager));
            Guard.NotNull(options, nameof(options));

            this.bufferManager = bufferManager;
            this.options = options;
            this.absolutePath = this.MapPath(this.Settings[Folder]);
        }

        /// <inheritdoc/>
        public IDictionary<string, string> Settings { get; }
            = new Dictionary<string, string>
            {
                { Folder, DefaultCacheFolder },
                { CheckSourceChanged, DefaultCheckSourceChanged }
            };

        /// <inheritdoc/>
        public async Task<IByteBuffer> GetAsync(string key)
        {
            var fileInfo = new FileInfo(this.ToFilePath(key));

            IByteBuffer buffer;

            // Check to see if the file exists.
            if (!fileInfo.Exists)
            {
                return default;
            }

            using (Stream stream = fileInfo.OpenRead())
            {
                int length = (int)stream.Length;

                // Buffer is disposed of in the middleware
                buffer = this.bufferManager.Allocate(length);
                await stream.ReadAsync(buffer.Array, 0, length);
            }

            return buffer;
        }

        /// <inheritdoc/>
        public Task<CachedInfo> IsExpiredAsync(HttpContext context, string key, DateTime minDateUtc)
        {
            bool.TryParse(this.Settings[CheckSourceChanged], out bool checkSource);

            var cachedFileInfo = new FileInfo(this.ToFilePath(key));
            bool exists = cachedFileInfo.Exists;
            DateTimeOffset lastModified = exists ? cachedFileInfo.LastWriteTime : DateTimeOffset.MinValue;
            long length = exists ? cachedFileInfo.Length : 0;
            bool expired = true;

            // Checking the source adds overhead but is configurable. Defaults to false
            if (checkSource)
            {
                var sourceFileInfo = new FileInfo(context.Request.Path);

                if (!sourceFileInfo.Exists)
                {
                    return Task.FromResult(default(CachedInfo));
                }

                // Check if the file exists and whether the last modified date is less than the min date.
                if (exists && lastModified.UtcDateTime > minDateUtc)
                {
                    // If it's newer than the cached file then it must be an update.
                    if (sourceFileInfo.LastWriteTimeUtc < lastModified.UtcDateTime)
                    {
                        expired = false;
                    }
                }
            }
            else
            {
                if (exists && lastModified.UtcDateTime > minDateUtc)
                {
                    expired = false;
                }
            }

            return Task.FromResult(new CachedInfo(expired, lastModified, length));
        }

        /// <inheritdoc/>
        public async Task<DateTimeOffset> SetAsync(string key, IByteBuffer value)
        {
            string path = this.ToFilePath(key);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (FileStream fileStream = File.Create(path))
            {
                await fileStream.WriteAsync(value.Array, 0, value.Length);
            }

            return File.GetLastWriteTimeUtc(path);
        }

        // TODO: Should we be defining some sort of equivalent to IFileProvider to handle this?
        private string MapPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }

            if (!path.StartsWith("~/"))
            {
                path = $"~/{path}";
            }

            if (path.Contains("/.."))
            {
                // If that is the case this means that the user may be traversing beyond the webroot
                // so we'll need to cater for that. HostingEnvironment.MapPath will throw a HttpException
                // if the request goes beyond the webroot so we'll need to use our own MapPath method.
                try
                {
                    return HostingEnvironment.MapPath(path);
                }
                catch (HttpException)
                {
                    // Need to use our own logic
                    return path.Replace("~/", HttpRuntime.AppDomainAppPath).Replace("/", "\\");
                }
            }

            return HostingEnvironment.MapPath(path);
        }

        /// <summary>
        /// Converts the key into a nested file path.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>The <see cref="string"/></returns>
        private string ToFilePath(string key)
        {
            return $"{this.absolutePath}/{string.Join("/", key.Substring(0, (int)this.options.CachedNameLength).ToCharArray())}/{key}";
        }
    }
}