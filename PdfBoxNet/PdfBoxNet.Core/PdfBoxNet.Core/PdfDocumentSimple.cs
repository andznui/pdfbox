using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.text;

namespace PdfBoxNet;

/// <summary>
/// Simple PDF document wrapper for basic operations
/// </summary>
public class SimplePdfDocument : IDisposable
{
    private PDDocument? _document;

    /// <summary>
    /// Load a PDF from file path
    /// </summary>
    public static SimplePdfDocument Load(string filePath)
    {
        var doc = org.apache.pdfbox.Loader.loadPDF(new java.io.File(filePath));
        return new SimplePdfDocument { _document = doc };
    }

    /// <summary>
    /// Load a PDF from byte array
    /// </summary>
    public static SimplePdfDocument Load(byte[] bytes)
    {
        var doc = org.apache.pdfbox.Loader.loadPDF(bytes);
        return new SimplePdfDocument { _document = doc };
    }

    /// <summary>
    /// Number of pages
    /// </summary>
    public int PageCount => _document?.getNumberOfPages() ?? 0;

    /// <summary>
    /// Extract all text
    /// </summary>
    public string ExtractText()
    {
        var stripper = new PDFTextStripper();
        return stripper.getText(_document);
    }

    /// <summary>
    /// Save to file
    /// </summary>
    public void Save(string path)
    {
        _document?.save(path);
    }

    public void Dispose()
    {
        _document?.close();
        _document = null;
    }
}
