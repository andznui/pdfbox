# PdfBoxNet

.NET wrapper for Apache PDFBox using IKVM.

## Quick Start

```csharp
using PdfBoxNet;

// Load and extract text
using var pdf = SimplePdfDocument.Load("document.pdf");
Console.WriteLine($"Pages: {pdf.PageCount}");
string text = pdf.ExtractText();
Console.WriteLine(text);

// Load from bytes
byte[] pdfBytes = File.ReadAllBytes("document.pdf");
using var pdf2 = SimplePdfDocument.Load(pdfBytes);
pdf2.Save("output.pdf");
```

## Features

- ✅ Load PDFs from file or bytes
- ✅ Extract text
- ✅ Get page count
- ✅ Save PDFs
- ❌ JPEG2000 images not supported

## License

Apache 2.0
