namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System.Collections.Generic;

    public class DeleteDocumentDetail
    {
        public DeleteDocumentDetail()
        {
            this.SensorKeys = new List<string>();
            this.AssetBarcodes = new List<string>();
        }

        public int GroupId { get; set; }

        public List<string> AssetBarcodes { get; set; }

        public List<string> SensorKeys { get; set; }
    }
}
