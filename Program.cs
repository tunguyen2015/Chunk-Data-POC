using DocumentChunker.Services;

namespace DocumentChunker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Document Chunking Application");
            Console.WriteLine("=============================\n");

            var chunkingService = new ChunkingService();
            var fileService = new FileService();
            var documentReader = new DocumentReaderService();

            try
            {
                // Read document from Sample folder
                Console.WriteLine("Reading document from Sample folder...");
                Console.WriteLine("Supported formats: TXT, DOCX, PDF, MD");
                var (document, sourceFileName) = await documentReader.ReadDocumentFromSampleFolderAsync();
                Console.WriteLine($"📄 Document length: {document.Length} characters");
                Console.WriteLine($"📄 Source: {sourceFileName}\n");

                // Apply RecursiveCharacterTextSplitter (LLM-optimized chunking)
                Console.WriteLine("⭐ Applying RecursiveCharacterTextSplitter (LLM-Optimized)");
                Console.WriteLine("   Parameters: chunk_size=500, chunk_overlap=100");
                Console.WriteLine("   Separators: [\"\\n\\n\", \"\\n\", \".\", \" \"]");
                
                var chunks = chunkingService.ChunkByRecursiveCharacterSplit(
                    document, 
                    chunkSize: 500, 
                    chunkOverlap: 100, 
                    separators: new[] { "\n\n", "\n", ".", " " }
                );
                
                Console.WriteLine($"   Generated {chunks.Count} LLM-friendly chunks\n");

                // Save to JSON file in dictionary format (text + metadata)
                var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "chunked_pieces.json");
                Console.WriteLine("💾 Saving chunks to chunked_pieces.json in dictionary format (text + metadata)...");
                await fileService.SaveChunksDictionaryToJsonAsync(chunks, outputPath, sourceFileName);

                Console.WriteLine($"\n🎉 Success! Generated {chunks.Count} chunks from sample document");
                Console.WriteLine($"📄 Source: {sourceFileName}");
                Console.WriteLine($"📊 Document length: {document.Length} characters");
                Console.WriteLine($"📁 Output file: {outputPath}");
                
                // Show chunk analysis
                Console.WriteLine("\n📊 Chunk Analysis:");
                var avgLength = chunks.Average(c => c.Length);
                var minLength = chunks.Min(c => c.Length);
                var maxLength = chunks.Max(c => c.Length);
                
                Console.WriteLine($"   Average chunk length: {avgLength:F0} characters");
                Console.WriteLine($"   Minimum chunk length: {minLength} characters");
                Console.WriteLine($"   Maximum chunk length: {maxLength} characters");

                // Analyze separator usage
                var separatorStats = chunks
                    .Where(c => c.Metadata.ContainsKey("actualSeparatorUsed") && c.Metadata["actualSeparatorUsed"] != null)
                    .GroupBy(c => c.Metadata["actualSeparatorUsed"]!.ToString()!)
                    .ToDictionary(g => g.Key, g => g.Count());

                Console.WriteLine("\n📋 Separator Usage:");
                foreach (var stat in separatorStats.OrderByDescending(s => s.Value))
                {
                    Console.WriteLine($"   {stat.Key}: {stat.Value} chunks");
                }

                Console.WriteLine("\n✨ Chunks are optimized for:");
                Console.WriteLine("   • Large Language Model processing");
                Console.WriteLine("   • Maintaining semantic coherence");
                Console.WriteLine("   • Preserving context across chunk boundaries");
                Console.WriteLine("   • Natural language boundaries");
                Console.WriteLine("   • Efficient token utilization");

                Console.WriteLine("\nPress any key to exit...");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine("\nPress any key to exit...");
                Console.Read();
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
