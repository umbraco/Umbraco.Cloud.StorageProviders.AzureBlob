using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Umbraco.Cloud.StorageProviders.AzureBlob.ImageSharp;

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
        // Configure ImageSharp support using Azure Blob Storage
        if (AzureBlobComposer.TryGetOptions(out _))
        {
            builder.AddAzureBlobImageSharpCache();
        }
    }
}
