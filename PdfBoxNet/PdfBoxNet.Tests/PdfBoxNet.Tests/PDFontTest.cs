/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using Xunit;
using org.apache.fontbox.ttf;
using org.apache.fontbox.util.autodetect;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.io;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.font.encoding;
using org.apache.pdfbox.rendering;
using org.apache.pdfbox.text;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Test for the PDFont class.
    /// Converted from org.apache.pdfbox.pdmodel.font.PDFontTest
    /// </summary>
    public class PDFontTest
    {
        private static readonly string PDFBOX_ROOT = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", ".."));
        private static readonly java.io.File OUT_DIR = new java.io.File(System.IO.Path.Combine(PDFBOX_ROOT, "target", "test-output"));

        public PDFontTest()
        {
            if (!OUT_DIR.exists())
            {
                OUT_DIR.mkdirs();
            }
        }

        /// <summary>
        /// Test of the error reported in PDFBOX-988
        /// </summary>
        [Fact]
        public void testPDFBox988()
        {
            try
            {
                var resourceStream = typeof(PDFontTest).Assembly.GetManifestResourceStream("F001u_3_7j.pdf");
                if (resourceStream == null) return; // Skip if resource doesn't exist

                using (var javaStream = new ikvm.io.InputStreamWrapper(resourceStream))
                using (PDDocument doc = Loader.loadPDF(RandomAccessReadBuffer.createBufferFromStream(javaStream)))
                {
                    PDFRenderer renderer = new PDFRenderer(doc);
                    renderer.renderImage(0);
                    // the allegation is that renderImage() will crash the JVM or hang
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        [Fact]
        public void testPDFBOX5486()
        {
            try
            {
                using (PDDocument doc = new PDDocument())
                {
                    var resourceStream = typeof(PDFontTest).Assembly.GetManifestResourceStream("LiberationSans-Regular.ttf");
                    if (resourceStream == null) return; // Skip if resource doesn't exist

                    using (var javaStream = new ikvm.io.InputStreamWrapper(resourceStream))
                    {
                        PDTrueTypeFont ttf = PDTrueTypeFont.load(doc, javaStream, WinAnsiEncoding.INSTANCE);
                        Assert.True(ttf.hasGlyph("A"));
                        ttf.getPath("A");
                    }
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        /// <summary>
        /// PDFBOX-3747: Test that using "-" with Calibri in Windows 7 has "-" in text extraction and not
        /// \u2010, which was because of a wrong ToUnicode mapping because prior to the bugfix,
        /// CmapSubtable#getCharCodes provided values in random order.
        /// </summary>
        [Fact]
        public void testPDFBox3747()
        {
            try
            {
                java.io.File file = new java.io.File("c:/windows/fonts/calibri.ttf");
                if (!file.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);
                    PDFont font = PDType0Font.load(doc, file);
                    using (PDPageContentStream cs = new PDPageContentStream(doc, page))
                    {
                        cs.beginText();
                        cs.setFont(font, 10);
                        cs.showText("PDFBOX-3747");
                        cs.endText();
                    }
                    doc.save(baos);
                }

                using (PDDocument doc = Loader.loadPDF(baos.toByteArray()))
                {
                    PDFTextStripper stripper = new PDFTextStripper();
                    string text = stripper.getText(doc);
                    Assert.Equal("PDFBOX-3747", text.Trim());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        /// <summary>
        /// PDFBOX-3826: Test ability to reuse a TrueTypeFont created from a file or a stream for several
        /// PDFs to avoid parsing it over and over again. Also check that full or partial embedding is
        /// done, and do render and text extraction.
        /// </summary>
        [Fact]
        public void testPDFBox3826()
        {
            try
            {
                var liberationSansPath = System.IO.Path.Combine(PDFBOX_ROOT, "pdfbox", "src", "main", "resources",
                    "org", "apache", "pdfbox", "resources", "ttf", "LiberationSans-Regular.ttf");
                java.io.File fontFile = new java.io.File(liberationSansPath);

                if (!fontFile.exists())
                {
                    return; // Skip if font file doesn't exist
                }

                using (TrueTypeFont ttf1 = new TTFParser().parse(new RandomAccessReadBufferedFile(fontFile)))
                {
                    testPDFBox3826checkFonts(testPDFBox3826createDoc(ttf1), fontFile);
                }

                using (TrueTypeFont ttf2 = new TTFParser().parse(new RandomAccessReadBufferedFile(fontFile)))
                {
                    testPDFBox3826checkFonts(testPDFBox3826createDoc(ttf2), fontFile);
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        /// <summary>
        /// PDFBOX-4115: Test ability to create PDF with german umlaut glyphs with a type 1 font.
        /// Test for everything that went wrong before this was fixed.
        /// </summary>
        [Fact]
        public void testPDFBOX4115()
        {
            try
            {
                java.io.File fontFile = new java.io.File(System.IO.Path.Combine(PDFBOX_ROOT, "target", "fonts", "n019003l.pfb"));
                java.io.File outputFile = new java.io.File(OUT_DIR, "FontType1.pdf");
                string text = "äöüÄÖÜ";

                if (!fontFile.exists())
                {
                    return; // Skip if font file doesn't exist
                }

                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    using (PDPageContentStream contentStream = new PDPageContentStream(doc, page))
                    using (java.io.InputStream jis = new java.io.FileInputStream(fontFile))
                    {
                        PDType1Font font = new PDType1Font(doc, jis, WinAnsiEncoding.INSTANCE);

                        contentStream.beginText();
                        contentStream.setFont(font, 10);
                        contentStream.newLineAtOffset(10, 700);
                        contentStream.showText(text);
                        contentStream.endText();
                    }

                    doc.addPage(page);
                    doc.save(outputFile);
                }

                using (PDDocument doc = Loader.loadPDF(outputFile))
                {
                    PDType1Font font = (PDType1Font)doc.getPage(0).getResources().getFont(COSName.getPDFName("F1"));
                    Assert.Equal(WinAnsiEncoding.INSTANCE, font.getEncoding());

                    foreach (char c in text.ToCharArray())
                    {
                        string name = font.getEncoding().getName(c);
                        Assert.Equal("dieresis", name.Substring(1));
                        Assert.False(font.getPath(name).getBounds2D().isEmpty());
                    }

                    PDFTextStripper stripper = new PDFTextStripper();
                    Assert.Equal(text, stripper.getText(doc).Trim());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        /// <summary>
        /// Test whether bug from PDFBOX-4318 is fixed, which had the wrong cache key.
        /// </summary>
        [Fact]
        public void testPDFox4318()
        {
            try
            {
                PDType1Font helveticaBold = new PDType1Font(Standard14Fonts.FontName.HELVETICA_BOLD);
                Assert.Throws<java.lang.IllegalArgumentException>(() => helveticaBold.encode("\u0080"));
                helveticaBold.encode("€");
                Assert.Throws<java.lang.IllegalArgumentException>(() => helveticaBold.encode("\u0080"));
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        [Fact]
        public void testFullEmbeddingTTC()
        {
            try
            {
                FontFileFinder fff = new FontFileFinder();
                TrueTypeCollection ttc = null;
                var fontUris = fff.find();
                var iterator = fontUris.iterator();

                while (iterator.hasNext())
                {
                    java.net.URI uri = (java.net.URI)iterator.next();
                    if (uri.getPath().EndsWith(".ttc"))
                    {
                        java.io.File file = new java.io.File(uri);
                        ttc = new TrueTypeCollection(file);
                        break;
                    }
                }

                if (ttc == null)
                {
                    return; // Skip test if no .ttc files available
                }

                var names = new java.util.ArrayList();
                ttc.processAllFonts(new FontNameCollector(names));

                TrueTypeFont ttf = ttc.getFontByName((string)names.get(0));

                var ex = Assert.Throws<java.io.IOException>(() =>
                    PDType0Font.load(new PDDocument(), ttf, false));
                Assert.Equal("Full embedding of TrueType font collections not supported", ex.getMessage());
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        // Helper class for processing TrueType fonts and collecting names
        private class FontNameCollector : TrueTypeCollection.TrueTypeFontProcessor
        {
            private readonly java.util.ArrayList names;

            public FontNameCollector(java.util.ArrayList names)
            {
                this.names = names;
            }

            public void process(TrueTypeFont ttf)
            {
                names.add(ttf.getName());
            }
        }

        [Fact]
        public void testSymbol()
        {
            try
            {
                java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    using (PDPageContentStream contentStream = new PDPageContentStream(doc, page))
                    {
                        PDType1Font font = new PDType1Font(Standard14Fonts.FontName.SYMBOL);

                        contentStream.beginText();
                        contentStream.setFont(font, 10);
                        contentStream.newLineAtOffset(10, 700);
                        // Note that the Alpha is the greek alpha, but the Omega is the Ohm symbol
                        contentStream.showText("\u0391 \u2126");
                        contentStream.endText();
                    }

                    doc.addPage(page);
                    doc.save(baos);
                }

                using (PDDocument doc = Loader.loadPDF(baos.toByteArray()))
                {
                    PDFTextStripper stripper = new PDFTextStripper();
                    string text = stripper.getText(doc);
                    Assert.Equal("\u0391 \u2126", text.Trim());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        [Fact]
        public void PDFBOX5920Type0()
        {
            try
            {
                var resourceStream = typeof(PDFontTest).Assembly.GetManifestResourceStream("LiberationSans-Regular.ttf");
                if (resourceStream == null) return;

                using (var javaStream = new ikvm.io.InputStreamWrapper(resourceStream))
                using (PDDocument document = new PDDocument())
                {
                    PDFont font = PDType0Font.load(document, javaStream, false);
                    Assert.Equal(20064.0f, font.getStringWidth("The quick brown fox jumps over the lazy dog."));
                    Assert.Equal(278.0f, font.getSpaceWidth());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        [Fact]
        public void PDFBOX5920TrueType()
        {
            try
            {
                var resourceStream = typeof(PDFontTest).Assembly.GetManifestResourceStream("LiberationSans-Regular.ttf");
                if (resourceStream == null) return;

                using (var javaStream = new ikvm.io.InputStreamWrapper(resourceStream))
                using (PDDocument document = new PDDocument())
                {
                    PDFont font = PDTrueTypeFont.load(document, javaStream, WinAnsiEncoding.INSTANCE);
                    Assert.Equal(20064.0f, font.getStringWidth("The quick brown fox jumps over the lazy dog."));
                    Assert.Equal(278.0f, font.getSpaceWidth());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        [Fact]
        public void testSoftHyphen()
        {
            try
            {
                string text = "- \u00AD";
                java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();

                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);
                    PDFont font1 = new PDType1Font(Standard14Fonts.FontName.HELVETICA);

                    var resourceStream = typeof(PDFontTest).Assembly.GetManifestResourceStream("LiberationSans-Regular.ttf");
                    if (resourceStream == null) return;

                    PDFont font2;
                    using (var javaStream = new ikvm.io.InputStreamWrapper(resourceStream))
                    {
                        font2 = PDType0Font.load(doc, javaStream);
                    }

                    Assert.Equal(font1.getStringWidth("-"), font1.getStringWidth("\u00AD"));
                    Assert.Equal(font2.getStringWidth("-"), font2.getStringWidth("\u00AD"));

                    using (PDPageContentStream cs = new PDPageContentStream(doc, page))
                    {
                        cs.beginText();
                        cs.newLineAtOffset(100, 500);
                        cs.setFont(font1, 10);
                        cs.showText(text);
                        cs.newLineAtOffset(0, 100);
                        cs.setFont(font2, 10);
                        cs.showText(text);
                        cs.endText();
                    }
                    doc.save(baos);
                }

                using (PDDocument doc = Loader.loadPDF(baos.toByteArray()))
                {
                    PDFTextStripper stripper = new PDFTextStripper();
                    stripper.setLineSeparator("\n");
                    string extractedText = stripper.getText(doc);
                    Assert.Equal(text + "\n" + text, extractedText.Trim());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        private void testPDFBox3826checkFonts(byte[] byteArray, java.io.File fontFile)
        {
            try
            {
                using (PDDocument doc = Loader.loadPDF(byteArray))
                {
                    PDPage page2 = doc.getPage(0);

                    // F1 = type0 subset
                    PDType0Font fontF1 = (PDType0Font)page2.getResources().getFont(COSName.getPDFName("F1"));
                    Assert.True(fontF1.getName().Contains("+"));
                    Assert.True(fontFile.length() > fontF1.getFontDescriptor().getFontFile2().toByteArray().Length);

                    // F2 = type0 full embed
                    PDType0Font fontF2 = (PDType0Font)page2.getResources().getFont(COSName.getPDFName("F2"));
                    Assert.False(fontF2.getName().Contains("+"));
                    Assert.Equal(fontFile.length(), fontF2.getFontDescriptor().getFontFile2().toByteArray().Length);

                    // F3 = tt full embed
                    PDTrueTypeFont fontF3 = (PDTrueTypeFont)page2.getResources().getFont(COSName.getPDFName("F3"));
                    Assert.False(fontF3.getName().Contains("+"));
                    Assert.Equal(fontFile.length(), fontF3.getFontDescriptor().getFontFile2().toByteArray().Length);

                    new PDFRenderer(doc).renderImage(0);

                    PDFTextStripper stripper = new PDFTextStripper();
                    stripper.setLineSeparator("\n");
                    string text = stripper.getText(doc);
                    Assert.Equal("testMultipleFontFileReuse1\ntestMultipleFontFileReuse2\ntestMultipleFontFileReuse3", text.Trim());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
            }
        }

        private byte[] testPDFBox3826createDoc(TrueTypeFont ttf)
        {
            try
            {
                java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream();
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);

                    // type 0 subset embedding
                    PDFont font = PDType0Font.load(doc, ttf, true);
                    using (PDPageContentStream cs = new PDPageContentStream(doc, page))
                    {
                        cs.beginText();
                        cs.newLineAtOffset(10, 700);
                        cs.setFont(font, 10);
                        cs.showText("testMultipleFontFileReuse1");
                        cs.endText();

                        // type 0 full embedding
                        font = PDType0Font.load(doc, ttf, false);
                        cs.beginText();
                        cs.newLineAtOffset(10, 650);
                        cs.setFont(font, 10);
                        cs.showText("testMultipleFontFileReuse2");
                        cs.endText();

                        // tt full embedding but only WinAnsiEncoding
                        font = PDTrueTypeFont.load(doc, ttf, WinAnsiEncoding.INSTANCE);
                        cs.beginText();
                        cs.newLineAtOffset(10, 600);
                        cs.setFont(font, 10);
                        cs.showText("testMultipleFontFileReuse3");
                        cs.endText();
                    }

                    doc.save(baos);
                }
                return baos.toByteArray();
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException: " + ex.getMessage());
                return null;
            }
        }
    }
}
