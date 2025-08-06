using Newtonsoft.Json;

namespace DocumentChunker.Models
{
    public class ChunkedPiece
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;

        [JsonProperty("startIndex")]
        public int StartIndex { get; set; }

        [JsonProperty("endIndex")]
        public int EndIndex { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("chunkType")]
        public string ChunkType { get; set; } = string.Empty;

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }
}
