using System.Diagnostics.CodeAnalysis;
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
    /// <summary>
    /// A delegate that is invoked when Azure Blob Storage is configured.
    /// </summary>
    /// <param name="azureBlobOptions">The Azure Blob options.</param>
    public delegate void ConfigureAzureBlobOptions(AzureBlobOptions azureBlobOptions);

    /// <summary>
    /// Gets or sets the delegate to invoke when Azure Blob Storage is configured.
    /// </summary>
    /// <value>
    /// The delegate that configures additional services.
    /// </value>
    public static ConfigureAzureBlobOptions? Configure { get; set; }

    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
    {
        // Configure Azure Blob Storage
        if (TryGetOptions(out AzureBlobOptions? azureBlobOptions))
        {
            builder.AddAzureBlobMediaFileSystem(options =>
            {
                options.ConnectionString = azureBlobOptions.ConnectionString;
                options.ContainerName = azureBlobOptions.ContainerName;
            });

            // Invoke delegate to configure additional services
            Configure?.Invoke(azureBlobOptions);
        }
    }

    /// <summary>
    /// Gets the validated Azure Blob Storage options from the environment variables.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>
    ///   <c>true</c> if the environment variables contains valid Azure Blob Storage options; otherwise, <c>false</c>.
    /// </returns>
    private static bool TryGetOptions([MaybeNullWhen(false)] out AzureBlobOptions options)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables("Umbraco:Cloud:")
            .Build();

        // Get options and manually validate (no need to add them to the service collection)
        options = configuration.GetSection("Storage:AzureBlob").Get<AzureBlobOptions>();
        if (options == null)
        {
            return false;
        }

        ValidateOptionsResult validateResult = new DataAnnotationValidateOptions<AzureBlobOptions>(null).Validate(null, options);
        if (validateResult.Failed)
        {
            options = null;
            return false;
        }

        return true;
    }
}
