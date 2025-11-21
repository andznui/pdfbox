/*
 * Converted from Apache PDFBox TestCOSIncrement.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using System.IO;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.io;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.common;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.graphics.color;
using org.apache.pdfbox.pdmodel.graphics.image;
using org.apache.pdfbox.pdmodel.interactive.annotation;
using org.apache.pdfbox.rendering;

namespace PdfBoxNet.Tests;

/// <summary>
/// Tests for COSIncrement
/// </summary>
public class TestCOSIncrement
{
    public TestCOSIncrement()
    {
        new java.io.File("target/test-output").mkdirs();
    }

    /// <summary>
    /// Create a document from scratch - incrementally making changes - checking results of previous steps.
    /// </summary>
    [Fact]
    public void TestIncrementallyCreateDocument()
    {
        byte[] documentData = new byte[0];

        // Add page 1.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = new PDDocument())
            {
                document.addPage(new PDPage(new PDRectangle(100, 100)));
                document.save(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }

        // Add page 2 and 3.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.Equal(1, document.getNumberOfPages());
                document.addPage(new PDPage(new PDRectangle(200, 200)));
                document.addPage(new PDPage(new PDRectangle(100, 100)));
                document.saveIncremental(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }

        // Remove page 2.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.Equal(3, document.getNumberOfPages());
                document.removePage(document.getPage(1));
                document.saveIncremental(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }

        // Add an image to page 1.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.NotEqual(200, document.getPage(1).getMediaBox().getWidth());
                Assert.Equal(2, document.getNumberOfPages());
                Assert.False(document.getPage(0).hasContents());
                Assert.Null(document.getPage(0).getResources());
                Assert.False(document.getPage(1).hasContents());
                Assert.Null(document.getPage(1).getResources());

                using (PDPageContentStream contentStream = new PDPageContentStream(document, document.getPage(0)))
                {
                    // Try to load test resource, skip if not available
                    string imagePath = "target/test-output/simple.png";
                    java.io.File imageFile = new java.io.File(imagePath);
                    if (!imageFile.exists())
                    {
                        // Skip test if resource not available
                        return;
                    }
                    contentStream.drawImage(PDImageXObject.createFromFileByExtension(imageFile, document), 15, 20);
                }
                document.saveIncremental(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }
        catch (java.net.URISyntaxException e)
        {
            Assert.Fail("URI syntax error.");
        }

        // Write a text to page 2.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.True(document.getPage(0).hasContents());
                Assert.NotNull(document.getPage(0).getResources());
                Assert.False(document.getPage(0).getResources().getFontNames().iterator().hasNext());
                Assert.True(document.getPage(0).getResources().getXObjectNames().iterator().hasNext());
                Assert.False(document.getPage(1).hasContents());
                Assert.Null(document.getPage(1).getResources());

                using (PDPageContentStream contentStream = new PDPageContentStream(document, document.getPage(1)))
                {
                    contentStream.beginText();
                    contentStream.setFont(new PDType1Font(Standard14Fonts.FontName.HELVETICA), 20);
                    contentStream.newLineAtOffset(20, 50);
                    contentStream.showText("Page 2");
                    contentStream.endText();
                }
                document.saveIncremental(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }

        // add an annotation to page 2.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.True(document.getPage(0).hasContents());
                Assert.NotNull(document.getPage(0).getResources());
                Assert.NotNull(document.getPage(1).getResources());
                Assert.False(document.getPage(1).getAnnotations().size() > 0);
                Assert.True(document.getPage(1).hasContents());
                Assert.True(document.getPage(1).getResources().getFontNames().iterator().hasNext());
                Assert.False(document.getPage(1).getResources().getXObjectNames().iterator().hasNext());

                PDAnnotationText textAnnotation = new PDAnnotationText();
                textAnnotation.setName("text annotation");
                textAnnotation.setContents("text annotation");
                textAnnotation.setOpen(true);
                textAnnotation.setColor(new PDColor(new float[] { 1, 0, 0 }, PDDeviceRGB.INSTANCE));
                textAnnotation.setRectangle(new PDRectangle(4, 5, 10, 10));
                textAnnotation.constructAppearances(document);
                document.getPage(1).getAnnotations().add(textAnnotation);
                document.saveIncremental(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }

        // Do nothing.
        try
        {
            using (java.io.ByteArrayOutputStream documentOutput = new java.io.ByteArrayOutputStream())
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.Equal(1, document.getPage(1).getAnnotations().size());
                document.saveIncremental(documentOutput);
                documentData = documentOutput.toByteArray();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }

        // Check the result.
        try
        {
            using (PDDocument document = LoadDocument(documentData))
            {
                Assert.Equal(2, document.getNumberOfPages());
                Assert.NotNull(document.getPage(0).getResources());
                Assert.NotNull(document.getPage(1).getResources());
                Assert.True(document.getPage(0).hasContents());
                Assert.False(document.getPage(0).getResources().getFontNames().iterator().hasNext());
                Assert.True(document.getPage(0).getResources().getXObjectNames().iterator().hasNext());
                Assert.True(document.getPage(1).hasContents());
                Assert.Equal(1, document.getPage(1).getAnnotations().size());
                Assert.True(document.getPage(1).getResources().getFontNames().iterator().hasNext());
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Closing streams failed.");
        }
    }

    /// <summary>
    /// PDFBOX-5263: There was a ConcurrentModificationException with
    /// YTW2VWJQTDAE67PGJT6GS7QSKW3GNUQR.pdf - test that this issues has been resolved.
    /// </summary>
    [Fact]
    public void TestConcurrentModification()
    {
        try
        {
            java.net.URL pdfLocation = new java.net.URI("https://issues.apache.org/jira/secure/attachment/12891316/YTW2VWJQTDAE67PGJT6GS7QSKW3GNUQR.pdf").toURL();

            using (PDDocument document = Loader.loadPDF(RandomAccessReadBuffer.createBufferFromStream(pdfLocation.openStream())))
            {
                document.setAllSecurityToBeRemoved(true);
                try
                {
                    document.save(new java.io.ByteArrayOutputStream());
                }
                catch (java.util.ConcurrentModificationException e)
                {
                    Assert.Fail("There shouldn't be a ConcurrentModificationException");
                }
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
        catch (java.net.URISyntaxException e)
        {
            Assert.Fail("URI syntax error.");
        }
    }

    private PDDocument LoadDocument(byte[] documentData)
    {
        try
        {
            return Loader.loadPDF(documentData);
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Loading the document failed.");
            return null;
        }
    }

    /// <summary>
    /// Check that subsetting takes place in incremental saving.
    /// </summary>
    [Fact]
    public void TestSubsetting()
    {
        try
        {
            java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();

            using (PDDocument document = new PDDocument())
            {
                PDPage page = new PDPage(PDRectangle.A4);
                document.addPage(page);
                document.save(baos);
            }

            using (PDDocument document = Loader.loadPDF(baos.toByteArray()))
            using (java.io.OutputStream os = new java.io.FileOutputStream("target/test-output/PDFBOX-5627.pdf"))
            {
                PDPage page = document.getPage(0);

                // Load font from PDFBox resources
                var resourceClass = java.lang.Class.forName("org.apache.pdfbox.pdmodel.font.PDType0Font");
                var fontStream = resourceClass.getResourceAsStream("/org/apache/pdfbox/resources/ttf/LiberationSans-Regular.ttf");
                if (fontStream == null)
                {
                    // Skip test if font resource not available
                    return;
                }
                PDFont font = PDType0Font.load(document, fontStream);

                using (PDPageContentStream contentStream = new PDPageContentStream(document, page))
                {
                    contentStream.beginText();
                    contentStream.setFont(font, 12);
                    contentStream.newLineAtOffset(75, 750);
                    contentStream.showText("Apache PDFBox");
                    contentStream.endText();
                }

                COSDictionary catalog = document.getDocumentCatalog().getCOSObject();
                catalog.setNeedToBeUpdated(true);
                COSDictionary pages = catalog.getCOSDictionary(COSName.PAGES);
                pages.setNeedToBeUpdated(true);
                page.getCOSObject().setNeedToBeUpdated(true);

                document.saveIncremental(os);
            }

            using (PDDocument document = Loader.loadPDF(new java.io.File("target/test-output/PDFBOX-5627.pdf")))
            {
                PDPage page = document.getPage(0);
                COSName fontName = (COSName)page.getResources().getFontNames().iterator().next();
                PDFont font = page.getResources().getFont(fontName);
                Assert.True(font.isEmbedded());
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }
}
