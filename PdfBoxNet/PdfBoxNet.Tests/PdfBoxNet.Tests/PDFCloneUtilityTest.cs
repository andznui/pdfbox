/*
 * Copyright 2014 The Apache Software Foundation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
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
using org.apache.pdfbox.pdmodel.common;
using org.apache.pdfbox.pdmodel.graphics.optionalcontent;
using Xunit;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Test suite for PDFCloneUtility, see PDFBOX-2052.
    /// </summary>
    public class PDFCloneUtilityTest
    {
        /// <summary>
        /// Original (minimal) test from PDFBOX-2052.
        /// </summary>
        [Fact]
        public void TestClonePDFWithCosArrayStream()
        {
            using (var srcDoc = new PDDocument())
            using (var dstDoc = new PDDocument())
            {
                var pdPage = new PDPage();
                srcDoc.addPage(pdPage);

                new org.apache.pdfbox.pdmodel.PDPageContentStream(
                    srcDoc, pdPage,
                    org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.APPEND,
                    true).close();

                new org.apache.pdfbox.pdmodel.PDPageContentStream(
                    srcDoc, pdPage,
                    org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.APPEND,
                    true).close();

                // PDFCloneUtility is now internal in PDFBox 3.x, test via PDFMergerUtility instead
                var merger = new PDFMergerUtility();
                merger.appendDocument(dstDoc, srcDoc);

                var clonedPage = dstDoc.getPage(0);

                var contentStreams = clonedPage.getContentStreams();
                Assert.True(contentStreams.hasNext());
                Assert.NotNull(contentStreams.next());
                Assert.True(contentStreams.hasNext());
                Assert.NotNull(contentStreams.next());
                Assert.False(contentStreams.hasNext());
            }
        }

        /// <summary>
        /// Broader test that saves to a real PDF document.
        /// </summary>
        [Fact]
        public void TestClonePDFWithCosArrayStream2()
        {
            const string TESTDIR = "target/test-output/clone/";
            const string CLONESRC = "clone-src.pdf";
            const string CLONEDST = "clone-dst.pdf";

            Directory.CreateDirectory(TESTDIR);

            var srcDoc = new PDDocument();
            var pdPage = new PDPage();
            srcDoc.addPage(pdPage);

            // Create first content stream
            using (var pdPageContentStream1 = new org.apache.pdfbox.pdmodel.PDPageContentStream(
                srcDoc, pdPage,
                org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.APPEND,
                false))
            {
                pdPageContentStream1.setNonStrokingColor(java.awt.Color.black);
                pdPageContentStream1.addRect(100, 600, 300, 100);
                pdPageContentStream1.fill();
            }

            // Create second content stream
            using (var pdPageContentStream2 = new org.apache.pdfbox.pdmodel.PDPageContentStream(
                srcDoc, pdPage,
                org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.APPEND,
                false))
            {
                pdPageContentStream2.setNonStrokingColor(java.awt.Color.red);
                pdPageContentStream2.addRect(100, 500, 300, 100);
                pdPageContentStream2.fill();
            }

            // Create third content stream
            using (var pdPageContentStream3 = new org.apache.pdfbox.pdmodel.PDPageContentStream(
                srcDoc, pdPage,
                org.apache.pdfbox.pdmodel.PDPageContentStream.AppendMode.APPEND,
                false))
            {
                pdPageContentStream3.setNonStrokingColor(java.awt.Color.yellow);
                pdPageContentStream3.addRect(100, 400, 300, 100);
                pdPageContentStream3.fill();
            }

            srcDoc.save(TESTDIR + CLONESRC);

            var merger = new PDFMergerUtility();
            using (var dstDoc = new PDDocument())
            {
                // This calls PDFCloneUtility.cloneForNewDocument(),
                // which would fail before the fix in PDFBOX-2052
                merger.appendDocument(dstDoc, srcDoc);

                // Save and reload PDF, so that one can see that the files are legit
                dstDoc.save(TESTDIR + CLONEDST);
            }

            srcDoc.close();

            // Verify source document
            using (var doc = Loader.loadPDF(new java.io.File(TESTDIR + CLONESRC)))
            {
                Assert.Equal(1, doc.getNumberOfPages());
            }

            using (var doc = Loader.loadPDF(new java.io.File(TESTDIR + CLONESRC), (string)null))
            {
                Assert.Equal(1, doc.getNumberOfPages());
            }

            // Verify destination document
            using (var doc = Loader.loadPDF(new java.io.File(TESTDIR + CLONEDST)))
            {
                Assert.Equal(1, doc.getNumberOfPages());
            }

            using (var doc = Loader.loadPDF(new java.io.File(TESTDIR + CLONEDST), (string)null))
            {
                Assert.Equal(1, doc.getNumberOfPages());
            }
        }

        /// <summary>
        /// PDFBOX-4814: this tests merging a direct and an indirect COSDictionary,
        /// when "target" is indirect in cloneMerge().
        /// </summary>
        [Fact]
        public void TestDirectIndirect()
        {
            using (var doc1 = new PDDocument())
            {
                doc1.addPage(new PDPage());
                doc1.getDocumentCatalog().setOCProperties(new PDOptionalContentProperties());

                var baos = new java.io.ByteArrayOutputStream();
                doc1.save(baos);

                using (var doc2 = Loader.loadPDF(baos.toByteArray()))
                {
                    var merger = new PDFMergerUtility();

                    // The OCProperties is a direct object here, but gets saved as an indirect object.
                    var ocProps1 = doc1.getDocumentCatalog().getCOSObject().getItem(COSName.OCPROPERTIES);
                    var ocProps2 = doc2.getDocumentCatalog().getCOSObject().getItem(COSName.OCPROPERTIES);

                    Assert.True(ocProps1 is COSDictionary);
                    Assert.True(ocProps2 is COSObject);

                    merger.appendDocument(doc2, doc1);
                    Assert.Equal(2, doc2.getNumberOfPages());
                }
            }
        }
    }
}
