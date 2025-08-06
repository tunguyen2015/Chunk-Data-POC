using DocumentChunker.Services;
using DocumentChunker.Models;

namespace DocumentChunker
{
    public class EnrichedChunker
    {
        public static async Task RunEnrichedChunkerAsync()
        {
            Console.WriteLine("📚 Enhanced Document Chunker with Metadata Enrichment");
            Console.WriteLine("=====================================================\n");

            var chunkingService = new ChunkingService();
            var fileService = new FileService();

            try
            {
                // Use built-in sample document
                Console.WriteLine("Using built-in sample document...");
                var document = GetSampleDocument();
                Console.WriteLine($"📄 Document length: {document.Length} characters\n");

                // Create enrichment metadata scenarios
                var scenarios = CreateEnrichmentScenarios();

                foreach (var scenario in scenarios)
                {
                    Console.WriteLine($"🔍 Processing scenario: {scenario.Name}");
                    Console.WriteLine($"   Source: {scenario.Metadata.Source}");
                    Console.WriteLine($"   Page: {scenario.Metadata.Page?.ToString() ?? "N/A"}");

                    // Apply enriched chunking
                    var enrichedChunks = chunkingService.ChunkWithEnrichment(
                        document, 
                        scenario.Metadata, 
                        "RecursiveCharacterSplit"
                    );

                    Console.WriteLine($"   Generated {enrichedChunks.Count} enriched chunks");

                    // Save enriched chunks to JSON
                    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"enriched-chunks-{scenario.Name.ToLower().Replace(" ", "-")}.json");
                    await fileService.SaveEnrichedChunksToJsonAsync(enrichedChunks, outputPath);
                    Console.WriteLine($"   💾 Saved to: {Path.GetFileName(outputPath)}\n");
                }

                // Create a comprehensive enriched dataset
                await CreateComprehensiveEnrichedDataset(document, chunkingService, fileService);

                Console.WriteLine("🎉 Enriched chunking completed successfully!");
                Console.WriteLine("\n📁 Generated Files:");
                Console.WriteLine("   • enriched-chunks-sample-document-page-1.json");
                Console.WriteLine("   • enriched-chunks-sample-document-page-2.json");
                Console.WriteLine("   • enriched-chunks-sample-document-no-page.json");
                Console.WriteLine("   • enriched-chunks-comprehensive.json");

                Console.WriteLine("\n✨ Enrichment Features:");
                Console.WriteLine("   • Source file identification");
                Console.WriteLine("   • Page number tracking (if available)");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private static List<EnrichmentScenario> CreateEnrichmentScenarios()
        {
            return new List<EnrichmentScenario>
            {
                new EnrichmentScenario
                {
                    Name = "Sample Document Page 1",
                    Metadata = new DocumentMetadata
                    {
                        Source = "built-in-sample.txt",
                        Page = 1
                    }
                },
                new EnrichmentScenario
                {
                    Name = "Sample Document Page 2",
                    Metadata = new DocumentMetadata
                    {
                        Source = "built-in-sample.txt",
                        Page = 2
                    }
                },
                new EnrichmentScenario
                {
                    Name = "Sample Document No Page",
                    Metadata = new DocumentMetadata
                    {
                        Source = "built-in-sample.txt",
                        Page = null
                    }
                }
            };
        }

        private static async Task CreateComprehensiveEnrichedDataset(string document, ChunkingService chunkingService, FileService fileService)
        {
            Console.WriteLine("🔄 Creating comprehensive enriched dataset...");

            var comprehensiveMetadata = new DocumentMetadata
            {
                Source = "built-in-sample.txt",
                Page = 1
            };

            var comprehensiveChunks = chunkingService.ChunkWithEnrichment(
                document, 
                comprehensiveMetadata, 
                "RecursiveCharacterSplit"
            );

            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "enriched-chunks-comprehensive.json");
            await fileService.SaveEnrichedChunksToJsonAsync(comprehensiveChunks, outputPath);

            Console.WriteLine($"✅ Comprehensive dataset saved: {Path.GetFileName(outputPath)}");
            Console.WriteLine($"   Total chunks: {comprehensiveChunks.Count}");
            Console.WriteLine($"   Source: {comprehensiveMetadata.Source}");
            Console.WriteLine($"   Page: {comprehensiveMetadata.Page}");
        }

        private class EnrichmentScenario
        {
            public string Name { get; set; } = string.Empty;
            public DocumentMetadata Metadata { get; set; } = new DocumentMetadata();
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
