# Document Chunker

A .NET C# application that demonstrates various text chunking techniques for Natural Language Processing (NLP) applications.

## Overview

This application implements multiple text chunking strategies and outputs the results to a JSON file called `chunked-pieces.json`. Text chunking is essential for breaking down large documents into manageable pieces for processing by NLP models, search systems, and other text analysis tools.

## Chunking Strategies Implemented

1. **RecursiveCharacterTextSplitter** ⭐ **RECOMMENDED FOR LLM**
   - Splits text using hierarchical separators to maintain semantic coherence
   - Parameters: chunk_size=500, chunk_overlap=100
   - Separators (in priority order): ["\n\n", "\n", ".", " "]
   - Optimized for Large Language Model processing
   - Preserves context across chunk boundaries
   - Maintains natural language structure

2. **Fixed-Size Chunking**
   - Divides text into pieces of predetermined character length
   - Supports overlapping chunks to maintain context
   - Configuration: 500 characters per chunk with 50-character overlap

3. **Sentence-Based Chunking**
   - Respects sentence boundaries to maintain complete thoughts
   - Groups multiple sentences per chunk
   - Configuration: 3 sentences per chunk

4. **Paragraph-Based Chunking**
   - Maintains the logical structure of documents
   - Each chunk represents a complete paragraph
   - Preserves document organization

5. **Token-Based Chunking**
   - Considers vocabulary units (words/tokens)
   - Useful for language models with token limits
   - Configuration: 75 tokens per chunk

6. **Enriched Chunking** 🆕 **METADATA ENRICHMENT**
   - Adds metadata to each chunk
   - Source file identification
   - Page number tracking (if available)
   - Advanced analytics and reporting

## Project Structure

```
DocumentChunker/
├── Models/
│   ├── ChunkedPiece.cs          # Data model for chunked text pieces
│   └── DocumentMetadata.cs      # Enhanced metadata models
├── Services/
│   ├── ChunkingService.cs       # Core chunking algorithms + enrichment
│   ├── FileService.cs           # File I/O and JSON serialization
│   └── DocxService.cs           # DOCX file creation and reading
├── Sample/
│   └── sample-document.docx     # Generated sample DOCX file
├── Program.cs                   # Main application entry point
├── LLMChunker.cs               # LLM-focused chunking application
├── EnrichedChunker.cs          # Metadata enrichment application (NEW)
├── DocxGenerator.cs             # Standalone DOCX generator utility
├── chunked_pieces.json         # Generated output file (dictionary format) 🆕
├── llm-chunks.json             # Generated LLM-optimized chunks (dictionary format) 🆕
└── enriched-chunks-*.json     # Generated enriched chunks with metadata
```

## Output Format

The application generates a JSON file with the following dictionary structure:

```json
{
  "timestamp": "2025-08-07T10:30:00Z",
  "totalChunks": 28,
  "chunkingStrategy": "RecursiveCharacterSplit",
  "chunks": [
    {
      "text": "Text content of the chunk...",
      "metadata": {
        "id": 1,
        "startIndex": 0,
        "endIndex": 499,
        "length": 500,
        "chunkType": "RecursiveCharacterSplit",
        "additionalMetadata": {
          "actualSeparatorUsed": "\n\n",
          "chunkSize": 500,
          "overlap": 100
        }
      }
    }
  ]
}
```

## ChunkedPiece Properties

### Dictionary Format (New)
Each chunk is now represented as a dictionary with:
- **text**: The actual text content of the chunk
- **metadata**: All chunk information including:
  - **id**: Unique identifier for the chunk
  - **startIndex**: Starting position in the original document
  - **endIndex**: Ending position in the original document
  - **length**: Length of the chunk in characters
  - **chunkType**: The chunking strategy used (RecursiveCharacterSplit, FixedSize, Sentence, Paragraph, Token)
  - **additionalMetadata**: Strategy-specific information (separators, overlap, etc.)

### Basic ChunkedPiece (Legacy)
- **id**: Unique identifier for the chunk
- **content**: The actual text content
- **startIndex**: Starting position in the original document
- **endIndex**: Ending position in the original document
- **length**: Length of the chunk in characters
- **chunkType**: The chunking strategy used (FixedSize, Sentence, Paragraph, Token)
- **metadata**: Additional information specific to the chunking strategy

### EnrichedChunk (Enhanced) 🆕
All basic properties plus:
- **documentMetadata**: Document context
  - **source**: Source file name
  - **page**: Page number (if available)

## Usage

1. **Build the project:**
   ```bash
   dotnet build
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```
   The application will automatically:
   - Use the sample DOCX file from the `Sample/` folder
   - Apply RecursiveCharacterTextSplitter chunking (LLM-optimized)
   - Generate `chunked_pieces.json` with dictionary format (text + metadata)
   - Parameters: chunk_size=500, chunk_overlap=100, separators=["\n\n", "\n", ".", " "]

3. **Alternative chunking modes (optional):**
   - **LLM-Optimized Chunking only:** Use `LLMChunker.cs` directly
   - **Enriched Chunking with Metadata:** Use `EnrichedChunker.cs` for source and page enrichment
   - **Generate DOCX Sample only:** Use `DocxGenerator.cs`

5. **View the results:**
   - Check `chunked_pieces.json` for the main output ⭐
   - Check `llm-chunks.json` for LLM-optimized chunks (if using LLMChunker)
   - Check `enriched-chunks-*.json` for metadata-enriched chunks (if using EnrichedChunker) 🆕
     - `enriched-chunks-sample-document-page-1.json` - Chunks with page 1 metadata
     - `enriched-chunks-sample-document-page-2.json` - Chunks with page 2 metadata
     - `enriched-chunks-sample-document-no-page.json` - Chunks without page metadata
     - `enriched-chunks-comprehensive.json` - Complete enriched dataset
   - Check `Sample/sample-document.docx` for the source document

## Dependencies

- **.NET 9.0**: Target framework
- **Newtonsoft.Json**: For JSON serialization and formatting
- **DocumentFormat.OpenXml**: For creating and reading DOCX files

## Sample Documents

The application includes two types of sample documents:

### 1. Built-in Sample Document
- Simple text-based content about NLP and chunking
- Approximately 2,483 characters
- Embedded directly in the application code

### 2. Generated DOCX Sample Document
- Comprehensive document about advanced text processing
- Approximately 4,300+ characters
- Created programmatically with proper formatting
- Located in `Sample/sample-document.docx`
- Includes headings, subheadings, paragraphs, and bullet points

## Sample Folder Structure

```
Sample/
└── sample-document.docx    # Generated comprehensive DOCX document
```

The DOCX file contains structured content about:
- Advanced text processing techniques
- Document analysis methodologies
- Implementation considerations
- Real-world applications
- Future directions in the field

## Customization

You can modify the chunking parameters in `Program.cs`:

- Change chunk sizes and overlap amounts
- Adjust sentences per chunk
- Modify token limits
- Add new chunking strategies

## Use Cases

This chunking implementation is suitable for:

- **Document Analysis**: Breaking down large documents for processing
- **Search Systems**: Creating searchable text segments
- **Machine Learning**: Preparing training data for NLP models
- **Chatbot Training**: Maintaining conversational context
- **Content Summarization**: Processing documents in logical segments
