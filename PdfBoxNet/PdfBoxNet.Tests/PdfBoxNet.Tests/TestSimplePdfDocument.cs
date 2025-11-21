/*
 * Simplified tests for SimplePdfDocument wrapper
 * Tests the PdfBoxNet IKVM.NET wrapper functionality
 */

using Xunit;
using PdfBoxNet;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test SimplePdfDocument wrapper class
/// </summary>
public class TestSimplePdfDocument
{
    private readonly string _testOutputDir;

    public TestSimplePdfDocument()
    {
        _testOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "test-output");
        Directory.CreateDirectory(_testOutputDir);
    }

    [Fact]
    public void TestCreateAndSaveEmptyPdf()
    {
        string outputPath = Path.Combine(_testOutputDir, "empty.pdf");

        // Create using Java API directly
        using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
        {
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.save(outputPath);
        }

        // Verify file was created
        Assert.True(File.Exists(outputPath));
        Assert.True(new FileInfo(outputPath).Length > 100);
    }

    [Fact]
    public void TestLoadPdfFromFile()
    {
        string testFile = Path.Combine(_testOutputDir, "load-test.pdf");

        // Create a test PDF
        using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
        {
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.save(testFile);
        }

        // Load using SimplePdfDocument
        using var loaded = SimplePdfDocument.Load(testFile);

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.PageCount);
    }

    [Fact]
    public void TestLoadPdfFromBytes()
    {
        // Create a test PDF in memory
        var baos = new java.io.ByteArrayOutputStream();
        using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
        {
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.save(baos);
        }

        byte[] pdfBytes = baos.toByteArray();

        // Load from bytes
        using var loaded = SimplePdfDocument.Load(pdfBytes);

        Assert.NotNull(loaded);
        Assert.Equal(3, loaded.PageCount);
    }

    [Fact]
    public void TestExtractText()
    {
        string testFile = Path.Combine(_testOutputDir, "text-test.pdf");

        // Create a PDF with text
        using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
        {
            var page = new org.apache.pdfbox.pdmodel.PDPage();
            doc.addPage(page);

            var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(doc, page);
            contentStream.beginText();

            // Use the new font API for PDFBox 3.x
            var font = new org.apache.pdfbox.pdmodel.font.PDType1Font(
                org.apache.pdfbox.pdmodel.font.Standard14Fonts.FontName.HELVETICA
            );
            contentStream.setFont(font, 12);
            contentStream.newLineAtOffset(100, 700);
            contentStream.showText("Hello from IKVM.NET!");
            contentStream.endText();
            contentStream.close();

            doc.save(testFile);
        }

        // Extract text
        using var loaded = SimplePdfDocument.Load(testFile);
        string text = loaded.ExtractText();

        Assert.NotNull(text);
        Assert.Contains("Hello from IKVM.NET!", text);
    }

    [Fact]
    public void TestSaveLoadCycle()
    {
        string originalFile = Path.Combine(_testOutputDir, "original.pdf");
        string savedFile = Path.Combine(_testOutputDir, "saved.pdf");

        // Create original
        using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
        {
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.save(originalFile);
        }

        // Load and save using SimplePdfDocument
        using (var loaded = SimplePdfDocument.Load(originalFile))
        {
            loaded.Save(savedFile);
        }

        // Verify saved file
        Assert.True(File.Exists(savedFile));
        using var reloaded = SimplePdfDocument.Load(savedFile);
        Assert.Equal(1, reloaded.PageCount);
    }

    [Fact]
    public void TestPageCount()
    {
        // Test with different page counts
        foreach (int pageCount in new[] { 1, 5, 10, 100 })
        {
            var baos = new java.io.ByteArrayOutputStream();
            using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
            {
                for (int i = 0; i < pageCount; i++)
                {
                    doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
                }
                doc.save(baos);
            }

            using var loaded = SimplePdfDocument.Load(baos.toByteArray());
            Assert.Equal(pageCount, loaded.PageCount);
        }
    }

    [Fact]
    public void TestDispose()
    {
        string testFile = Path.Combine(_testOutputDir, "dispose-test.pdf");

        using (var doc = new org.apache.pdfbox.pdmodel.PDDocument())
        {
            doc.addPage(new org.apache.pdfbox.pdmodel.PDPage());
            doc.save(testFile);
        }

        // Create and dispose
        var pdf = SimplePdfDocument.Load(testFile);
        Assert.Equal(1, pdf.PageCount);
        pdf.Dispose();

        // File should be accessible after dispose
        Assert.True(File.Exists(testFile));
    }

    [Fact]
    public void TestLoadNonExistentFile()
    {
        string nonExistent = Path.Combine(_testOutputDir, "does-not-exist.pdf");

        // PDFBox 3.x throws NoSuchFileException instead of FileNotFoundException
        Assert.Throws<java.nio.file.NoSuchFileException>(() =>
        {
            SimplePdfDocument.Load(nonExistent);
        });
    }

    [Fact]
    public void TestLoadInvalidPdf()
    {
        string badFile = Path.Combine(_testOutputDir, "invalid.pdf");
        File.WriteAllText(badFile, "This is not a PDF file");

        Assert.Throws<java.io.IOException>(() =>
        {
            SimplePdfDocument.Load(badFile);
        });
    }
}
