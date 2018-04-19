namespace AssetMonitoring.Contracts.DocumentDbContract
{
    using System.Collections.Generic;

    public class DocumentDbPaginationResult<T>
    {
        public DocumentDbPaginationResult()
        {
            this.Result = new List<T>();
        }

        public List<T> Result { get; set; }

        public string ResponseContinuation { get; set; }
    }
}
