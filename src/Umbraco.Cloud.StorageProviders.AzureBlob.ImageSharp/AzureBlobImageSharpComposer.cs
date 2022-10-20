using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.StorageProviders.AzureBlob.IO;

namespace Umbraco.Cloud.StorageProviders.AzureBlob;

/// <summary>
/// Automatically configures ImageSharp support using the Azure Blob Storage for use on Umbraco Cloud.
/// </summary>
/// <seealso cref="Umbraco.Cms.Core.Composing.IComposer" />
[ComposeAfter(typeof(AzureBlobComposer))]
public sealed class AzureBlobImageSharpComposer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (builder.Services.Any(x => x.ServiceType == typeof(IAzureBlobFileSystemProvider)))
        {
            // Configure ImageSharp support using Azure Blob Storage
            builder.AddAzureBlobImageSharpCache();
        }
    }
}
