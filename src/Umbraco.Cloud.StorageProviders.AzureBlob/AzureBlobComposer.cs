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

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables("Umbraco:Cloud:")
                .Build();

            // Only configure when running on Umbraco Cloud with valid options
            var isRunningOnCloud = configuration.GetValue<bool>("IsRunningOnCloud");
            if (isRunningOnCloud == false) return;

            // Get options and manually validate (no need to register them into DI)
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
