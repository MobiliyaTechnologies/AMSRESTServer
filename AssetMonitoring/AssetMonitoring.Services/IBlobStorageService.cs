using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetMonitoring.Contracts;

namespace AssetMonitoring.Services
{
    /// <summary>
    /// Provides blob storage operations.
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads the blob to blob storage, override blob if exist already.
        /// Create public container if not exists already.
        /// </summary>
        /// <param name="blobStorage">The BLOB storage model.</param>
        void UploadBlob(BlobStorage blobStorage);

        /// <summary>
        /// Rename existing blob.
        /// </summary>
        /// <param name="oldBlob">The old BLOB.</param>
        /// <param name="newBlobName">New name of the BLOB.</param>
        void RenameBlob(BlobStorage oldBlob, string newBlobName);

        /// <summary>
        /// Deletes the BLOB if exists.
        /// </summary>
        /// <param name="blobStorage">The BLOB storage model.</param>
        void DeleteBlob(BlobStorage blobStorage);

        /// <summary>
        /// Gets the BLOB uri.
        /// </summary>
        /// <param name="blobStorage">The BLOB storage model.</param>
        /// <returns>
        /// The blob uri.
        /// </returns>
        string GetBlobUri(BlobStorage blobStorage);

        /// <summary>
        /// Gets the container URI.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The blob container URI</returns>
        string GetContainerUri(string containerName);
    }
}
