using DocumentChunker.Models;
using System.Text.RegularExpressions;

namespace DocumentChunker.Services
{
    public interface IChunkingService
    {
        List<ChunkedPiece> ChunkByFixedSize(string text, int chunkSize, int overlap = 0);
        List<ChunkedPiece> ChunkBySentences(string text, int maxSentencesPerChunk = 3);
        List<ChunkedPiece> ChunkByParagraphs(string text);
        List<ChunkedPiece> ChunkByTokens(string text, int maxTokensPerChunk = 100);
        List<ChunkedPiece> ChunkByRecursiveCharacterSplit(string text, int chunkSize = 500, int chunkOverlap = 100, string[]? separators = null);
        
        // Enhanced methods with metadata enrichment
        List<EnrichedChunk> ChunkWithEnrichment(string text, DocumentMetadata metadata, string chunkingMethod = "RecursiveCharacterSplit");
        List<EnrichedChunk> EnrichExistingChunks(List<ChunkedPiece> chunks, DocumentMetadata metadata);
    }

    public class ChunkingService : IChunkingService
    {
        public List<ChunkedPiece> ChunkByFixedSize(string text, int chunkSize, int overlap = 0)
        {
            var chunks = new List<ChunkedPiece>();
            var id = 1;

            for (int i = 0; i < text.Length; i += chunkSize - overlap)
            {
                var endIndex = Math.Min(i + chunkSize, text.Length);
                var content = text.Substring(i, endIndex - i);

                chunks.Add(new ChunkedPiece
                {
                    Id = id++,
                    Content = content,
                    StartIndex = i,
                    EndIndex = endIndex - 1,
                    Length = content.Length,
                    ChunkType = "FixedSize",
                    Metadata = new Dictionary<string, object>
                    {
                        ["chunkSize"] = chunkSize,
                        ["overlap"] = overlap,
                        ["hasOverlap"] = overlap > 0 && i > 0
                    }
                });

                if (endIndex == text.Length)
                    break;
            }

            return chunks;
        }

        public List<ChunkedPiece> ChunkBySentences(string text, int maxSentencesPerChunk = 3)
        {
            var chunks = new List<ChunkedPiece>();
            var sentences = SplitIntoSentences(text);
            var id = 1;

            for (int i = 0; i < sentences.Count; i += maxSentencesPerChunk)
            {
                var chunkSentences = sentences.Skip(i).Take(maxSentencesPerChunk).ToList();
                var content = string.Join(" ", chunkSentences);
                var startIndex = text.IndexOf(chunkSentences.First());
                var endIndex = startIndex + content.Length - 1;

                chunks.Add(new ChunkedPiece
                {
                    Id = id++,
                    Content = content,
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    Length = content.Length,
                    ChunkType = "Sentence",
                    Metadata = new Dictionary<string, object>
                    {
                        ["sentenceCount"] = chunkSentences.Count,
                        ["maxSentencesPerChunk"] = maxSentencesPerChunk
                    }
                });
            }

            return chunks;
        }

        public List<ChunkedPiece> ChunkByParagraphs(string text)
        {
            var chunks = new List<ChunkedPiece>();
            var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
            var id = 1;
            var currentIndex = 0;

            foreach (var paragraph in paragraphs)
            {
                var startIndex = text.IndexOf(paragraph, currentIndex);
                var endIndex = startIndex + paragraph.Length - 1;

                chunks.Add(new ChunkedPiece
                {
                    Id = id++,
                    Content = paragraph.Trim(),
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    Length = paragraph.Length,
                    ChunkType = "Paragraph",
                    Metadata = new Dictionary<string, object>
                    {
                        ["wordCount"] = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length
                    }
                });

                currentIndex = endIndex + 1;
            }

            return chunks;
        }

        public List<ChunkedPiece> ChunkByTokens(string text, int maxTokensPerChunk = 100)
        {
            var chunks = new List<ChunkedPiece>();
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var id = 1;

            for (int i = 0; i < words.Length; i += maxTokensPerChunk)
            {
                var chunkWords = words.Skip(i).Take(maxTokensPerChunk).ToArray();
                var content = string.Join(" ", chunkWords);
                var startIndex = text.IndexOf(chunkWords.First());
                var endIndex = startIndex + content.Length - 1;

                chunks.Add(new ChunkedPiece
                {
                    Id = id++,
                    Content = content,
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    Length = content.Length,
                    ChunkType = "Token",
                    Metadata = new Dictionary<string, object>
                    {
                        ["tokenCount"] = chunkWords.Length,
                        ["maxTokensPerChunk"] = maxTokensPerChunk
                    }
                });
            }

            return chunks;
        }

        public List<ChunkedPiece> ChunkByRecursiveCharacterSplit(string text, int chunkSize = 500, int chunkOverlap = 100, string[]? separators = null)
        {
            // Default separators in order of preference for LLM-friendly chunks
            separators ??= new[] { "\n\n", "\n", ".", " " };
            
            var chunks = new List<ChunkedPiece>();
            var textsToProcess = new List<string> { text };
            var id = 1;

            // Process each separator in order
            foreach (var separator in separators)
            {
                var newTexts = new List<string>();
                
                foreach (var currentText in textsToProcess)
                {
                    if (currentText.Length <= chunkSize)
                    {
                        newTexts.Add(currentText);
                    }
                    else
                    {
                        var splitTexts = SplitTextBySeparator(currentText, separator, chunkSize);
                        newTexts.AddRange(splitTexts);
                    }
                }
                
                textsToProcess = newTexts;
            }

            // Create chunks with overlap
            var finalChunks = CreateChunksWithOverlap(textsToProcess, text, chunkSize, chunkOverlap);
            
            foreach (var chunk in finalChunks)
            {
                chunks.Add(new ChunkedPiece
                {
                    Id = id++,
                    Content = chunk.Content,
                    StartIndex = chunk.StartIndex,
                    EndIndex = chunk.EndIndex,
                    Length = chunk.Content.Length,
                    ChunkType = "RecursiveCharacterSplit",
                    Metadata = new Dictionary<string, object>
                    {
                        ["chunkSize"] = chunkSize,
                        ["chunkOverlap"] = chunkOverlap,
                        ["separators"] = separators,
                        ["hasOverlap"] = chunk.HasOverlap,
                        ["actualSeparatorUsed"] = chunk.SeparatorUsed
                    }
                });
            }

            return chunks;
        }

        private List<string> SplitIntoSentences(string text)
        {
            // Simple sentence splitting using regex
            var sentencePattern = @"[.!?]+\s+";
            var sentences = Regex.Split(text, sentencePattern)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            return sentences;
        }

        private List<string> SplitTextBySeparator(string text, string separator, int maxChunkSize)
        {
            var result = new List<string>();
            
            if (string.IsNullOrEmpty(separator))
            {
                // If no separator, split by character length
                for (int i = 0; i < text.Length; i += maxChunkSize)
                {
                    var chunk = text.Substring(i, Math.Min(maxChunkSize, text.Length - i));
                    if (!string.IsNullOrWhiteSpace(chunk))
                        result.Add(chunk);
                }
                return result;
            }

            var parts = text.Split(new[] { separator }, StringSplitOptions.None);
            var currentChunk = "";

            foreach (var part in parts)
            {
                var potentialChunk = string.IsNullOrEmpty(currentChunk) 
                    ? part 
                    : currentChunk + separator + part;

                if (potentialChunk.Length <= maxChunkSize)
                {
                    currentChunk = potentialChunk;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(currentChunk))
                    {
                        result.Add(currentChunk);
                    }
                    currentChunk = part;
                    
                    // If even a single part is too large, we'll need to split it further
                    if (part.Length > maxChunkSize)
                    {
                        if (!string.IsNullOrWhiteSpace(currentChunk))
                        {
                            result.Add(currentChunk);
                        }
                        // Split large part by character length
                        for (int i = 0; i < part.Length; i += maxChunkSize)
                        {
                            var subChunk = part.Substring(i, Math.Min(maxChunkSize, part.Length - i));
                            if (!string.IsNullOrWhiteSpace(subChunk))
                                result.Add(subChunk);
                        }
                        currentChunk = "";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(currentChunk))
            {
                result.Add(currentChunk);
            }

            return result.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        private List<ChunkInfo> CreateChunksWithOverlap(List<string> texts, string originalText, int chunkSize, int overlap)
        {
            var chunks = new List<ChunkInfo>();
            var processedTexts = new List<string>();

            // Merge small chunks and ensure proper sizing
            var currentChunk = "";
            
            foreach (var text in texts)
            {
                if (string.IsNullOrWhiteSpace(text)) continue;

                var potentialChunk = string.IsNullOrEmpty(currentChunk) 
                    ? text 
                    : currentChunk + " " + text;

                if (potentialChunk.Length <= chunkSize)
                {
                    currentChunk = potentialChunk;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentChunk))
                    {
                        processedTexts.Add(currentChunk);
                    }
                    currentChunk = text;
                }
            }

            if (!string.IsNullOrEmpty(currentChunk))
            {
                processedTexts.Add(currentChunk);
            }

            // Create final chunks with overlap
            var currentPosition = 0;
            
            for (int i = 0; i < processedTexts.Count; i++)
            {
                var chunkText = processedTexts[i];
                var startIndex = FindTextPosition(originalText, chunkText, currentPosition);
                var endIndex = Math.Min(startIndex + chunkText.Length - 1, originalText.Length - 1);

                chunks.Add(new ChunkInfo
                {
                    Content = chunkText,
                    StartIndex = startIndex,
                    EndIndex = endIndex,
                    HasOverlap = i > 0 && overlap > 0,
                    SeparatorUsed = DetermineSeparatorUsed(chunkText)
                });

                // Update position for next search
                currentPosition = Math.Max(0, endIndex - overlap + 1);

                // Add overlap for next chunk if needed
                if (i < processedTexts.Count - 1 && overlap > 0 && chunkText.Length > overlap)
                {
                    var overlapText = chunkText.Substring(Math.Max(0, chunkText.Length - overlap));
                    if (i + 1 < processedTexts.Count)
                    {
                        processedTexts[i + 1] = overlapText + " " + processedTexts[i + 1];
                    }
                }
            }

            return chunks;
        }

        private int FindTextPosition(string originalText, string chunkText, int startSearchFrom = 0)
        {
            // Clean the chunk text for searching (remove extra whitespace)
            var cleanChunkText = chunkText.Trim();
            if (string.IsNullOrEmpty(cleanChunkText)) return startSearchFrom;

            // Ensure startSearchFrom is within bounds
            startSearchFrom = Math.Max(0, Math.Min(startSearchFrom, originalText.Length - 1));

            // Try to find the exact text
            if (startSearchFrom < originalText.Length)
            {
                var position = originalText.IndexOf(cleanChunkText, startSearchFrom, StringComparison.OrdinalIgnoreCase);
                if (position >= 0) return position;
            }

            // If not found, try with first few words
            var words = cleanChunkText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length > 0 && startSearchFrom < originalText.Length)
            {
                var firstWords = string.Join(" ", words.Take(Math.Min(3, words.Length)));
                var position = originalText.IndexOf(firstWords, startSearchFrom, StringComparison.OrdinalIgnoreCase);
                if (position >= 0) return position;
            }

            // Fallback to safe position
            return Math.Max(0, Math.Min(startSearchFrom, originalText.Length - 1));
        }

        private string DetermineSeparatorUsed(string chunkText)
        {
            if (chunkText.Contains("\n\n")) return "\\n\\n (paragraph)";
            if (chunkText.Contains("\n")) return "\\n (line)";
            if (chunkText.Contains(".")) return ". (sentence)";
            return "space (word)";
        }

        // Enhanced methods with metadata enrichment
        public List<EnrichedChunk> ChunkWithEnrichment(string text, DocumentMetadata metadata, string chunkingMethod = "RecursiveCharacterSplit")
        {
            List<ChunkedPiece> baseChunks;

            // Apply the requested chunking method
            switch (chunkingMethod.ToLowerInvariant())
            {
                case "recursivecharactersplit":
                    baseChunks = ChunkByRecursiveCharacterSplit(text);
                    break;
                case "fixedsize":
                    baseChunks = ChunkByFixedSize(text, 500, 100);
                    break;
                case "sentences":
                    baseChunks = ChunkBySentences(text);
                    break;
                case "paragraphs":
                    baseChunks = ChunkByParagraphs(text);
                    break;
                case "tokens":
                    baseChunks = ChunkByTokens(text);
                    break;
                default:
                    baseChunks = ChunkByRecursiveCharacterSplit(text);
                    break;
            }

            return EnrichExistingChunks(baseChunks, metadata);
        }

        public List<EnrichedChunk> EnrichExistingChunks(List<ChunkedPiece> chunks, DocumentMetadata metadata)
        {
            var enrichedChunks = new List<EnrichedChunk>();

            for (int i = 0; i < chunks.Count; i++)
            {
                var chunk = chunks[i];
                var enriched = new EnrichedChunk
                {
                    Id = chunk.Id,
                    Content = chunk.Content,
                    StartIndex = chunk.StartIndex,
                    EndIndex = chunk.EndIndex,
                    Length = chunk.Length,
                    ChunkType = chunk.ChunkType,
                    Metadata = new Dictionary<string, object>(chunk.Metadata),
                    DocumentMetadata = metadata
                };

                // Add enrichment-specific metadata
                enriched.Metadata["enriched"] = true;
                enriched.Metadata["enrichmentTimestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                enriched.Metadata["chunkPosition"] = $"{i + 1}/{chunks.Count}";
                enriched.Metadata["source"] = metadata.Source;
                if (metadata.Page.HasValue)
                    enriched.Metadata["page"] = metadata.Page.Value;

                enrichedChunks.Add(enriched);
            }

            return enrichedChunks;
        }

        private class ChunkInfo
        {
            public string Content { get; set; } = string.Empty;
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public bool HasOverlap { get; set; }
            public string SeparatorUsed { get; set; } = string.Empty;
        }
    }
}
