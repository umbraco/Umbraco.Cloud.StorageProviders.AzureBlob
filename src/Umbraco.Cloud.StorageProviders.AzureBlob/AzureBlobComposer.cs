using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace Umbraco.Cloud.StorageProviders.AzureBlob
{
    /// <summary>
    /// Automatically configures Azure Blob Storage for use on Umbraco Cloud.
    /// </summary>
    /// <seealso cref="Umbraco.Cms.Core.Composing.IComposer" />
    public class AzureBlobComposer : IComposer
    {
        /// <inheritdoc />
        public void Compose(IUmbracoBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            /* There's a bug in Microsoft.Extensions.Configuration.EnvironmentVariables @ 6.0.0 WRT env var normalization + AddEnvironmentVariables(prefix) 
             * See https://github.com/dotnet/runtime/pull/62916, should be resolved upstream in https://github.com/dotnet/runtime/milestone/87
             * Until then, safest thing to do is explicitly add environment variables using both prefixes.
             * (otherwise there are issues when folks update TFM to net6) */
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables("Umbraco:Cloud:")
                .AddEnvironmentVariables("Umbraco__Cloud__")
                .Build();

            // Get options and manually validate (no need to add them to the service collection)
            var azureBlobOptions = configuration.GetSection("Storage:AzureBlob").Get<AzureBlobOptions>();
            if (azureBlobOptions == null) return;

            var validateResult = new DataAnnotationValidateOptions<AzureBlobOptions>(null).Validate(null, azureBlobOptions);
            if (validateResult.Failed) return;

            // Configure Azure Blob Storage
            builder.AddAzureBlobMediaFileSystem(options =>
            {
                options.ConnectionString = azureBlobOptions.ConnectionString;
                options.ContainerName = azureBlobOptions.ContainerName;
            });

            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                options.AddFilter(new UmbracoPipelineFilter("AzureBlob")
                {
                    Endpoints = app => app.UseAzureBlobMediaFileSystem()
                });
            });
        }
    }
}
