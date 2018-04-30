// Copyright (c) Six Labors and contributors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Web;
using SixLabors.ImageSharp.Web.Middleware;

namespace SixLabors.ImageSharp.Web.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="HttpApplication"/> to simplify middleware registration.
    /// </summary>
    public static class HttpApplicationExtensions
    {
        /// <summary>
        /// Adds ImageSharp services to the specified <see cref="HttpApplication" /> with the given options.
        /// </summary>
        /// <param name="app">The application with the mechanism to configure a request pipeline</param>
        /// <returns><see cref="HttpApplication"/></returns>
        public static HttpApplication UseImageSharp(this HttpApplication app) => app.UseImageSharp(new ImageSharpMiddlewareOptions());

        /// <summary>
        /// Adds ImageSharp services to the specified <see cref="HttpApplication" /> with the given options.
        /// </summary>
        /// <param name="app">The application with the mechanism to configure a request pipeline</param>
        /// <param name="setupAction">The action used to configure the options.</param>
        /// <returns><see cref="HttpApplication"/></returns>
        public static HttpApplication UseImageSharp(this HttpApplication app, Action<ImageSharpMiddlewareOptions> setupAction)
        {
            var options = new ImageSharpMiddlewareOptions();
            setupAction(options);
            return app.UseImageSharp(options);
        }

        /// <summary>
        /// Adds ImageSharp services to the specified <see cref="HttpApplication" /> with the given options.
        /// </summary>
        /// <param name="app">The application with the mechanism to configure a request pipeline</param>
        /// <param name="options">The configuration options for the middleware</param>
        /// <returns><see cref="HttpApplication"/></returns>
        public static HttpApplication UseImageSharp(this HttpApplication app, ImageSharpMiddlewareOptions options)
        {
            // TODO Register services and pass to constructor.
            var middleware = new ImageSharpMiddleware();
            middleware.Init(app);
            return app;
        }

        /// <summary>
        /// Provides the means to add essential ImageSharp services to the specified <see cref="HttpApplication" /> with the default options.
        /// All additional services are required to be configured.
        /// </summary>
        /// <param name="app">The application with the mechanism to configure a request pipeline</param>
        /// <returns>An <see cref="ImageSharpCoreBuilder"/> that can be used to further configure the ImageSharp services.</returns>
        public static ImageSharpCoreBuilder UseImageSharpCore(this HttpApplication app)
        {
            var builder = new ImageSharpCoreBuilder();

            // TODO Register services and pass to constructor.
            var middleware = new ImageSharpMiddleware();
            middleware.Init(app);

            return builder;
        }
    }
}