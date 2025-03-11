using Azure;
using Azure.Data.Tables;


namespace US.BOX.BoxAPI.Data.Models
{
    public class DocumentEntity : ITableEntity
    {
        public string ActivityName { get; set; }
        public string CaseNo { get; set; }
        public string File { get; set; }
        public string Param { get; set; }
        public string ERROR { get; set; }
        public DateTime REGDATE { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; } = default!;
        public DateTimeOffset? Timestamp { get; set; } = default!;

    }
}
