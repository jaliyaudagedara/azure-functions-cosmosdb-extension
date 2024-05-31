﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using Microsoft.Azure.Functions.Worker.Extensions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.Functions.Worker
{
    internal class CosmosDBMongoBindingOptionsSetup : IConfigureNamedOptions<CosmosDBMongoBindingOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly AzureComponentFactory _componentFactory;
        private readonly IOptionsMonitor<WorkerOptions> _workerOptions;

        public CosmosDBMongoBindingOptionsSetup(IConfiguration configuration, AzureComponentFactory componentFactory, IOptionsMonitor<WorkerOptions> workerOptions)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _componentFactory = componentFactory ?? throw new ArgumentNullException(nameof(componentFactory));
            _workerOptions = workerOptions ?? throw new ArgumentNullException(nameof(workerOptions));
        }

        public void Configure(CosmosDBMongoBindingOptions options)
        {
            Configure(Options.DefaultName, options);
        }

        public void Configure(string connectionName, CosmosDBMongoBindingOptions options)
        {
            IConfigurationSection connectionSection = _configuration.GetWebJobsConnectionStringSection(connectionName);

            if (!connectionSection.Exists())
            {
                throw new InvalidOperationException($"Cosmos DB connection configuration '{connectionName}' does not exist. " +
                    "Make sure that it is a defined App Setting.");
            }

            options.ConnectionName = connectionName;

            options.ConnectionString = connectionSection.Value;
        }
    }
}
