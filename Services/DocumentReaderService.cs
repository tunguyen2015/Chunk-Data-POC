using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using System.Text;

namespace DocumentChunker.Services
{
    public interface IDocumentReaderService
    {
        Task<(string content, string fileName)> ReadDocumentFromSampleFolderAsync();
        Task<List<(string content, string fileName)>> ReadAllDocumentsFromSampleFolderAsync();
        Task<string> ReadDocumentAsync(string filePath);
        bool IsDocumentSupported(string filePath);
    }

    public class DocumentReaderService : IDocumentReaderService
    {
        private readonly string[] _supportedExtensions = { ".txt", ".docx", ".pdf", ".md" };

        public async Task<(string content, string fileName)> ReadDocumentFromSampleFolderAsync()
        {
            var sampleFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Sample");
            
            if (!Directory.Exists(sampleFolder))
            {
                throw new DirectoryNotFoundException($"Sample folder not found: {sampleFolder}");
            }

            // Look for supported file types in the Sample folder
            var sampleFiles = new List<string>();

            foreach (var ext in _supportedExtensions)
            {
                sampleFiles.AddRange(Directory.GetFiles(sampleFolder, $"*{ext}"));
            }

            if (!sampleFiles.Any())
            {
                throw new FileNotFoundException($"No supported files found in Sample folder. Supported formats: {string.Join(", ", _supportedExtensions)}");
            }

            // Use the first file found
            var filePath = sampleFiles.First();
            var fileName = System.IO.Path.GetFileName(filePath);

            Console.WriteLine($"📄 Found document: {fileName}");
            
            var content = await ReadDocumentAsync(filePath);
            return (content, fileName);
        }

        public async Task<List<(string content, string fileName)>> ReadAllDocumentsFromSampleFolderAsync()
        {
            var sampleFolder = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Sample");
            
            if (!Directory.Exists(sampleFolder))
            {
                throw new DirectoryNotFoundException($"Sample folder not found: {sampleFolder}");
            }

            // Look for supported file types in the Sample folder
            var sampleFiles = new List<string>();

            foreach (var ext in _supportedExtensions)
            {
                sampleFiles.AddRange(Directory.GetFiles(sampleFolder, $"*{ext}"));
            }

            if (!sampleFiles.Any())
            {
                throw new FileNotFoundException($"No supported files found in Sample folder. Supported formats: {string.Join(", ", _supportedExtensions)}");
            }

            Console.WriteLine($"📄 Found {sampleFiles.Count} documents in Sample folder:");
            
            var results = new List<(string content, string fileName)>();
            
            foreach (var filePath in sampleFiles)
            {
                var fileName = System.IO.Path.GetFileName(filePath);
                Console.WriteLine($"   • {fileName}");
                
                try
                {
                    var content = await ReadDocumentAsync(filePath);
                    results.Add((content, fileName));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️ Warning: Could not read {fileName}: {ex.Message}");
                }
            }

            return results;
        }

        public async Task<string> ReadDocumentAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var extension = System.IO.Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".txt" or ".md" => await ReadTextFileAsync(filePath),
                ".docx" => await ReadDocxFileAsync(filePath),
                ".pdf" => await ReadPdfFileAsync(filePath),
                _ => throw new NotSupportedException($"File type {extension} is not supported. Supported formats: {string.Join(", ", _supportedExtensions)}")
            };
        }

        public bool IsDocumentSupported(string filePath)
        {
            var extension = System.IO.Path.GetExtension(filePath).ToLower();
            return _supportedExtensions.Contains(extension);
        }

        private async Task<string> ReadTextFileAsync(string filePath)
        {
            Console.WriteLine($"📖 Reading text file: {System.IO.Path.GetFileName(filePath)}");
            return await File.ReadAllTextAsync(filePath);
        }

        private async Task<string> ReadDocxFileAsync(string filePath)
        {
            Console.WriteLine($"📖 Reading DOCX file: {System.IO.Path.GetFileName(filePath)}");
            
            return await Task.Run(() =>
            {
                var text = new List<string>();

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
                {
                    Body? body = wordDoc.MainDocumentPart?.Document?.Body;
                    if (body != null)
                    {
                        foreach (var paragraph in body.Elements<Paragraph>())
                        {
                            var paragraphText = paragraph.InnerText;
                            if (!string.IsNullOrWhiteSpace(paragraphText))
                            {
                                text.Add(paragraphText);
                            }
                        }
                    }
                }

                return string.Join("\n\n", text);
            });
        }

        private async Task<string> ReadPdfFileAsync(string filePath)
        {
            Console.WriteLine($"📖 Reading PDF file: {System.IO.Path.GetFileName(filePath)}");
            
            return await Task.Run(() =>
            {
                var text = new StringBuilder();

                try
                {
                    using (var reader = new PdfReader(filePath))
                    {
                        for (int page = 1; page <= reader.NumberOfPages; page++)
                        {
                            var pageText = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);
                            if (!string.IsNullOrWhiteSpace(pageText))
                            {
                                text.AppendLine(pageText);
                                text.AppendLine(); // Add spacing between pages
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to read PDF file: {ex.Message}", ex);
                }

                return text.ToString().Trim();
            });
        }
    }
}
