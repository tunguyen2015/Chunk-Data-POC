using DocumentChunker.Models;
using Newtonsoft.Json;

namespace DocumentChunker.Services
{
    public interface IFileService
    {
        Task SaveChunksToJsonAsync(List<ChunkedPiece> chunks, string filePath);
        Task SaveEnrichedChunksToJsonAsync(List<EnrichedChunk> chunks, string filePath);
        Task SaveChunksDictionaryToJsonAsync(List<ChunkedPiece> chunks, string filePath);
        Task<string> ReadSampleDocumentAsync();
    }

    public class FileService : IFileService
    {
        public async Task SaveChunksToJsonAsync(List<ChunkedPiece> chunks, string filePath)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(new
            {
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                totalChunks = chunks.Count,
                chunkingStrategy = chunks.FirstOrDefault()?.ChunkType ?? "Unknown",
                chunks = chunks
            }, jsonSettings);

            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task SaveEnrichedChunksToJsonAsync(List<EnrichedChunk> chunks, string filePath)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            var enrichmentSummary = new
            {
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                totalChunks = chunks.Count,
                chunkingStrategy = chunks.FirstOrDefault()?.ChunkType ?? "Unknown",
                sources = chunks.Select(c => c.DocumentMetadata.Source).Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList(),
                pages = chunks.Select(c => c.DocumentMetadata.Page).Where(p => p.HasValue).Distinct().ToList(),
                chunks = chunks
            };

            var json = JsonConvert.SerializeObject(enrichmentSummary, jsonSettings);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task SaveChunksDictionaryToJsonAsync(List<ChunkedPiece> chunks, string filePath)
        {
            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            // Convert chunks to dictionary format with text and metadata
            var chunkDictionaries = chunks.Select(chunk => new
            {
                text = chunk.Content,
                metadata = new
                {
                    id = chunk.Id,
                    startIndex = chunk.StartIndex,
                    endIndex = chunk.EndIndex,
                    length = chunk.Length,
                    chunkType = chunk.ChunkType,
                    additionalMetadata = chunk.Metadata
                }
            }).ToList();

            var output = new
            {
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                totalChunks = chunks.Count,
                chunkingStrategy = chunks.FirstOrDefault()?.ChunkType ?? "Unknown",
                chunks = chunkDictionaries
            };

            var json = JsonConvert.SerializeObject(output, jsonSettings);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<string> ReadSampleDocumentAsync()
        {
            // Sample document content
            var sampleDocument = @"
Natural Language Processing and Text Chunking

Natural Language Processing (NLP) is a branch of artificial intelligence that focuses on the interaction between computers and humans through natural language. The ultimate objective of NLP is to read, decipher, understand, and make sense of human languages in a valuable way.

Text chunking is a fundamental technique in NLP that involves breaking down large texts into smaller, more manageable pieces. This process is essential for various applications including document analysis, information retrieval, and machine learning model training.

There are several approaches to text chunking. Fixed-size chunking divides text into pieces of predetermined length, which is simple but may break sentences or paragraphs unnaturally. Sentence-based chunking respects sentence boundaries, ensuring that each chunk contains complete thoughts. Paragraph-based chunking maintains the logical structure of documents by keeping related sentences together.

Another important consideration in chunking is overlap. Overlapping chunks can help maintain context across boundaries, which is particularly useful for applications like semantic search or question answering systems. However, overlap also increases the total amount of text to process.

Token-based chunking is another strategy that considers the vocabulary units of the text. This approach is especially useful when working with language models that have specific token limits. By counting tokens rather than characters or words, we can ensure that chunks fit within model constraints.

The choice of chunking strategy depends on the specific use case and requirements. For document summarization, paragraph-based chunking might be most appropriate. For chatbot training, sentence-based chunking could preserve conversational flow. For search applications, fixed-size chunking with overlap might provide the best balance of coverage and efficiency.

Modern applications often employ hybrid approaches, combining multiple chunking strategies or using adaptive techniques that adjust chunk size based on content characteristics. These sophisticated methods can improve the quality of downstream NLP tasks significantly.

In conclusion, text chunking is a critical preprocessing step that can greatly impact the performance of NLP applications. Understanding the various approaches and their trade-offs is essential for building effective natural language processing systems.
";

            return await Task.FromResult(sampleDocument.Trim());
        }
    }
}
