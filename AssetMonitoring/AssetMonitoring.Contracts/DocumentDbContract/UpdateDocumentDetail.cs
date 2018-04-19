namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System.Collections.Generic;

    public class UpdateDocumentDetail
    {
        public UpdateDocumentDetail()
        {
            this.GroupAssets = new Dictionary<int, List<string>>();
        }

        public int NewGroupId { get; set; }

        public Dictionary<int, List<string>> GroupAssets { get; set; }
    }
}
