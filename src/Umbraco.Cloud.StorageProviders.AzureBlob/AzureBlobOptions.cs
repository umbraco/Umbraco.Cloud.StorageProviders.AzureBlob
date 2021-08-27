using System.ComponentModel.DataAnnotations;

namespace Umbraco.Cloud.StorageProviders.AzureBlob
{
    /// <summary>
    /// The Azure Blob options used on Umbraco Cloud.
    /// </summary>
    internal class AzureBlobOptions
    {
        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        [Required]
        public string Endpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets the shared access signature.
        /// </summary>
        /// <value>
        /// The shared access signature.
        /// </value>
        [Required]
        public string SharedAccessSignature { get; set; } = null!;

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString => $"BlobEndpoint={Endpoint};SharedAccessSignature={SharedAccessSignature}";

        /// <summary>
        /// Gets or sets the name of the container.
        /// </summary>
        /// <value>
        /// The name of the container.
        /// </value>
        [Required]
        public string ContainerName { get; set; } = null!;
    }
}
