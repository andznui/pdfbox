/*
 * Converted from Apache PDFBox PageExtractorTest.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using System.IO;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.multipdf;
using org.apache.pdfbox.pdmodel;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test suite for PageExtractor.
/// This is just some simple tests based on a test document. It merely ensures
/// that the correct number of pages are extracted as this is virtually the only
/// thing which could go wrong when coping pages from one PDF to a new one.
/// Converted from org.apache.pdfbox.multipdf.PageExtractorTest
/// </summary>
public class PageExtractorTest
{
    private void CloseDoc(PDDocument doc)
    {
        if (doc != null)
        {
            try
            {
                doc.close();
            }
            catch (Exception)
            {
                // Can't do much about this...
            }
        }
    }

    /// <summary>
    /// Test of extract method, of class org.apache.pdfbox.util.PageExtractor.
    /// </summary>
    [Fact]
    public void TestExtract()
    {
        PDDocument sourcePdf = null;
        PDDocument result = null;
        try
        {
            // this should work for most users
            string testFile = Path.Combine("src", "test", "resources", "input", "cweb.pdf");
            if (!File.Exists(testFile))
            {
                // Try from the pdfbox subdirectory
                testFile = Path.Combine("pdfbox", testFile);
            }

            if (!File.Exists(testFile))
            {
                // Skip test if file doesn't exist
                return;
            }

            sourcePdf = Loader.loadPDF(new java.io.File(testFile));
            PageExtractor instance = new PageExtractor(sourcePdf);
            result = instance.extract();
            Assert.Equal(sourcePdf.getNumberOfPages(), result.getNumberOfPages());
            CloseDoc(result);

            instance = new PageExtractor(sourcePdf, 1, 1);
            result = instance.extract();
            Assert.Equal(1, result.getNumberOfPages());
            CloseDoc(result);

            instance = new PageExtractor(sourcePdf, 1, 5);
            result = instance.extract();
            Assert.Equal(5, result.getNumberOfPages());
            CloseDoc(result);

            instance = new PageExtractor(sourcePdf, 5, 10);
            result = instance.extract();
            Assert.Equal(6, result.getNumberOfPages());
            CloseDoc(result);

            instance = new PageExtractor(sourcePdf, 2, 1);
            result = instance.extract();
            Assert.Equal(0, result.getNumberOfPages());
            CloseDoc(result);
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
        finally
        {
            CloseDoc(sourcePdf);
            CloseDoc(result);
        }
    }
}
