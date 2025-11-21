/*
 * Converted from Apache PDFBox TestPDFToImage.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using System.IO;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.rendering;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test suite for rendering.
///
/// FILE SET VALIDATION
///
/// This test is designed to test PDFToImage using a set of PDF files and known good output for
/// each. The default mode is to process all *.pdf and *.ai files in
/// "src/test/resources/input/rendering". An output file is created in "target/test-output/rendering"
/// with the same name as the PDF file, plus an additional page number and ".png" suffix.
///
/// The output file is then tested against a known good result file from the input directory (again,
/// with the same name as the tested PDF file, but with the additional page number and ".png"
/// suffix).
///
/// If the two aren't identical, a graphical .diff.png file is created. If they are identical, the
/// output .png file is deleted. If a "good result" file doesn't exist, the output .png file is left
/// there for human inspection.
///
/// Errors are flagged by creating empty files with appropriate names in the target directory.
///
/// Converted from org.apache.pdfbox.rendering.TestPDFToImage
/// @author Daniel Wilson
/// @author Ben Litchfield
/// @author Tilman Hausherr
/// </summary>
public class TestPDFToImage
{
    // private static readonly org.apache.logging.log4j.Logger LOG =
    //     org.apache.logging.log4j.LogManager.getLogger(typeof(TestPDFToImage));

    /// <summary>
    /// Constructor.
    /// </summary>
    public TestPDFToImage()
    {
    }

    /// <summary>
    /// Create an image; the part between the smaller and the larger image is painted black, the rest
    /// in white
    /// </summary>
    /// <param name="minWidth">width of the smaller image</param>
    /// <param name="minHeight">width of the smaller image</param>
    /// <param name="maxWidth">height of the larger image</param>
    /// <param name="maxHeight">height of the larger image</param>
    /// <returns>The created empty diff image</returns>
    private static java.awt.image.BufferedImage createEmptyDiffImage(int minWidth, int minHeight, int maxWidth,
            int maxHeight)
    {
        var bim3 = new java.awt.image.BufferedImage(maxWidth, maxHeight, java.awt.image.BufferedImage.TYPE_INT_RGB);
        var graphics = bim3.getGraphics();
        if (minWidth != maxWidth || minHeight != maxHeight)
        {
            graphics.setColor(java.awt.Color.BLACK);
            graphics.fillRect(0, 0, maxWidth, maxHeight);
        }
        graphics.setColor(java.awt.Color.WHITE);
        graphics.fillRect(0, 0, minWidth, minHeight);
        graphics.dispose();
        return bim3;
    }

    /// <summary>
    /// Get the difference between two images, identical colors are set to white, differences are
    /// xored, the highest bit of each color is reset to avoid colors that are too light.
    /// </summary>
    /// <param name="bim1">First image</param>
    /// <param name="bim2">Second image</param>
    /// <returns>If the images are different, the function returns a diff image. If the images are
    /// identical, the function returns null. If the size is different, a black border on the bottom
    /// at the right is created.</returns>
    private static java.awt.image.BufferedImage diffImages(java.awt.image.BufferedImage bim1, java.awt.image.BufferedImage bim2)
    {
        int minWidth = Math.Min(bim1.getWidth(), bim2.getWidth());
        int minHeight = Math.Min(bim1.getHeight(), bim2.getHeight());
        int maxWidth = Math.Max(bim1.getWidth(), bim2.getWidth());
        int maxHeight = Math.Max(bim1.getHeight(), bim2.getHeight());
        java.awt.image.BufferedImage bim3 = null;
        if (minWidth != maxWidth || minHeight != maxHeight)
        {
            bim3 = createEmptyDiffImage(minWidth, minHeight, maxWidth, maxHeight);
        }
        for (int x = 0; x < minWidth; ++x)
        {
            for (int y = 0; y < minHeight; ++y)
            {
                int rgb1 = bim1.getRGB(x, y);
                int rgb2 = bim2.getRGB(x, y);
                if (rgb1 != rgb2
                        // don't bother about small differences
                        && (Math.Abs((rgb1 & 0xFF) - (rgb2 & 0xFF)) > 3
                        || Math.Abs(((rgb1 >> 8) & 0xFF) - ((rgb2 >> 8) & 0xFF)) > 3
                        || Math.Abs(((rgb1 >> 16) & 0xFF) - ((rgb2 >> 16) & 0xFF)) > 3))
                {
                    if (bim3 == null)
                    {
                        bim3 = createEmptyDiffImage(minWidth, minHeight, maxWidth, maxHeight);
                    }
                    int r = Math.Abs((rgb1 & 0xFF) - (rgb2 & 0xFF));
                    int g = Math.Abs((rgb1 & 0xFF00) - (rgb2 & 0xFF00));
                    int b = Math.Abs((rgb1 & 0xFF0000) - (rgb2 & 0xFF0000));
                    bim3.setRGB(x, y, 0xFFFFFF - (r | g | b));
                }
                else
                {
                    if (bim3 != null)
                    {
                        bim3.setRGB(x, y, java.awt.Color.WHITE.getRGB());
                    }
                }
            }
        }
        return bim3;
    }

    /// <summary>
    /// Validate the renderings of a single file.
    /// </summary>
    /// <param name="file">The file to validate</param>
    /// <param name="inDir">Name of the input directory</param>
    /// <param name="outDir">Name of the output directory</param>
    /// <returns>false if the test failed (not identical or other problem), true if the test succeeded
    /// (all identical)</returns>
    public static bool doTestFile(java.io.File file, string inDir, string outDir)
    {
        PDDocument document = null;
        bool failed = false;

        // LOG.info("Opening: {}", file.getName());
        try
        {
            new java.io.FileOutputStream(new java.io.File(outDir, file.getName() + ".parseerror")).close();
            document = Loader.loadPDF(file, (string)null);
            int numPages = document.getNumberOfPages();
            if (numPages < 1)
            {
                failed = true;
                // LOG.error("file {} has < 1 page", file.getName());
            }
            else
            {
                new java.io.File(outDir, file.getName() + ".parseerror").delete();
                new java.io.File(outDir, file.getName() + ".parseerror").deleteOnExit();
            }

            // LOG.info("Rendering: {}", file.getName());
            var renderer = new PDFRenderer(document);
            for (int i = 0; i < numPages; i++)
            {
                string fileName = file.getName() + "-" + (i + 1) + ".png";
                new java.io.FileOutputStream(new java.io.File(outDir, fileName + ".rendererror")).close();
                var image = renderer.renderImageWithDPI(i, 96); // Windows native DPI
                new java.io.File(outDir, fileName + ".rendererror").delete();
                new java.io.File(outDir, fileName + ".rendererror").deleteOnExit();
                // LOG.info("Writing: {}", fileName);
                new java.io.FileOutputStream(new java.io.File(outDir, fileName + ".writeerror")).close();
                bool writeSuccess = javax.imageio.ImageIO.write(image, "PNG", new java.io.File(outDir, fileName));
                if (writeSuccess)
                {
                    new java.io.File(outDir, fileName + ".writeerror").delete();
                    new java.io.File(outDir, fileName + ".writeerror").deleteOnExit();
                }
            }

            // test to see whether file is destroyed in pdfbox
            new java.io.FileOutputStream(new java.io.File(outDir, file.getName() + ".saveerror")).close();
            var tmpFile = java.nio.file.Files.createTempFile("pdfbox", ".pdf").toFile();
            document.setAllSecurityToBeRemoved(true);
            document.save(tmpFile);
            new java.io.File(outDir, file.getName() + ".saveerror").delete();
            new java.io.File(outDir, file.getName() + ".saveerror").deleteOnExit();
            new java.io.FileOutputStream(new java.io.File(outDir, file.getName() + ".reloaderror")).close();
            Loader.loadPDF(tmpFile).close();
            new java.io.File(outDir, file.getName() + ".reloaderror").delete();
            new java.io.File(outDir, file.getName() + ".reloaderror").deleteOnExit();
            tmpFile.delete();
            tmpFile.deleteOnExit();
        }
        catch (java.io.IOException e)
        {
            failed = true;
            // LOG.error("Error converting file {}", file.getName());
            throw e;
        }
        finally
        {
            if (document != null)
            {
                document.close();
            }
        }

        // LOG.info("Comparing: {}", file.getName());

        //Now check the resulting files ... did we get identical PNG(s)?
        try
        {
            new java.io.File(outDir, file.getName() + ".cmperror").delete();

            var outFiles = new java.io.File(outDir).listFiles(new FilenameFilterImpl(file.getName()));
            if (outFiles.Length == 0)
            {
                failed = true;
                // LOG.warn("*** TEST FAILURE *** Output missing for file: {}", file.getName());
            }
            foreach (var outFile in outFiles)
            {
                new java.io.File(outFile.getAbsolutePath() + "-diff.png").delete(); // delete diff file from a previous run
                var inFile = new java.io.File(inDir + '/' + outFile.getName());
                if (!inFile.exists())
                {
                    failed = true;
                    // LOG.warn("*** TEST FAILURE *** Input missing for file: {}", inFile.getName());
                }
                else if (!filesAreIdentical(outFile, inFile))
                {
                    // different files might still have identical content
                    // save the difference (if any) into a diff image
                    var bim3 = diffImages(javax.imageio.ImageIO.read(inFile), javax.imageio.ImageIO.read(outFile));
                    if (bim3 != null)
                    {
                        failed = true;
                        // LOG.warn("*** TEST FAILURE *** Input and output not identical for file: {}", inFile.getName());
                        javax.imageio.ImageIO.write(bim3, "png", new java.io.File(outFile.getAbsolutePath() + "-diff.png"));
                        Console.Error.WriteLine("Files differ: " + inFile.getAbsolutePath() + "\n" +
                                           "              " + outFile.getAbsolutePath());
                    }
                    else
                    {
                        // LOG.info("*** TEST OK *** for file: {}", inFile.getName());
                        // LOG.info("Deleting: {}", outFile.getName());
                        outFile.delete();
                        outFile.deleteOnExit();
                    }
                }
                else
                {
                    // LOG.info("*** TEST OK *** for file: {}", inFile.getName());
                    // LOG.info("Deleting: {}", outFile.getName());
                    outFile.delete();
                    outFile.deleteOnExit();
                }
            }
        }
        catch (Exception e)
        {
            new java.io.FileOutputStream(new java.io.File(outDir, file.getName() + ".cmperror")).close();
            failed = true;
            // LOG.error(() => "Error comparing file output for " + file.getName(), e);
        }

        return !failed;
    }

    private static bool filesAreIdentical(java.io.File left, java.io.File right)
    {
        //http://forum.java.sun.com/thread.jspa?threadID=688105&messageID=4003259
        //http://web.archive.org/web/20060515173719/http://forum.java.sun.com/thread.jspa?threadID=688105&messageID=4003259

        /* -- I reworked ASSERT's into IF statement -- dwilson
         assert left != null;
         assert right != null;
         assert left.exists();
         assert right.exists();
         */
        if (left != null && right != null && left.exists() && right.exists())
        {
            if (left.length() != right.length())
            {
                return false;
            }

            java.io.FileInputStream lin = null;
            java.io.FileInputStream rin = null;
            try
            {
                lin = new java.io.FileInputStream(left);
                rin = new java.io.FileInputStream(right);
                byte[] lbuffer = new byte[4096];
                byte[] rbuffer = new byte[lbuffer.Length];
                int lcount;
                while ((lcount = lin.read(lbuffer)) > 0)
                {
                    int bytesRead = 0;
                    int rcount;
                    while ((rcount = rin.read(rbuffer, bytesRead, lcount - bytesRead)) > 0)
                    {
                        bytesRead += rcount;
                    }
                    for (int byteIndex = 0; byteIndex < lcount; byteIndex++)
                    {
                        if (lbuffer[byteIndex] != rbuffer[byteIndex])
                        {
                            return false;
                        }
                    }
                }
            }
            catch (java.io.IOException)
            {
                return false;
            }
            finally
            {
                if (lin != null)
                {
                    try { lin.close(); } catch { }
                }
                if (rin != null)
                {
                    try { rin.close(); } catch { }
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// FilenameFilter implementation for filtering PNG files.
    /// </summary>
    private class FilenameFilterImpl : java.io.FilenameFilter
    {
        private readonly string fileName;

        public FilenameFilterImpl(string fileName)
        {
            this.fileName = fileName;
        }

        public bool accept(java.io.File dir, string name)
        {
            return (name.EndsWith(".png")
                    && name.StartsWith(fileName))
                    && !name.EndsWith(".png-diff.png");
        }
    }
}
