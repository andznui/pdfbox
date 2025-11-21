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
using System.Collections.Generic;
using System.IO;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.multipdf;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.interactive.annotation;
using Xunit;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Test merging different PDFs with Annotations.
    /// </summary>
    public class MergeAnnotationsTest
    {
        private static readonly string OUT_DIR = "target/test-output/merge/";
        private static readonly string TARGET_PDF_DIR = "target/pdfs";

        public MergeAnnotationsTest()
        {
            Directory.CreateDirectory(OUT_DIR);
        }

        /// <summary>
        /// PDFBOX-1065 Ensure that after merging the PDFs there are all link
        /// annotations and they point to the correct page.
        /// </summary>
        [Fact]
        public void TestLinkAnnotations()
        {
            // Merge the PDFs from PDFBOX-1065
            var merger = new PDFMergerUtility();
            var file1 = new java.io.File(TARGET_PDF_DIR, "PDFBOX-1065-1.pdf");
            var file2 = new java.io.File(TARGET_PDF_DIR, "PDFBOX-1065-2.pdf");

            // Skip test if required files don't exist
            if (!file1.exists() || !file2.exists())
            {
                return; // Skip test
            }

            var pdfOutput = new java.io.File(OUT_DIR, "PDFBOX-1065.pdf");
            merger.setDestinationFileName(pdfOutput.getAbsolutePath());
            merger.addSource(file1);
            merger.addSource(file2);
            merger.mergeDocuments(null);

            // Test merge result
            using (var mergedPDF = Loader.loadPDF(pdfOutput))
            {
                Assert.Equal(6, mergedPDF.getNumberOfPages());

                var destinations = mergedPDF.getDocumentCatalog().getDests();

                // Each document has 3 annotations with 2 entries in the /Dests dictionary per annotation.
                // One for the source and one for the target.
                Assert.Equal(12, destinations.getCOSObject().entrySet().size());

                var sourceAnnotations01 = mergedPDF.getPage(0).getAnnotations();
                var sourceAnnotations02 = mergedPDF.getPage(3).getAnnotations();

                var targetAnnotations01 = mergedPDF.getPage(2).getAnnotations();
                var targetAnnotations02 = mergedPDF.getPage(5).getAnnotations();

                // Test for the first set of annotations to be merged and linked correctly
                Assert.Equal(3, sourceAnnotations01.size());
                Assert.Equal(3, targetAnnotations01.size());
                Assert.True(TestAnnotationsMatch(sourceAnnotations01, targetAnnotations01));

                // Test for the second set of annotations to be merged and linked correctly
                Assert.Equal(3, sourceAnnotations02.size());
                Assert.Equal(3, targetAnnotations02.size());
                Assert.True(TestAnnotationsMatch(sourceAnnotations02, targetAnnotations02));
            }
        }

        /// <summary>
        /// Source and target annotations are linked by name with the target annotation's name
        /// being the source annotation's name prepended with 'annoRef_'
        /// </summary>
        private bool TestAnnotationsMatch(java.util.List sourceAnnots, java.util.List targetAnnots)
        {
            var targetAnnotsByName = new Dictionary<string, PDAnnotation>();

            // Fill the map with the annotations destination name
            var targetIterator = targetAnnots.iterator();
            while (targetIterator.hasNext())
            {
                var targetAnnot = (PDAnnotation)targetIterator.next();
                var destinationName = (COSName)targetAnnot.getCOSObject().getDictionaryObject(COSName.DEST);
                if (destinationName != null)
                {
                    targetAnnotsByName[destinationName.getName()] = targetAnnot;
                }
            }

            // Try to lookup the target annotation for the source annotation by destination name
            var sourceIterator = sourceAnnots.iterator();
            while (sourceIterator.hasNext())
            {
                var sourceAnnot = (PDAnnotation)sourceIterator.next();
                var destinationName = (COSName)sourceAnnot.getCOSObject().getDictionaryObject(COSName.DEST);
                if (destinationName != null)
                {
                    string lookupKey = "annoRef_" + destinationName.getName();
                    if (!targetAnnotsByName.ContainsKey(lookupKey))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
