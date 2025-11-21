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
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.multipdf;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.graphics.optionalcontent;
using org.apache.pdfbox.pdfwriter.compress;
using Xunit;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Tests the LayerUtility class.
    /// </summary>
    public class TestLayerUtility
    {
        private static readonly string TESTRESULTSDIR = "target/test-output";

        public TestLayerUtility()
        {
            Directory.CreateDirectory(TESTRESULTSDIR);
        }

        /// <summary>
        /// Tests layer import.
        /// </summary>
        [Fact]
        public void TestLayerImport()
        {
            var mainPDF = CreateMainPDF();
            var overlay1 = CreateOverlay1();
            var targetFile = new java.io.File(TESTRESULTSDIR, "text-with-form-overlay.pdf");

            using (var targetDoc = Loader.loadPDF(mainPDF))
            using (var overlay1Doc = Loader.loadPDF(overlay1))
            {
                Assert.Equal(1.4f, targetDoc.getVersion());

                var layerUtil = new LayerUtility(targetDoc);
                var form = layerUtil.importPageAsForm(overlay1Doc, 0);
                var targetPage = targetDoc.getPage(0);
                layerUtil.wrapInSaveRestore(targetPage);

                var at = new java.awt.geom.AffineTransform();
                layerUtil.appendFormAsLayer(targetPage, form, at, "overlay");

                Assert.Equal(1.5f, targetDoc.getVersion());

                // Save with no compression to avoid version going up to 1.6
                targetDoc.save(targetFile.getAbsolutePath(), CompressParameters.NO_COMPRESSION);
                Assert.Equal(1.5f, targetDoc.getVersion());
            }

            using (var doc = Loader.loadPDF(targetFile))
            {
                var catalog = doc.getDocumentCatalog();

                // OCGs require PDF 1.5 or later
                Assert.Equal(1.5f, doc.getVersion());

                var page = doc.getPage(0);
                var ocg = (PDOptionalContentGroup)page.getResources()
                    .getProperties(COSName.getPDFName("oc1"));

                Assert.NotNull(ocg);
                Assert.Equal("overlay", ocg.getName());

                var ocgs = catalog.getOCProperties();
                var overlay = ocgs.getGroup("overlay");
                Assert.Equal(ocg.getName(), overlay.getName());

                // Test PDFBOX-5232 (never ended)
                new LayerUtility(doc).importPageAsForm(doc, 0);
            }
        }

        private java.io.File CreateMainPDF()
        {
            var targetFile = new java.io.File(TESTRESULTSDIR, "text-doc.pdf");

            using (var doc = new PDDocument())
            {
                // Create new page
                var page = new PDPage();
                doc.addPage(page);

                var resources = page.getResources();
                if (resources == null)
                {
                    resources = new org.apache.pdfbox.pdmodel.PDResources();
                    page.setResources(resources);
                }

                string[] text = {
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer fermentum lacus in eros",
                    "condimentum eget tristique risus viverra. Sed ac sem et lectus ultrices placerat. Nam",
                    "fringilla tincidunt nulla id euismod. Vivamus eget mauris dui. Mauris luctus ullamcorper",
                    "leo, et laoreet diam suscipit et. Nulla viverra commodo sagittis. Integer vitae rhoncus velit.",
                    "Mauris porttitor ipsum in est sagittis non luctus purus molestie. Sed placerat aliquet",
                    "vulputate."
                };

                using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(
                    doc, page,
                    org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.OVERWRITE,
                    false))
                {
                    // Setup page content stream and paint background/title
                    var font = new PDType1Font(org.apache.pdfbox.pdmodel.font.Standard14Fonts.FontName.HELVETICA_BOLD);
                    contentStream.beginText();
                    contentStream.newLineAtOffset(50, 720);
                    contentStream.setFont(font, 14);
                    contentStream.showText("Simple test document with text.");
                    contentStream.endText();

                    font = new PDType1Font(org.apache.pdfbox.pdmodel.font.Standard14Fonts.FontName.HELVETICA);
                    contentStream.beginText();
                    int fontSize = 12;
                    contentStream.setFont(font, fontSize);
                    contentStream.newLineAtOffset(50, 700);

                    foreach (string line in text)
                    {
                        contentStream.newLineAtOffset(0, -fontSize * 1.2f);
                        contentStream.showText(line);
                    }
                    contentStream.endText();
                }

                // Save with no compression to avoid version going up to 1.6
                doc.save(targetFile.getAbsolutePath(), CompressParameters.NO_COMPRESSION);
            }

            return targetFile;
        }

        private java.io.File CreateOverlay1()
        {
            var targetFile = new java.io.File(TESTRESULTSDIR, "overlay1.pdf");

            using (var doc = new PDDocument())
            {
                // Create new page
                var page = new PDPage();
                doc.addPage(page);

                var resources = page.getResources();
                if (resources == null)
                {
                    resources = new org.apache.pdfbox.pdmodel.PDResources();
                    page.setResources(resources);
                }

                using (var contentStream = new org.apache.pdfbox.pdmodel.PDPageContentStream(
                    doc, page,
                    org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.OVERWRITE,
                    false))
                {
                    // Setup page content stream and paint background/title
                    var font = new PDType1Font(org.apache.pdfbox.pdmodel.font.Standard14Fonts.FontName.HELVETICA_BOLD);
                    contentStream.setNonStrokingColor(java.awt.Color.LIGHT_GRAY);
                    contentStream.beginText();

                    float fontSize = 96;
                    contentStream.setFont(font, fontSize);
                    string text = "OVERLAY";

                    var crop = page.getCropBox();
                    float cx = crop.getWidth() / 2f;
                    float cy = crop.getHeight() / 2f;

                    var transform = new org.apache.pdfbox.util.Matrix();
                    transform.translate(cx, cy);
                    transform.rotate(45 * System.Math.PI / 180);
                    transform.translate(-190, 0);

                    contentStream.setTextMatrix(transform);
                    contentStream.showText(text);
                    contentStream.endText();
                }

                doc.save(targetFile.getAbsolutePath());
            }

            return targetFile;
        }
    }
}
