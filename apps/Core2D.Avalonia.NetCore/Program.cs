﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Autofac;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.Avalonia.Modules;
using Core2D.Avalonia.NetCore.Modules;
using Core2D.Interfaces;
using Serilog;

namespace Core2D.Avalonia.NetCore
{
    /// <summary>
    /// Encapsulates an Avalonia program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        static void Main(string[] args)
        {
            try
            {
                InitializeLogging();

                var fbdev = args.Contains("--fbdev");
                var direct2d1 = args.Contains("--direct2d1");
                var skia = args.Contains("--skia");

                var builder = new ContainerBuilder();

                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DependenciesModule>();
                builder.RegisterModule<AppModule>();
                builder.RegisterModule<ViewModule>();

                if (fbdev == true)
                {
                    IContainer container = builder.Build();
                    var app = new App();
                    AppBuilder.Configure(app)
                        .InitializeWithLinuxFramebuffer(tl =>
                        {
                            tl.Content = app.CreateView(container.Resolve<IServiceProvider>());
                            ThreadPool.QueueUserWorkItem(_ => ConsoleSilencer());
                        });
                }
                else
                {
                    using (IContainer container = builder.Build())
                    {
                        using (ILog log = container.Resolve<ILog>())
                        {
                            var app = new App();
                            var appBuilder = AppBuilder.Configure(app).UsePlatformDetect();

                            if (direct2d1 == true)
                            {
                                appBuilder.UseDirect2D1();
                            }
                            else if (skia == true)
                            {
                                appBuilder.UseSkia();
                            }

                            appBuilder.SetupWithoutStarting();

                            var aboutInfo = app.CreateAboutInfo(
                                appBuilder.RuntimePlatform.GetRuntimeInfo(),
                                appBuilder.WindowingSubsystemName,
                                appBuilder.RenderingSubsystemName);
                            Debug.Write(aboutInfo);

                            app.Start(container.Resolve<IServiceProvider>(), aboutInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Silence the console.
        /// </summary>
        static void ConsoleSilencer()
        {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        }

        /// <summary>
        /// Initialize the Serilog logger.
        /// </summary>
        static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}
