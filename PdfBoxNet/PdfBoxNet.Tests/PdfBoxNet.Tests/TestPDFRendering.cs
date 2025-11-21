/*
 * Test suite for PDF rendering functionality
 * Demonstrates PDF-to-image conversion using PDFBox
 */

using System;
using System.IO;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.rendering;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Tests for PDF rendering (PDF to image conversion)
    /// </summary>
    public class TestPDFRendering
    {
        private readonly string testOutputDir;

        public TestPDFRendering()
        {
            testOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "test-output", "rendering");
            Directory.CreateDirectory(testOutputDir);
        }

        /// <summary>
        /// Test basic PDF rendering - create a PDF and render it to an image
        /// </summary>
        [Fact]
        public void TestBasicPDFRendering()
        {
            try
            {
                string pdfPath = Path.Combine(testOutputDir, "render-test.pdf");
                string imagePath = Path.Combine(testOutputDir, "render-test-page0.png");

                // Create a simple PDF with text
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);

                    using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(doc, page))
                    {
                        contentStream.beginText();
                        var font = new PDType1Font(Standard14Fonts.FontName.HELVETICA_BOLD);
                        contentStream.setFont(font, 24);
                        contentStream.newLineAtOffset(100, 700);
                        contentStream.showText("PDF Rendering Test");
                        contentStream.endText();
                    }

                    doc.save(pdfPath);
                }

                // Now render the PDF to an image
                using (PDDocument doc = Loader.loadPDF(new java.io.File(pdfPath)))
                {
                    PDFRenderer renderer = new PDFRenderer(doc);

                    // Render first page at 72 DPI
                    java.awt.image.BufferedImage image = renderer.renderImage(0);

                    // Verify image was created
                    Assert.NotNull(image);
                    Assert.True(image.getWidth() > 0);
                    Assert.True(image.getHeight() > 0);

                    // Save the rendered image
                    javax.imageio.ImageIO.write(image, "png", new java.io.File(imagePath));

                    // Verify file was created
                    Assert.True(File.Exists(imagePath));
                    Assert.True(new FileInfo(imagePath).Length > 1000); // Should be at least 1KB
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail($"IOException during rendering: {ex.Message}");
            }
        }

        /// <summary>
        /// Test rendering at different DPI values
        /// </summary>
        [Fact]
        public void TestRenderingAtDifferentDPI()
        {
            try
            {
                string pdfPath = Path.Combine(testOutputDir, "dpi-test.pdf");

                // Create a simple PDF
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);

                    using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(doc, page))
                    {
                        contentStream.beginText();
                        var font = new PDType1Font(Standard14Fonts.FontName.TIMES_ROMAN);
                        contentStream.setFont(font, 12);
                        contentStream.newLineAtOffset(50, 750);
                        contentStream.showText("Testing different DPI values");
                        contentStream.endText();
                    }

                    doc.save(pdfPath);
                }

                // Render at different DPIs
                using (PDDocument doc = Loader.loadPDF(new java.io.File(pdfPath)))
                {
                    PDFRenderer renderer = new PDFRenderer(doc);

                    // Render at 72 DPI
                    java.awt.image.BufferedImage image72 = renderer.renderImageWithDPI(0, 72);
                    Assert.NotNull(image72);
                    int width72 = image72.getWidth();
                    int height72 = image72.getHeight();

                    // Render at 144 DPI (2x)
                    java.awt.image.BufferedImage image144 = renderer.renderImageWithDPI(0, 144);
                    Assert.NotNull(image144);
                    int width144 = image144.getWidth();
                    int height144 = image144.getHeight();

                    // Higher DPI should result in larger image (approximately 2x)
                    Assert.True(width144 > width72);
                    Assert.True(height144 > height72);

                    // Should be approximately double (allow some rounding)
                    Assert.InRange(width144, width72 * 1.9, width72 * 2.1);
                    Assert.InRange(height144, height72 * 1.9, height72 * 2.1);

                    // Save both images for visual inspection
                    javax.imageio.ImageIO.write(image72, "png",
                        new java.io.File(Path.Combine(testOutputDir, "dpi-72.png")));
                    javax.imageio.ImageIO.write(image144, "png",
                        new java.io.File(Path.Combine(testOutputDir, "dpi-144.png")));
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail($"IOException during DPI test: {ex.Message}");
            }
        }

        /// <summary>
        /// Test rendering multiple pages
        /// </summary>
        [Fact]
        public void TestRenderingMultiplePages()
        {
            try
            {
                string pdfPath = Path.Combine(testOutputDir, "multi-page-test.pdf");

                // Create a multi-page PDF
                using (PDDocument doc = new PDDocument())
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        PDPage page = new PDPage();
                        doc.addPage(page);

                        using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(doc, page))
                        {
                            contentStream.beginText();
                            var font = new PDType1Font(Standard14Fonts.FontName.COURIER);
                            contentStream.setFont(font, 18);
                            contentStream.newLineAtOffset(200, 400);
                            contentStream.showText($"Page {i}");
                            contentStream.endText();
                        }
                    }

                    doc.save(pdfPath);
                }

                // Render all pages
                using (PDDocument doc = Loader.loadPDF(new java.io.File(pdfPath)))
                {
                    PDFRenderer renderer = new PDFRenderer(doc);

                    Assert.Equal(3, doc.getNumberOfPages());

                    for (int i = 0; i < 3; i++)
                    {
                        java.awt.image.BufferedImage image = renderer.renderImage(i);
                        Assert.NotNull(image);
                        Assert.True(image.getWidth() > 0);
                        Assert.True(image.getHeight() > 0);

                        // Save each page
                        string imagePath = Path.Combine(testOutputDir, $"multi-page-{i}.png");
                        javax.imageio.ImageIO.write(image, "png", new java.io.File(imagePath));
                        Assert.True(File.Exists(imagePath));
                    }
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail($"IOException during multi-page test: {ex.Message}");
            }
        }

        /// <summary>
        /// Test rendering an existing PDF file (if available)
        /// </summary>
        [Fact]
        public void TestRenderingExistingPDF()
        {
            try
            {
                // Try to load a test PDF
                var testFile = new java.io.File("target/pdfs/PDFBOX-3208-L33MUTT2SVCWGCS6UIYL5TH3PNPXHIS6.pdf");

                if (!testFile.exists())
                {
                    // Skip test if file doesn't exist
                    return;
                }

                using (PDDocument doc = Loader.loadPDF(testFile))
                {
                    PDFRenderer renderer = new PDFRenderer(doc);

                    // Render first page
                    java.awt.image.BufferedImage image = renderer.renderImage(0);

                    Assert.NotNull(image);
                    Assert.True(image.getWidth() > 0);
                    Assert.True(image.getHeight() > 0);

                    // Save rendered image
                    string imagePath = Path.Combine(testOutputDir, "existing-pdf-page0.png");
                    javax.imageio.ImageIO.write(image, "png", new java.io.File(imagePath));
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail($"IOException: {ex.Message}");
            }
        }

        /// <summary>
        /// Test rendering with different image types (RGB, ARGB, etc.)
        /// </summary>
        [Fact]
        public void TestRenderingImageTypes()
        {
            try
            {
                string pdfPath = Path.Combine(testOutputDir, "image-type-test.pdf");

                // Create a simple PDF
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);

                    using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(doc, page))
                    {
                        contentStream.setNonStrokingColor(java.awt.Color.BLUE);
                        contentStream.addRect(100, 100, 200, 200);
                        contentStream.fill();
                    }

                    doc.save(pdfPath);
                }

                // Render with default image type (RGB)
                using (PDDocument doc = Loader.loadPDF(new java.io.File(pdfPath)))
                {
                    PDFRenderer renderer = new PDFRenderer(doc);

                    java.awt.image.BufferedImage imageRGB = renderer.renderImage(0);
                    Assert.NotNull(imageRGB);

                    // Default rendering should produce an RGB image
                    int imageType = imageRGB.getType();
                    Assert.True(imageType == java.awt.image.BufferedImage.TYPE_INT_RGB ||
                               imageType == java.awt.image.BufferedImage.TYPE_3BYTE_BGR ||
                               imageType == java.awt.image.BufferedImage.TYPE_INT_BGR);

                    // Save the image
                    string imagePath = Path.Combine(testOutputDir, "image-type-rgb.png");
                    javax.imageio.ImageIO.write(imageRGB, "png", new java.io.File(imagePath));
                    Assert.True(File.Exists(imagePath));
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail($"IOException: {ex.Message}");
            }
        }
    }
}
