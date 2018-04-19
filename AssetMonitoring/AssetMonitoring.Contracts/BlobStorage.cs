namespace AssetMonitoring.Contracts
{
    using System.IO;

    public class BlobStorage
    {
        public string BlobName { get; set; }

        public string BlobType { get; set; }

        public Stream Blob { get; set; }

        public string StorageContainer { get; set; }

        public bool IsPublicContainer { get; set; }
    }
}
