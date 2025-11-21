/*
 * Converted from Apache PDFBox TestPDDocument.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System.Text;
using Xunit;
using PdfBoxNet;
using org.apache.pdfbox;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdfwriter.compress;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test PDDocument basic operations
/// Converted from org.apache.pdfbox.pdmodel.TestPDDocument
/// </summary>
public class TestPDDocument : IDisposable
{
    private readonly string _testOutputDir;

    public TestPDDocument()
    {
        _testOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "test-output");
        Directory.CreateDirectory(_testOutputDir);
    }

    /// <summary>
    /// Test document save/load using a byte array stream
    /// </summary>
    [Fact]
    public void TestSaveLoadStream()
    {
        byte[] pdfBytes;

        // Create PDF with one blank page
        using (var document = new PDDocument())
        {
            document.addPage(new PDPage());

            // Save to byte array
            var baos = new java.io.ByteArrayOutputStream();
            document.save(baos, CompressParameters.NO_COMPRESSION);
            pdfBytes = baos.toByteArray();
        }

        // Verify content
        Assert.True(pdfBytes.Length > 200, "PDF should be larger than 200 bytes");

        string header = Encoding.UTF8.GetString(pdfBytes, 0, 8);
        Assert.Equal("%PDF-1.4", header);

        string footer = Encoding.UTF8.GetString(pdfBytes, pdfBytes.Length - 6, 6);
        Assert.Equal("%%EOF\n", footer);

        // Reload and verify
        using (var loadDoc = Loader.loadPDF(pdfBytes))
        {
            Assert.Equal(1, loadDoc.getNumberOfPages());
        }
    }

    /// <summary>
    /// Test document save/load using a file
    /// </summary>
    [Fact]
    public void TestSaveLoadFile()
    {
        string targetFile = Path.Combine(_testOutputDir, "pddocument-saveloadfile.pdf");

        // Create PDF with one blank page
        using (var document = new PDDocument())
        {
            document.addPage(new PDPage());
            document.save(targetFile, CompressParameters.NO_COMPRESSION);
        }

        // Verify file exists and has content
        Assert.True(File.Exists(targetFile), "PDF file should exist");

        var fileInfo = new FileInfo(targetFile);
        Assert.True(fileInfo.Length > 200, "PDF file should be larger than 200 bytes");

        // Verify content
        byte[] pdfBytes = File.ReadAllBytes(targetFile);
        Assert.True(pdfBytes.Length > 200);

        string header = Encoding.UTF8.GetString(pdfBytes, 0, 8);
        Assert.Equal("%PDF-1.4", header);

        string footer = Encoding.UTF8.GetString(pdfBytes, pdfBytes.Length - 6, 6);
        Assert.Equal("%%EOF\n", footer);

        // Reload and verify
        var javaFile = new java.io.File(targetFile);
        using (var loadDoc = Loader.loadPDF(javaFile))
        {
            Assert.Equal(1, loadDoc.getNumberOfPages());
        }
    }

    /// <summary>
    /// Test PDF version handling (get/set)
    /// </summary>
    [Fact]
    public void TestVersions()
    {
        // Test default version
        using (var document = new PDDocument())
        {
            Assert.Equal(1.4f, document.getVersion(), 2);
            Assert.Equal(1.4f, document.getDocument().getVersion(), 2);
            Assert.Equal("1.4", document.getDocumentCatalog().getVersion());

            // Force downgrading version (header)
            document.getDocument().setVersion(1.3f);
            document.getDocumentCatalog().setVersion(null);

            // Test new version (header)
            Assert.Equal(1.3f, document.getVersion(), 2);
            Assert.Equal(1.3f, document.getDocument().getVersion(), 2);
            Assert.Null(document.getDocumentCatalog().getVersion());
        }

        // Check if version downgrade is denied
        using (var document = new PDDocument())
        {
            document.setVersion(1.3f);

            // All versions shall have their default value
            Assert.Equal(1.4f, document.getVersion(), 2);
            Assert.Equal(1.4f, document.getDocument().getVersion(), 2);
            Assert.Equal("1.4", document.getDocumentCatalog().getVersion());

            // Check version upgrade
            document.setVersion(1.5f);

            // Overall version has to be 1.5f
            Assert.Equal(1.5f, document.getVersion(), 2);

            // Header version has to be unchanged
            Assert.Equal(1.4f, document.getDocument().getVersion(), 2);

            // Catalog version has to be 1.5
            Assert.Equal("1.5", document.getDocumentCatalog().getVersion());
        }

        // PDFBOX-5265: Check that all versions are 1.6 when compression is used (default)
        byte[] pdfBytes;
        using (var document = new PDDocument())
        {
            document.addPage(new PDPage());
            var baos = new java.io.ByteArrayOutputStream();
            document.save(baos);
            pdfBytes = baos.toByteArray();
        }

        using (var document = Loader.loadPDF(pdfBytes))
        {
            Assert.Equal("1.6", document.getDocumentCatalog().getVersion());
            Assert.Equal(1.6f, document.getDocument().getVersion(), 2);
            Assert.Equal(1.6f, document.getVersion(), 2);
        }

        string header = Encoding.UTF8.GetString(pdfBytes, 0, 8);
        Assert.Equal("%PDF-1.6", header);
    }

    /// <summary>
    /// Test whether a bad file can be deleted after load() failed
    /// </summary>
    [Fact]
    public void TestDeleteBadFile()
    {
        string badFile = Path.Combine(_testOutputDir, "testDeleteBadFile.pdf");

        // Create a bad PDF file
        File.WriteAllText(badFile, "<script language='JavaScript'>");

        // Parsing should fail
        var javaFile = new java.io.File(badFile);
        Assert.Throws<java.io.IOException>(() => Loader.loadPDF(javaFile));

        // File should be deletable after failed load
        try
        {
            File.Delete(badFile);
        }
        catch (IOException ex)
        {
            Assert.Fail($"Delete bad file failed after failed load: {ex.Message}");
        }
    }

    /// <summary>
    /// Test whether a good file can be deleted after loadPDF() and close() succeed
    /// </summary>
    [Fact]
    public void TestDeleteGoodFile()
    {
        string goodFile = Path.Combine(_testOutputDir, "testDeleteGoodFile.pdf");

        // Create a good PDF
        using (var doc = new PDDocument())
        {
            doc.addPage(new PDPage());
            doc.save(goodFile);
        }

        // Load and close
        var javaFile = new java.io.File(goodFile);
        Loader.loadPDF(javaFile).close();

        // File should be deletable after successful load and close
        try
        {
            File.Delete(goodFile);
        }
        catch (IOException ex)
        {
            Assert.Fail($"Delete good file failed after successful load() and close(): {ex.Message}");
        }
    }

    public void Dispose()
    {
        // Cleanup is handled by individual tests
    }
}
