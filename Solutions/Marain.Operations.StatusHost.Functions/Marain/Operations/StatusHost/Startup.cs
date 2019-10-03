﻿// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.StatusHost.Startup))]

namespace Marain.Operations.StatusHost
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.AddLogging(logging =>
            {
#if DEBUG
                // Ensure you enable the required logging level in host.json
                // e.g:
                //
                // "logging": {
                //    "fileLoggingMode": "debugOnly",
                //    "logLevel": {
                //
                //    // For all functions
                //    "Function": "Debug",
                //
                //    // Default settings, e.g. for host
                //    "default": "Debug"
                // }
                logging.AddConsole();
#endif
            });

            IConfigurationRoot root = Configure(services);

            services.AddTenantedOperationsStatusApi(root, config =>
            {
                config.Documents.AddSwaggerEndpoint();
            });
        }

        private static IConfigurationRoot Configure(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot root = configurationBuilder.Build();
            services.AddSingleton(root);
            return root;
        }
    }
}
