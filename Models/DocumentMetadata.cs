using Newtonsoft.Json;

namespace DocumentChunker.Models
{
    /// <summary>
    /// Document metadata for enriching chunks with contextual information
    /// </summary>
    public class DocumentMetadata
    {
        [JsonProperty("source")]
        public string Source { get; set; } = string.Empty; // file name

        [JsonProperty("page")]
        public int? Page { get; set; } // page number (if available)
    }

    /// <summary>
    /// Enhanced ChunkedPiece with enrichment metadata
    /// </summary>
    public class EnrichedChunk : ChunkedPiece
    {
        [JsonProperty("documentMetadata")]
        public DocumentMetadata DocumentMetadata { get; set; } = new DocumentMetadata();
    }
}
