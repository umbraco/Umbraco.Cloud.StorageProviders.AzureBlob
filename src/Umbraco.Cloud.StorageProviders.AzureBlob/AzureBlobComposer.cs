using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco.Cloud.StorageProviders.AzureBlob;

/// <summary>
/// Automatically configures Azure Blob Storage for use on Umbraco Cloud.
/// </summary>
/// <seealso cref="Umbraco.Cms.Core.Composing.IComposer" />
public sealed class AzureBlobComposer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // There was a bug with normalizing prefixes in .NET 6 (fixed in 6.0.2), so safest thing is to add both prefixes until the TFM is updated to net7.0
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables("Umbraco:Cloud:")
            .AddEnvironmentVariables("Umbraco__Cloud__")
            .Build();

        // Get options and manually validate (no need to add them to the service collection)
        var azureBlobOptions = configuration.GetSection("Storage:AzureBlob").Get<AzureBlobOptions>();
        if (azureBlobOptions == null)
        {
            return;
        }

        var validateResult = new DataAnnotationValidateOptions<AzureBlobOptions>(null).Validate(null, azureBlobOptions);
        if (validateResult.Failed)
        {
            return;
        }

        // Configure Azure Blob Storage
        builder.AddAzureBlobMediaFileSystem(options =>
        {
            options.ConnectionString = azureBlobOptions.ConnectionString;
            options.ContainerName = azureBlobOptions.ContainerName;
        });
    }
}
