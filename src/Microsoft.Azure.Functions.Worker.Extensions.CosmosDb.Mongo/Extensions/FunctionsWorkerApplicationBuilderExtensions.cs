// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Azure.Functions.Worker
{
    /// <summary>
    /// Provides extension methods to work with a <see cref="IFunctionsWorkerApplicationBuilder"/>.
    /// </summary>
    public static class FunctionsWorkerApplicationBuilderExtensions
    {
        /// <summary>
        /// Configures the CosmosDB extension.
        /// </summary>
        /// <param name="builder">The <see cref="IFunctionsWorkerApplicationBuilder"/> to configure.</param>
        /// <returns>The same instance of the <see cref="IFunctionsWorkerApplicationBuilder"/> for chaining.</returns>
        public static IFunctionsWorkerApplicationBuilder ConfigureCosmosDBMongoExtension(this IFunctionsWorkerApplicationBuilder builder)
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            builder.Services.AddAzureClientsCore(); // Adds AzureComponentFactory
            builder.Services.AddOptions<CosmosDBMongoBindingOptions>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<CosmosDBMongoBindingOptions>, CosmosDBMongoBindingOptionsSetup>());

            return builder;
        }
    }

    internal static class ConfigurationExtensions
    {
        private const string WebJobsConfigurationSectionName = "AzureWebJobs";
        private const string ConnectionStringsConfigurationSectionName = "ConnectionStrings";

        /// <summary>
        /// Gets the configuration section for a given connection name.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionName">The connection string key.</param>
        internal static IConfigurationSection GetWebJobsConnectionStringSection(this IConfiguration configuration, string connectionName)
        {
            // first try prefixing
            string prefixedConnectionStringName = GetPrefixedConnectionStringName(connectionName);
            IConfigurationSection section = configuration.GetConnectionStringOrSetting(prefixedConnectionStringName);

            if (!section.Exists())
            {
                // next try a direct un-prefixed lookup
                section = configuration.GetConnectionStringOrSetting(connectionName);
            }

            return section;
        }


        /// <summary>
        /// Creates a WebJobs specific prefixed string using a given connection name.
        /// </summary>
        /// <param name="connectionName">The connection string key.</param>
        private static string GetPrefixedConnectionStringName(string connectionName)
        {
            return WebJobsConfigurationSectionName + connectionName;
        }

        /// <summary>
        /// Looks for a connection string by first checking the ConfigurationStrings section, and then the root.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionName">The connection string key.</param>
        private static IConfigurationSection GetConnectionStringOrSetting(this IConfiguration configuration, string connectionName)
        {
            if (configuration.GetSection(ConnectionStringsConfigurationSectionName).Exists())
            {
                IConfigurationSection onConnectionStrings = configuration.GetSection(ConnectionStringsConfigurationSectionName).GetSection(connectionName);
                if (onConnectionStrings.Exists())
                {
                    return onConnectionStrings;
                }
            }

            return configuration.GetSection(connectionName);
        }
    }
}
