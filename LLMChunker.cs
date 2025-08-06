using DocumentChunker.Services;

namespace DocumentChunker
{
    public class LLMChunker
    {
        public static async Task RunLLMChunkerAsync()
        {
            Console.WriteLine("LLM-Friendly Document Chunker");
            Console.WriteLine("RecursiveCharacterTextSplitter Implementation");
            Console.WriteLine("==============================================\n");

            var chunkingService = new ChunkingService();
            var fileService = new FileService();

            try
            {
                // Use built-in sample document
                Console.WriteLine("Using built-in sample document...");
                var document = GetSampleDocument();
                Console.WriteLine($"📄 Document length: {document.Length} characters");
                Console.WriteLine($"📄 Word count: {document.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length} words\n");

                // Apply RecursiveCharacterTextSplitter with LLM-optimized settings
                Console.WriteLine("🤖 Applying RecursiveCharacterTextSplitter for LLM processing:");
                Console.WriteLine("   Parameters:");
                Console.WriteLine("   - Chunk size: 500 characters");
                Console.WriteLine("   - Chunk overlap: 100 characters");
                Console.WriteLine("   - Separators: [\"\\n\\n\", \"\\n\", \".\", \" \"] (in priority order)");
                Console.WriteLine("   - Optimized for maintaining context and semantic meaning\n");

                var llmChunks = chunkingService.ChunkByRecursiveCharacterSplit(
                    document, 
                    chunkSize: 500, 
                    chunkOverlap: 100, 
                    separators: new[] { "\n\n", "\n", ".", " " }
                );

                Console.WriteLine($"✅ Generated {llmChunks.Count} LLM-friendly chunks\n");

                // Show chunk analysis
                Console.WriteLine("📊 Chunk Analysis:");
                var avgLength = llmChunks.Average(c => c.Length);
                var minLength = llmChunks.Min(c => c.Length);
                var maxLength = llmChunks.Max(c => c.Length);
                
                Console.WriteLine($"   Average chunk length: {avgLength:F0} characters");
                Console.WriteLine($"   Minimum chunk length: {minLength} characters");
                Console.WriteLine($"   Maximum chunk length: {maxLength} characters");

                // Analyze separator usage
                var separatorStats = llmChunks
                    .Where(c => c.Metadata.ContainsKey("actualSeparatorUsed") && c.Metadata["actualSeparatorUsed"] != null)
                    .GroupBy(c => c.Metadata["actualSeparatorUsed"]!.ToString()!)
                    .ToDictionary(g => g.Key, g => g.Count());

                Console.WriteLine("\n📋 Separator Usage:");
                foreach (var stat in separatorStats.OrderByDescending(s => s.Value))
                {
                    Console.WriteLine($"   {stat.Key}: {stat.Value} chunks");
                }

                // Show sample chunks
                Console.WriteLine("\n📖 Sample Chunks (first 3):");
                for (int i = 0; i < Math.Min(3, llmChunks.Count); i++)
                {
                    var chunk = llmChunks[i];
                    var preview = chunk.Content.Length > 150 
                        ? chunk.Content.Substring(0, 150) + "..." 
                        : chunk.Content;
                    
                    Console.WriteLine($"\n   Chunk {chunk.Id}:");
                    Console.WriteLine($"   Length: {chunk.Length} chars | Start: {chunk.StartIndex} | End: {chunk.EndIndex}");
                    Console.WriteLine($"   Separator: {chunk.Metadata.GetValueOrDefault("actualSeparatorUsed", "N/A")}");
                    Console.WriteLine($"   Content: \"{preview.Replace("\n", "\\n").Replace("\r", "")}\"");
                }

                // Save LLM-optimized chunks to JSON in dictionary format
                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "llm-chunks.json");
                Console.WriteLine($"\n💾 Saving LLM-optimized chunks to: llm-chunks.json (dictionary format)");
                await fileService.SaveChunksDictionaryToJsonAsync(llmChunks, outputPath);

                Console.WriteLine($"\n🎉 Success! Generated {llmChunks.Count} LLM-friendly chunks");
                Console.WriteLine("✨ These chunks are optimized for:");
                Console.WriteLine("   • Large Language Model processing");
                Console.WriteLine("   • Maintaining semantic coherence");
                Console.WriteLine("   • Preserving context across chunk boundaries");
                Console.WriteLine("   • Natural language boundaries");
                Console.WriteLine("   • Efficient token utilization");

                Console.WriteLine("\n📁 Output files:");
                Console.WriteLine($"   • {outputPath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static string GetSampleDocument()
        {
            return @"Advanced Text Processing and Document Analysis

Introduction

Document processing and text analysis have become increasingly important in the digital age. With the exponential growth of textual data, organizations need sophisticated tools to extract meaningful insights from documents. This comprehensive guide explores various techniques and methodologies for effective document processing.

Text chunking represents a fundamental preprocessing step in natural language processing workflows. By breaking down large documents into manageable segments, we can improve the efficiency and accuracy of downstream analysis tasks.

Chunking Methodologies

There are several established approaches to text chunking, each with distinct advantages and use cases:

Fixed-Size Chunking

Fixed-size chunking divides text into segments of predetermined length. This approach offers simplicity and predictability but may split sentences or paragraphs unnaturally. It's particularly useful when working with character or token limits imposed by machine learning models.

The key parameters for fixed-size chunking include chunk size and overlap. Overlap helps maintain context across chunk boundaries, which is crucial for tasks like semantic search or question answering systems.

Semantic Chunking

Semantic chunking attempts to preserve meaning by respecting natural language boundaries. This includes sentence-based chunking, which ensures each segment contains complete thoughts, and paragraph-based chunking, which maintains the logical structure of documents.

Advanced semantic chunking algorithms may use linguistic analysis to identify topic shifts, ensuring that related content remains grouped together. This approach is particularly valuable for document summarization and content analysis tasks.

Implementation Considerations

When implementing chunking strategies, several factors must be considered:

• Document type and structure
• Target application requirements
• Processing constraints and limitations
• Quality metrics and evaluation criteria

Performance optimization is crucial for large-scale document processing. Efficient algorithms and data structures can significantly reduce processing time while maintaining output quality.

Applications and Use Cases

Text chunking finds applications across numerous domains:

Information Retrieval

Search engines use chunking to create indexable segments that can be efficiently matched against user queries. Proper chunking improves search relevance and reduces computational overhead.

Machine Learning

Many natural language processing models have input length limitations. Chunking enables the processing of arbitrarily long documents by breaking them into model-compatible segments.

Content Management

Digital libraries and content management systems employ chunking for efficient storage, retrieval, and presentation of large documents. This enables features like progressive loading and targeted content delivery.

Future Directions

The field of document processing continues to evolve with advances in artificial intelligence and machine learning. Emerging techniques include:

Adaptive chunking algorithms that dynamically adjust segment size based on content characteristics. These systems can optimize chunk boundaries for specific applications, potentially improving downstream task performance.

Multi-modal chunking approaches that consider not only textual content but also document layout, formatting, and embedded media. This holistic approach is particularly relevant for processing complex documents like research papers, technical manuals, and multimedia presentations.

Conclusion

Effective text chunking is essential for modern document processing workflows. The choice of chunking strategy should align with specific application requirements and document characteristics. As the volume and complexity of textual data continue to grow, sophisticated chunking techniques will become increasingly important for extracting value from digital content.

Organizations investing in robust document processing capabilities will be better positioned to leverage their textual assets for competitive advantage. The techniques and principles outlined in this document provide a foundation for implementing effective chunking solutions.";
        }
    }
}
