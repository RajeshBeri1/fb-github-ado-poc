using Azure.Storage.Blobs;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// AzureStorage
    /// </summary>
    public class AzureBlobStorage
    {
        private readonly BlobServiceClient blobServiceClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobStorage" /> class.
        /// </summary>
        /// <param name="blobServiceClient">The BLOB service client.</param>
        public AzureBlobStorage(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Creates the container asynchronous.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task CreateContainerAsync(string containerName, CancellationToken cancellationToken)
        {
            var containers = blobServiceClient.GetBlobContainersAsync(cancellationToken: cancellationToken);

            var list = await containers.ToListAsync(cancellationToken);

            if (!list.Any(x => x.Name == containerName))
            {
                await blobServiceClient.CreateBlobContainerAsync(containerName, cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Downloads the asynchronous.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<Stream> DownloadAsync(string fileName, string containerName, CancellationToken cancellationToken)
        {
            var client = GetBlobClient(fileName, containerName);
            var response = await client.DownloadContentAsync(cancellationToken);
            return response.Value.Content.ToStream();
        }

        /// <summary>
        /// Uploads the asynchronous.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task UploadAsync(Stream stream, string fileName, string containerName, CancellationToken cancellationToken)
        {
            var client = GetBlobClient(fileName, containerName);
            await client.UploadAsync(stream, true, cancellationToken);
        }

        private BlobClient GetBlobClient(string fileName, string containerName)
        {
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            return container.GetBlobClient(fileName);
        }
    }
}