/*
 * Converted from Apache PDFBox ValidateXImage.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using System.Collections.Generic;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.graphics.image;
using org.apache.pdfbox.rendering;

namespace PdfBoxNet.Tests;

/// <summary>
/// Helper class to do some validations for PDImageXObject.
/// Converted from org.apache.pdfbox.pdmodel.graphics.image.ValidateXImage
/// </summary>
public static class ValidateXImage
{
    public static void validate(PDImageXObject ximage, int bpc, int width, int height, string format, string colorSpaceName)
    {
        // check the dictionary
        Assert.NotNull(ximage);
        var cosObj = ximage.getCOSObject();
        Assert.NotNull(cosObj);
        Assert.Equal(COSName.XOBJECT, cosObj.getItem(COSName.TYPE));
        Assert.Equal(COSName.IMAGE, cosObj.getItem(COSName.SUBTYPE));
        // In PDFBox 3.x, getCOSObject() returns COSDictionary, which is also a COSStream for images
        if (cosObj is COSStream cosStream)
        {
            Assert.True(cosStream.getLength() > 0);
        }
        Assert.Equal(bpc, ximage.getBitsPerComponent());
        Assert.Equal(width, ximage.getWidth());
        Assert.Equal(height, ximage.getHeight());
        Assert.Equal(format, ximage.getSuffix());
        Assert.Equal(colorSpaceName, ximage.getColorSpace().getName());

        // check the image
        Assert.NotNull(ximage.getImage());
        Assert.Equal(ximage.getWidth(), ximage.getImage().getWidth());
        Assert.Equal(ximage.getHeight(), ximage.getImage().getHeight());
        var rawRaster = ximage.getRawRaster();
        Assert.NotNull(rawRaster);
        Assert.Equal(rawRaster.getWidth(), ximage.getWidth());
        Assert.Equal(rawRaster.getHeight(), ximage.getHeight());
        if (colorSpaceName.Equals("ICCBased"))
        {
            var rawImage = ximage.getRawImage();
            Assert.NotNull(rawImage);
            Assert.Equal(rawImage.getWidth(), ximage.getWidth());
            Assert.Equal(rawImage.getHeight(), ximage.getHeight());
        }

        bool canEncode = true;
        bool writeOk;
        // jdk11+ no longer encodes ARGB jpg
        // https://bugs.openjdk.java.net/browse/JDK-8211748
        if ("jpg".Equals(format) &&
            ximage.getImage().getType() == java.awt.image.BufferedImage.TYPE_INT_ARGB)
        {
            var writers = javax.imageio.ImageIO.getImageWritersBySuffix(format);
            if (writers.hasNext())
            {
                var writer = (javax.imageio.ImageWriter)writers.next();
                var originatingProvider = writer.getOriginatingProvider();
                canEncode = originatingProvider.canEncodeImage(ximage.getImage());
            }
        }
        if (canEncode)
        {
            // Use ByteArrayOutputStream instead of nullOutputStream (Java 11 method)
            var nullOut = new java.io.ByteArrayOutputStream();
            writeOk = javax.imageio.ImageIO.write(ximage.getImage(), format, nullOut);
            Assert.True(writeOk);
        }
        var nullOut2 = new java.io.ByteArrayOutputStream();
        writeOk = javax.imageio.ImageIO.write(ximage.getOpaqueImage(null, 1), format, nullOut2);
        Assert.True(writeOk);
    }

    public static int colorCount(java.awt.image.BufferedImage bim)
    {
        var colors = new HashSet<int>();
        int w = bim.getWidth();
        int h = bim.getHeight();
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                colors.Add(bim.getRGB(x, y));
            }
        }
        return colors.Count;
    }

    // write image twice (overlapped) in document, close document and re-read PDF
    public static void doWritePDF(PDDocument document, PDImageXObject ximage, java.io.File testResultsDir, string filename)
    {
        var pdfFile = new java.io.File(testResultsDir, filename);

        // This part isn't really needed because this test doesn't break
        // if the mask has the wrong colorspace (PDFBOX-2057), but it is still useful
        // if something goes wrong in the future and we want to have a PDF to open.

        var page = new PDPage();
        document.addPage(page);
        using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(
            document, page,
            org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.APPEND,
            false))
        {
            contentStream.drawImage(ximage, 150, 300);
            contentStream.drawImage(ximage, 200, 350);
        }

        // check that the resource map is up-to-date
        Assert.Equal(1, count(document.getPage(0).getResources().getXObjectNames()));

        document.save(pdfFile);
        document.close();

        document = Loader.loadPDF(pdfFile);
        Assert.Equal(1, count(document.getPage(0).getResources().getXObjectNames()));
        new PDFRenderer(document).renderImage(0);
        document.close();
    }

    private static int count(java.lang.Iterable iterable)
    {
        int count = 0;
        var iterator = iterable.iterator();
        while (iterator.hasNext())
        {
            iterator.next();
            count++;
        }
        return count;
    }

    /// <summary>
    /// Check whether the images are identical.
    /// </summary>
    /// <param name="expectedImage">Expected image</param>
    /// <param name="actualImage">Actual image</param>
    public static void checkIdent(java.awt.image.BufferedImage expectedImage, java.awt.image.BufferedImage actualImage)
    {
        string errMsg = "";

        expectedImage = convertToSRGB(expectedImage);
        actualImage = convertToSRGB(actualImage);

        int w = expectedImage.getWidth();
        int h = expectedImage.getHeight();
        Assert.Equal(w, actualImage.getWidth());
        Assert.Equal(h, actualImage.getHeight());
        for (int y = 0; y < h; ++y)
        {
            for (int x = 0; x < w; ++x)
            {
                if (expectedImage.getRGB(x, y) != actualImage.getRGB(x, y))
                {
                    errMsg = string.Format("({0},{1}) expected: <{2:X8}> but was: <{3:X8}>; ",
                        x, y, expectedImage.getRGB(x, y), actualImage.getRGB(x, y));
                }
                Assert.Equal(expectedImage.getRGB(x, y), actualImage.getRGB(x, y));
            }
        }
    }

    public static java.awt.image.BufferedImage convertToSRGB(java.awt.image.BufferedImage image)
    {
        // The image is already sRGB - we don't need to do anything
        if (image.getColorModel().getColorSpace().isCS_sRGB())
        {
            return image;
        }

        // 16-Bit images need to converted to 8 bit first, to avoid rounding differences
        if (image.getRaster().getDataBuffer().getDataType() == java.awt.image.DataBuffer.TYPE_USHORT)
        {
            int width = image.getWidth();
            bool hasAlpha = image.getColorModel().hasAlpha();

            var colorModel = new java.awt.image.DirectColorModel(
                    image.getColorModel().getColorSpace(), 32, 0xFF, 0xFF00, 0xFF0000, unchecked((int)0xFF000000),
                    false, java.awt.image.DataBuffer.TYPE_INT);
            var targetRaster = java.awt.image.Raster
                    .createPackedRaster(java.awt.image.DataBuffer.TYPE_INT, image.getWidth(), image.getHeight(),
                            colorModel.getMasks(), new java.awt.Point(0, 0));

            var image8Bit = new java.awt.image.BufferedImage(colorModel, targetRaster, false,
                    new java.util.Hashtable());

            var sourceRaster = image.getRaster();

            int numShortPixelElements = hasAlpha ? 4 : 3;
            // 3 or 4 short per pixel
            short[] pixelShort = new short[numShortPixelElements * width];
            // Packed RGB
            int[] pixelInt = new int[width];
            for (int y = 0; y < image.getHeight(); y++)
            {
                sourceRaster.getDataElements(0, y, width, 1, pixelShort);
                int ptrShort = 0;
                for (int x = 0; x < width; x++)
                {
                    int r = pixelShort[ptrShort++] & 0xFFFF;
                    int g = pixelShort[ptrShort++] & 0xFFFF;
                    int b = pixelShort[ptrShort++] & 0xFFFF;
                    if (hasAlpha)
                        ptrShort++;

                    // We divide using a float exactly the same way as SampledImageReader
                    // to get from 16 bit to 8 bit sample values
                    int r8bit = convert16To8Bit(r);
                    int g8bit = convert16To8Bit(g);
                    int b8bit = convert16To8Bit(b);
                    int v = r8bit | (g8bit << 8) | (b8bit << 16) | unchecked((int)0xFF000000);
                    pixelInt[x] = v;
                }
                targetRaster.setDataElements(0, y, width, 1, pixelInt);
            }
            image = image8Bit;
        }

        var destination = new java.awt.image.BufferedImage(image.getWidth(), image.getHeight(),
                java.awt.image.BufferedImage.TYPE_INT_RGB);
        var op = new java.awt.image.ColorConvertOp(
            java.awt.color.ColorSpace.getInstance(java.awt.color.ColorSpace.CS_sRGB), null);
        return op.filter(image, destination);
    }

    private static int convert16To8Bit(int v)
    {
        float output = v / (float)0xFFFF;
        return (int)Math.Round(output * 0xFF);
    }
}
