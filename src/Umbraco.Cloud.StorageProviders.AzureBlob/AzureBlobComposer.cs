using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.StorageProviders.AzureBlob;

namespace Umbraco.Cloud.StorageProviders.AzureBlob
{
    public class AzureBlobComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("UMBRACO__CLOUD__ENVIRONMENT"))) return;

            builder.AddAzureBlobMediaFileSystem();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("umbraco-cloud.json", true, true)
                .AddEnvironmentVariables("UMBRACO:CLOUD:")
                .Build();

            builder.Services.Configure<AzureBlobFileSystemOptions>(AzureBlobFileSystemOptions.MediaFileSystemName,
                configuration.GetSection("Storage:AzureBlob").Bind);

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
