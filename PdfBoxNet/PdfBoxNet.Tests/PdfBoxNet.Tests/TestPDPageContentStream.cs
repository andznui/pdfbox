/*
 * Converted from Apache PDFBox TestPDPageContentStream.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using System.Collections.Generic;
using Xunit;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.contentstream.@operator;
using org.apache.pdfbox.pdfparser;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.graphics.image;
using org.apache.pdfbox.pdmodel.graphics.shading;
using org.apache.pdfbox.pdmodel.graphics.state;
using org.apache.pdfbox.util;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test PDPageContentStream
/// Converted from org.apache.pdfbox.pdmodel.TestPDPageContentStream
/// </summary>
public class TestPDPageContentStream
{
    [Fact]
    public void TestSetCmykColors()
    {
        try
        {
            using (var doc = new PDDocument())
            {
                PDPage page = new PDPage();
                doc.addPage(page);

                using (var contentStream = new PDPageContentStream(doc, page, PDPageContentStream.AppendMode.OVERWRITE, true))
                {
                    // pass a non-stroking color in CMYK color space
                    contentStream.setNonStrokingColor(0.1f, 0.2f, 0.3f, 0.4f);

                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(1.1f, 0, 0, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(0, 1.1f, 0, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(0, 0, 1.1f, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(0, 0, 0, 1.1f));
                }

                // now read the PDF stream and verify that the CMYK values are correct
                PDFStreamParser parser = new PDFStreamParser(page);
                java.util.List pageTokens = parser.parse();
                // expected five tokens :
                // [0] = COSFloat{0.1}
                // [1] = COSFloat{0.2}
                // [2] = COSFloat{0.3}
                // [3] = COSFloat{0.4}
                // [4] = PDFOperator{"k"}
                Assert.Equal(0.1f, ((COSNumber)pageTokens.get(0)).floatValue(), 2);
                Assert.Equal(0.2f, ((COSNumber)pageTokens.get(1)).floatValue(), 2);
                Assert.Equal(0.3f, ((COSNumber)pageTokens.get(2)).floatValue(), 2);
                Assert.Equal(0.4f, ((COSNumber)pageTokens.get(3)).floatValue(), 2);
                Assert.Equal(OperatorName.NON_STROKING_CMYK, ((Operator)pageTokens.get(4)).getName());

                // same as above but for PDPageContentStream#setStrokingColor
                page = new PDPage();
                doc.addPage(page);

                using (var contentStream = new PDPageContentStream(doc, page, PDPageContentStream.AppendMode.OVERWRITE, false))
                {
                    // pass a stroking color in CMYK color space
                    contentStream.setStrokingColor(0.5f, 0.6f, 0.7f, 0.8f);

                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(1.1f, 0, 0, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(0, 1.1f, 0, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(0, 0, 1.1f, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(0, 0, 0, 1.1f));
                }

                // now read the PDF stream and verify that the CMYK values are correct
                parser = new PDFStreamParser(page);
                pageTokens = parser.parse();
                // expected five tokens  :
                // [0] = COSFloat{0.5}
                // [1] = COSFloat{0.6}
                // [2] = COSFloat{0.7}
                // [3] = COSFloat{0.8}
                // [4] = PDFOperator{"K"}
                Assert.Equal(0.5f, ((COSNumber)pageTokens.get(0)).floatValue(), 2);
                Assert.Equal(0.6f, ((COSNumber)pageTokens.get(1)).floatValue(), 2);
                Assert.Equal(0.7f, ((COSNumber)pageTokens.get(2)).floatValue(), 2);
                Assert.Equal(0.8f, ((COSNumber)pageTokens.get(3)).floatValue(), 2);
                Assert.Equal(OperatorName.STROKING_COLOR_CMYK, ((Operator)pageTokens.get(4)).getName());
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public void TestSetRGBandGColors()
    {
        try
        {
            using (var doc = new PDDocument())
            {
                PDPage page = new PDPage();
                doc.addPage(page);

                using (var contentStream = new PDPageContentStream(doc, page, PDPageContentStream.AppendMode.OVERWRITE, true))
                {
                    // pass a non-stroking color in RGB and Gray color space
                    contentStream.setNonStrokingColor(0.1f, 0.2f, 0.3f);
                    contentStream.setNonStrokingColor(0.8f);

                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(1.1f, 0, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(0, 1.1f, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(0, 0, 1.1f));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setNonStrokingColor(1.1f));
                }

                // now read the PDF stream and verify that the values are correct
                PDFStreamParser parser = new PDFStreamParser(page);
                java.util.List pageTokens = parser.parse();
                Assert.Equal(0.1f, ((COSNumber)pageTokens.get(0)).floatValue(), 2);
                Assert.Equal(0.2f, ((COSNumber)pageTokens.get(1)).floatValue(), 2);
                Assert.Equal(0.3f, ((COSNumber)pageTokens.get(2)).floatValue(), 2);
                Assert.Equal(OperatorName.NON_STROKING_RGB, ((Operator)pageTokens.get(3)).getName());
                Assert.Equal(0.8f, ((COSNumber)pageTokens.get(4)).floatValue(), 2);
                Assert.Equal(OperatorName.NON_STROKING_GRAY, ((Operator)pageTokens.get(5)).getName());

                // same as above but for PDPageContentStream#setStrokingColor
                page = new PDPage();
                doc.addPage(page);

                using (var contentStream = new PDPageContentStream(doc, page, PDPageContentStream.AppendMode.OVERWRITE, false))
                {
                    // pass a stroking color in RGB and Gray color space
                    contentStream.setStrokingColor(0.5f, 0.6f, 0.7f);
                    contentStream.setStrokingColor(0.8f);

                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(1.1f, 0, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(0, 1.1f, 0));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(0, 0, 1.1f));
                    Assert.Throws<java.lang.IllegalArgumentException>(() =>
                        contentStream.setStrokingColor(1.1f));
                }

                // now read the PDF stream and verify that the values are correct
                parser = new PDFStreamParser(page);
                pageTokens = parser.parse();
                Assert.Equal(0.5f, ((COSNumber)pageTokens.get(0)).floatValue(), 2);
                Assert.Equal(0.6f, ((COSNumber)pageTokens.get(1)).floatValue(), 2);
                Assert.Equal(0.7f, ((COSNumber)pageTokens.get(2)).floatValue(), 2);
                Assert.Equal(OperatorName.STROKING_COLOR_RGB, ((Operator)pageTokens.get(3)).getName());
                Assert.Equal(0.8f, ((COSNumber)pageTokens.get(4)).floatValue(), 2);
                Assert.Equal(OperatorName.STROKING_COLOR_GRAY, ((Operator)pageTokens.get(5)).getName());
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    /// <summary>
    /// PDFBOX-3510: missing content stream should not fail.
    /// </summary>
    [Fact]
    public void TestMissingContentStream()
    {
        try
        {
            PDPage page = new PDPage();
            PDFStreamParser parser = new PDFStreamParser(page);
            java.util.List tokens = parser.parse();
            Assert.Equal(0, tokens.size());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    /// <summary>
    /// Check that close() can be called twice.
    /// </summary>
    [Fact]
    public void TestCloseContract()
    {
        try
        {
            using (var doc = new PDDocument())
            {
                PDPage page = new PDPage();
                doc.addPage(page);
                PDPageContentStream contentStream = new PDPageContentStream(doc, page, PDPageContentStream.AppendMode.OVERWRITE, true);
                contentStream.close();
                contentStream.close();
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    /// <summary>
    /// Check that general graphics state operators are allowed in text mode.
    /// </summary>
    [Fact]
    public void TestGeneralGraphicStateOperatorTextMode()
    {
        try
        {
            using (var doc = new PDDocument())
            {
                PDPage page = new PDPage();
                doc.addPage(page);
                PDPageContentStream contentStream = new PDPageContentStream(doc, page);
                contentStream.beginText();

                PDImageXObject img1 = new PDImageXObject(doc);
                PDInlineImage img2 = new PDInlineImage(new COSDictionary(), new byte[0], new PDResources());
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.drawImage(img1, 0f, 0f, 1f, 1f));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.drawImage(img1, new Matrix()));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.drawImage(img2, 0f, 0f, 1f, 1f));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.addRect(0, 0, 1, 1));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.curveTo(0, 0, 1, 1, 2, 2));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.curveTo1(0, 0, 1, 1));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.curveTo2(0, 0, 1, 1));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.moveTo(0, 0));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.lineTo(1, 1));
                Assert.Throws<java.lang.IllegalStateException>(() =>
                    contentStream.shadingFill(new PDShadingType1(new COSDictionary())));
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.stroke());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.closeAndStroke());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.closeAndFillAndStroke());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.closeAndFillAndStrokeEvenOdd());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.fill());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.fillAndStroke());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.fillAndStrokeEvenOdd());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.fillEvenOdd());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.closePath());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.clip());
                Assert.Throws<java.lang.IllegalStateException>(() => contentStream.clipEvenOdd());

                // J
                contentStream.setLineCapStyle(0);
                // j
                contentStream.setLineJoinStyle(0);
                // w
                contentStream.setLineWidth(10f);
                // d
                contentStream.setLineDashPattern(new float[] { 2, 1 }, 0f);
                // M
                contentStream.setMiterLimit(1.0f);
                // gs
                contentStream.setGraphicsStateParameters(new PDExtendedGraphicsState());
                // ri, i are not supported with a specific setter
                contentStream.endText();
                contentStream.close();
            }
        }
        catch (java.lang.IllegalArgumentException exception)
        {
            Assert.Fail(exception.getMessage());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }
}
