using PdfBoxNet;

Console.WriteLine("=== PdfBoxNet Example ===\n");

// Example 1: Create a simple text file to convert to PDF
var sampleText = @"This is a test document.
This demonstrates IKVM.NET wrapping Apache PDFBox.

Key Features:
- Load PDFs from file or bytes
- Extract text content
- Count pages
- Save PDFs

This wrapper uses Apache PDFBox 3.0.3 via IKVM 8.14.0.
";

File.WriteAllText("sample.txt", sampleText);
Console.WriteLine("✓ Created sample.txt");

// Note: For a real example, you would need an actual PDF file
// The code below shows how you would use the wrapper:

Console.WriteLine("\nExample usage (commented out - needs actual PDF):");
Console.WriteLine(@"
// Load a PDF
using var pdf = SimplePdfDocument.Load(""document.pdf"");

// Get page count
Console.WriteLine($""Pages: {pdf.PageCount}"");

// Extract all text
string text = pdf.ExtractText();
Console.WriteLine(text);

// Load from bytes
byte[] bytes = File.ReadAllBytes(""document.pdf"");
using var pdf2 = SimplePdfDocument.Load(bytes);

// Save to new file
pdf2.Save(""output.pdf"");
");

Console.WriteLine("\nIKVM.NET + PDFBox wrapper successfully built!");
Console.WriteLine("Add a PDF file to test the actual functionality.");
