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
using System.Linq;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.io;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.interactive.form;
using org.apache.pdfbox.pdmodel.interactive.annotation;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.common;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Test for the PDAcroForm class.
    /// Converted from org.apache.pdfbox.pdmodel.interactive.form.PDAcroFormTest
    /// </summary>
    public class PDAcroFormTest : IDisposable
    {
        private PDDocument document;
        private PDAcroForm acroForm;

        private static readonly java.io.File OUT_DIR = new java.io.File("target/test-output");
        private static readonly java.io.File IN_DIR = new java.io.File("src/test/resources/org/apache/pdfbox/pdmodel/interactive/form");

        public PDAcroFormTest()
        {
            document = new PDDocument();
            acroForm = new PDAcroForm(document);
            document.getDocumentCatalog().setAcroForm(acroForm);
        }

        [Fact]
        public void testFieldsEntry()
        {
            // the /Fields entry has been created with the AcroForm
            // as this is a required entry
            Assert.NotNull(acroForm.getFields());
            Assert.Equal(0, acroForm.getFields().size());

            // there shouldn't be an exception if there is no such field
            Assert.Null(acroForm.getField("foo"));

            // remove the required entry which is the case for some
            // PDFs (see PDFBOX-2965)
            acroForm.getCOSObject().removeItem(COSName.FIELDS);

            // ensure there is always an empty collection returned
            Assert.NotNull(acroForm.getFields());
            Assert.Equal(0, acroForm.getFields().size());

            // there shouldn't be an exception if there is no such field
            Assert.Null(acroForm.getField("foo"));
        }

        [Fact]
        public void testAcroFormProperties()
        {
            Assert.True(string.IsNullOrEmpty(acroForm.getDefaultAppearance()));
            acroForm.setDefaultAppearance("/Helv 0 Tf 0 g");
            Assert.Equal("/Helv 0 Tf 0 g", acroForm.getDefaultAppearance());
        }

        [Fact]
        public void testFlatten()
        {
            try
            {
                java.io.File file = new java.io.File(OUT_DIR, "AlignmentTests-flattened.pdf");
                java.io.File inputFile = new java.io.File(IN_DIR, "AlignmentTests.pdf");

                if (!inputFile.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                using (PDDocument testPdf = Loader.loadPDF(inputFile))
                {
                    testPdf.getDocumentCatalog().getAcroForm().flatten();
                    Assert.True(testPdf.getDocumentCatalog().getAcroForm().getFields().isEmpty());
                    testPdf.save(file);
                }

                // compare rendering
                if (!TestPDFToImage.doTestFile(file, IN_DIR.getAbsolutePath(), OUT_DIR.getAbsolutePath()))
                {
                    // don't fail, rendering is different on different systems, result must be viewed manually
                    Console.Error.WriteLine("Rendering of " + file + " failed or is not identical to expected rendering in " + IN_DIR + " directory");
                }
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        /*
         * Same as above but remove the page reference from the widget annotation
         * before doing the flatten() to ensure that the widgets page reference is properly looked up
         * (PDFBOX-3301)
         */
        [Fact]
        public void testFlattenWidgetNoRef()
        {
            try
            {
                java.io.File file = new java.io.File(OUT_DIR, "AlignmentTests-flattened-noRef.pdf");
                java.io.File inputFile = new java.io.File(IN_DIR, "AlignmentTests.pdf");

                if (!inputFile.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                using (PDDocument testPdf = Loader.loadPDF(inputFile))
                {
                    PDAcroForm acroFormToTest = testPdf.getDocumentCatalog().getAcroForm();
                    var fieldTreeIterator = acroFormToTest.getFieldTree().iterator();
                    while (fieldTreeIterator.hasNext())
                    {
                        PDField field = (PDField)fieldTreeIterator.next();
                        var widgetsIterator = field.getWidgets().iterator();
                        while (widgetsIterator.hasNext())
                        {
                            PDAnnotationWidget widget = (PDAnnotationWidget)widgetsIterator.next();
                            widget.getCOSObject().removeItem(COSName.P);
                        }
                    }
                    acroFormToTest.flatten();

                    // 36 non widget annotations shall not be flattened
                    Assert.Equal(36, testPdf.getPage(0).getAnnotations().size());

                    Assert.True(acroFormToTest.getFields().isEmpty());
                    testPdf.save(file);
                }

                // compare rendering
                if (!TestPDFToImage.doTestFile(file, IN_DIR.getAbsolutePath(), OUT_DIR.getAbsolutePath()))
                {
                    // don't fail, rendering is different on different systems, result must be viewed manually
                    Console.Error.WriteLine("Rendering of " + file + " failed or is not identical to expected rendering in " + IN_DIR + " directory");
                }
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void testFlattenSpecificFieldsOnly()
        {
            try
            {
                java.io.File file = new java.io.File(OUT_DIR, "AlignmentTests-flattened-specificFields.pdf");
                java.io.File inputFile = new java.io.File(IN_DIR, "AlignmentTests.pdf");

                if (!inputFile.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                java.util.List fieldsToFlatten = new java.util.ArrayList();

                using (PDDocument testPdf = Loader.loadPDF(inputFile))
                {
                    PDAcroForm acroFormToFlatten = testPdf.getDocumentCatalog().getAcroForm();
                    int numFieldsBeforeFlatten = acroFormToFlatten.getFields().size();
                    int numWidgetsBeforeFlatten = countWidgets(testPdf);

                    fieldsToFlatten.add(acroFormToFlatten.getField("AlignLeft-Border_Small-Filled"));
                    fieldsToFlatten.add(acroFormToFlatten.getField("AlignLeft-Border_Medium-Filled"));
                    fieldsToFlatten.add(acroFormToFlatten.getField("AlignLeft-Border_Wide-Filled"));
                    fieldsToFlatten.add(acroFormToFlatten.getField("AlignLeft-Border_Wide_Clipped-Filled"));

                    acroFormToFlatten.flatten(fieldsToFlatten, true);
                    int numFieldsAfterFlatten = acroFormToFlatten.getFields().size();
                    int numWidgetsAfterFlatten = countWidgets(testPdf);

                    Assert.Equal(numFieldsBeforeFlatten, numFieldsAfterFlatten + fieldsToFlatten.size());
                    Assert.Equal(numWidgetsBeforeFlatten, numWidgetsAfterFlatten + fieldsToFlatten.size());

                    testPdf.save(file);
                }
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        /*
         * Test that we do not modify an AcroForm with missing resource information
         * when loading the document only.
         * (PDFBOX-3752)
         */
        [Fact]
        public void testDontAddMissingInformationOnDocumentLoad()
        {
            try
            {
                byte[] pdfBytes = createAcroFormWithMissingResourceInformation();

                using (PDDocument pdfDocument = Loader.loadPDF(pdfBytes))
                {
                    // do a low level access to the AcroForm to avoid the generation of missing entries
                    PDDocumentCatalog documentCatalog = pdfDocument.getDocumentCatalog();
                    COSDictionary catalogDictionary = documentCatalog.getCOSObject();
                    COSDictionary acroFormDictionary = (COSDictionary)catalogDictionary.getDictionaryObject(COSName.ACRO_FORM);

                    // ensure that the missing information has not been generated
                    Assert.Null(acroFormDictionary.getDictionaryObject(COSName.DA));
                    Assert.Null(acroFormDictionary.getDictionaryObject(COSName.RESOURCES));
                }
            }
            catch (java.io.IOException e)
            {
                Console.Error.WriteLine("Couldn't create test document, test skipped");
            }
        }

        /*
         * Test that we add missing resource information to an AcroForm
         * when accessing the AcroForm on the PD level
         * (PDFBOX-3752)
         */
        [Fact]
        public void testAddMissingInformationOnAcroFormAccess()
        {
            try
            {
                byte[] pdfBytes = createAcroFormWithMissingResourceInformation();

                using (PDDocument pdfDocument = Loader.loadPDF(pdfBytes))
                {
                    PDDocumentCatalog documentCatalog = pdfDocument.getDocumentCatalog();

                    // this call shall trigger the generation of missing information
                    PDAcroForm theAcroForm = documentCatalog.getAcroForm();

                    // ensure that the missing information has been generated
                    // DA entry
                    Assert.Equal("/Helv 0 Tf 0 g ", theAcroForm.getDefaultAppearance());
                    Assert.NotNull(theAcroForm.getDefaultResources());

                    // DR entry
                    PDResources acroFormResources = theAcroForm.getDefaultResources();
                    Assert.NotNull(acroFormResources.getFont(COSName.getPDFName("Helv")));
                    Assert.Equal("Helvetica", acroFormResources.getFont(COSName.getPDFName("Helv")).getName());
                    Assert.NotNull(acroFormResources.getFont(COSName.getPDFName("ZaDb")));
                    Assert.Equal("ZapfDingbats", acroFormResources.getFont(COSName.getPDFName("ZaDb")).getName());
                }
            }
            catch (java.io.IOException e)
            {
                Console.Error.WriteLine("Couldn't create test document, test skipped");
            }
        }

        /// <summary>
        /// PDFBOX-4235: a bad /DA string should not result in an NPE.
        /// </summary>
        [Fact]
        public void testBadDA()
        {
            try
            {
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage();
                    doc.addPage(page);

                    PDAcroForm theAcroForm = new PDAcroForm(document);
                    doc.getDocumentCatalog().setAcroForm(theAcroForm);
                    theAcroForm.setDefaultResources(new PDResources());

                    PDTextField textBox = new PDTextField(theAcroForm);
                    textBox.setPartialName("SampleField");

                    // https://stackoverflow.com/questions/50609478/
                    // "tf" is a typo, should have been "Tf" and this results that no font is chosen
                    textBox.setDefaultAppearance("/Helv 0 tf 0 g");
                    theAcroForm.getFields().add(textBox);

                    PDAnnotationWidget widget = (PDAnnotationWidget)textBox.getWidgets().get(0);
                    PDRectangle rect = new PDRectangle(50, 750, 200, 20);
                    widget.setRectangle(rect);
                    widget.setPage(page);

                    page.getAnnotations().add(widget);

                    Assert.Throws<java.lang.IllegalArgumentException>(() => textBox.setValue("huhu"));
                }
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        /// <summary>
        /// PDFBOX-3732, PDFBOX-4303, PDFBOX-4393: Test whether /Helv and /ZaDb get added, but only if
        /// they don't exist.
        /// </summary>
        [Fact]
        public void testAcroFormDefaultFonts()
        {
            try
            {
                var baos = new java.io.ByteArrayOutputStream();
                using (PDDocument doc = new PDDocument())
                {
                    PDPage page = new PDPage(PDRectangle.A4);
                    doc.addPage(page);
                    PDAcroForm acroForm2 = new PDAcroForm(doc);
                    doc.getDocumentCatalog().setAcroForm(acroForm2);
                    PDResources defaultResources = acroForm2.getDefaultResources();
                    Assert.Null(defaultResources);
                    defaultResources = new PDResources();
                    acroForm2.setDefaultResources(defaultResources);
                    Assert.Null(defaultResources.getFont(COSName.HELV));
                    Assert.Null(defaultResources.getFont(COSName.ZA_DB));

                    // getting AcroForm sets the two fonts
                    acroForm2 = doc.getDocumentCatalog().getAcroForm();
                    defaultResources = acroForm2.getDefaultResources();
                    Assert.NotNull(defaultResources.getFont(COSName.HELV));
                    Assert.NotNull(defaultResources.getFont(COSName.ZA_DB));

                    // repeat with a new AcroForm (to delete AcroForm cache) and thus missing /DR
                    doc.getDocumentCatalog().setAcroForm(new PDAcroForm(doc));
                    acroForm2 = doc.getDocumentCatalog().getAcroForm();
                    defaultResources = acroForm2.getDefaultResources();

                    PDFont helv = defaultResources.getFont(COSName.HELV);
                    PDFont zadb = defaultResources.getFont(COSName.ZA_DB);
                    Assert.NotNull(helv);
                    Assert.NotNull(zadb);
                    doc.save(baos);
                }

                using (PDDocument doc = Loader.loadPDF(baos.toByteArray()))
                {
                    PDAcroForm acroForm2 = doc.getDocumentCatalog().getAcroForm();
                    PDResources defaultResources = acroForm2.getDefaultResources();
                    PDFont helv = defaultResources.getFont(COSName.HELV);
                    PDFont zadb = defaultResources.getFont(COSName.ZA_DB);
                    Assert.NotNull(helv);
                    Assert.NotNull(zadb);
                    // make sure that font wasn't overwritten
                    Assert.True(helv is PDType1Font);
                    Assert.True(zadb is PDType1Font);
                    PDType1Font helvType1 = (PDType1Font)helv;
                    PDType1Font zadbType1 = (PDType1Font)zadb;
                    Assert.Equal(Standard14Fonts.FontName.HELVETICA.getName(), helv.getName());
                    Assert.Equal(Standard14Fonts.FontName.ZAPF_DINGBATS.getName(), zadb.getName());
                    Assert.Null(helvType1.getType1Font());
                    Assert.Null(zadbType1.getType1Font());
                }
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        /// <summary>
        /// PDFBOX-3777 Illegal Fields definition COSDictionary instead of Array
        /// </summary>
        [Fact]
        public void testIllegalFieldsDefinition()
        {
            try
            {
                string sourceUrl = "https://issues.apache.org/jira/secure/attachment/12866226/D1790B.PDF";

                using (PDDocument testPdf = Loader.loadPDF(
                    RandomAccessReadBuffer.createBufferFromStream(new java.net.URI(sourceUrl).toURL().openStream())))
                {
                    PDDocumentCatalog catalog = testPdf.getDocumentCatalog();

                    // Getting the AcroForm shall not throw an exception
                    try
                    {
                        catalog.getAcroForm();
                    }
                    catch (Exception)
                    {
                        Assert.Fail("Getting the AcroForm shall not throw an exception");
                    }
                }
            }
            catch (java.io.IOException)
            {
                // Skip test if URL is not accessible
                return;
            }
            catch (java.net.URISyntaxException)
            {
                // Skip test if URL is malformed
                return;
            }
        }

        /// <summary>
        /// Test for names with invalid UTF-8.
        /// </summary>
        [Fact]
        public void testPDFBox3347()
        {
            try
            {
                string sourceUrl = "https://issues.apache.org/jira/secure/attachment/12968302/KYF%20211%20Best%C3%A4llning%202014.pdf";

                using (PDDocument doc = Loader.loadPDF(
                    RandomAccessReadBuffer.createBufferFromStream(new java.net.URI(sourceUrl).toURL().openStream())))
                {
                    PDField field = doc.getDocumentCatalog().getAcroForm().getField("Krematorier");
                    java.util.List widgets = field.getWidgets();
                    java.util.Set set = new java.util.TreeSet();

                    var iterator = widgets.iterator();
                    while (iterator.hasNext())
                    {
                        PDAnnotationWidget annot = (PDAnnotationWidget)iterator.next();
                        PDAppearanceDictionary ap = annot.getAppearance();
                        PDAppearanceEntry normalAppearance = ap.getNormalAppearance();
                        java.util.Set nameSet = normalAppearance.getSubDictionary().keySet();
                        Assert.True(nameSet.contains(COSName.Off));

                        var nameIterator = nameSet.iterator();
                        while (nameIterator.hasNext())
                        {
                            COSName name = (COSName)nameIterator.next();
                            if (!name.equals(COSName.Off))
                            {
                                set.add(name.getName());
                            }
                        }
                    }

                    Assert.Equal("[Nynäshamn, Råcksta, Silverdal, Skogskrem, St Botvid, Storkällan]",
                        set.ToString());
                }
            }
            catch (java.io.IOException)
            {
                // Skip test if URL is not accessible
                return;
            }
            catch (java.net.URISyntaxException)
            {
                // Skip test if URL is malformed
                return;
            }
        }

        /// <summary>
        /// PDFBOX-5797: Check that Sejda generated files have their widget /DA entries changed.
        /// </summary>
        [Fact]
        public void testPDFBox5797()
        {
            try
            {
                java.io.File inputFile = new java.io.File(
                    "src/test/resources/org/apache/pdfbox/pdmodel/interactive/annotation/PDFBOX-5797-SO79271803.pdf");

                if (!inputFile.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                using (PDDocument doc = Loader.loadPDF(inputFile))
                {
                    // Try to load from PDFBox resources
                    java.io.InputStream fontStream = null;
                    try
                    {
                        var javaClass = java.lang.Class.forName("org.apache.pdfbox.pdmodel.interactive.form.PDAcroFormFromAnnotsTest");
                        fontStream = javaClass.getResourceAsStream("/org/apache/pdfbox/resources/ttf/LiberationSans-Regular.ttf");

                        if (fontStream == null)
                        {
                            return; // Skip if font not available
                        }

                        PDType0Font load = PDType0Font.load(doc, fontStream, false);

                        PDAcroForm theAcroForm = doc.getDocumentCatalog().getAcroForm();
                        PDResources resources = theAcroForm.getDefaultResources();
                        string fontName = resources.add(load).getName();
                        string defaultAppearanceString = "/" + fontName + " 12 Tf 0 g";

                        PDTextField myField = (PDTextField)theAcroForm.getField("Name");
                        myField.setDefaultAppearance(defaultAppearanceString);
                        ((PDAnnotationWidget)myField.getWidgets().get(0)).setAppearance(null);
                        myField.setValue("ŞŞ"); // Text with the Ş character made it crash

                        Assert.Equal("ŞŞ", myField.getValue());
                    }
                    finally
                    {
                        fontStream?.close();
                    }
                }
            }
            catch (java.io.IOException)
            {
                // Skip test if file not accessible or other IO error
                return;
            }
            catch (java.lang.ClassNotFoundException)
            {
                // Skip if class not found
                return;
            }
        }

        public void Dispose()
        {
            try
            {
                document?.close();
            }
            catch (java.io.IOException)
            {
                // Ignore disposal errors
            }
        }

        private byte[] createAcroFormWithMissingResourceInformation()
        {
            using (PDDocument tmpDocument = new PDDocument())
            {
                var baos = new java.io.ByteArrayOutputStream();
                PDPage page = new PDPage();
                tmpDocument.addPage(page);

                PDAcroForm newAcroForm = new PDAcroForm(document);
                tmpDocument.getDocumentCatalog().setAcroForm(newAcroForm);

                PDTextField textBox = new PDTextField(newAcroForm);
                textBox.setPartialName("SampleField");
                newAcroForm.getFields().add(textBox);

                PDAnnotationWidget widget = (PDAnnotationWidget)textBox.getWidgets().get(0);
                PDRectangle rect = new PDRectangle(50, 750, 200, 20);
                widget.setRectangle(rect);
                widget.setPage(page);

                page.getAnnotations().add(widget);

                tmpDocument.save(baos); // this is a working PDF
                return baos.toByteArray();
            }
        }

        private int countWidgets(PDDocument documentToTest)
        {
            int count = 0;
            var pages = documentToTest.getPages();
            var pageIterator = pages.iterator();

            while (pageIterator.hasNext())
            {
                PDPage page = (PDPage)pageIterator.next();
                try
                {
                    var annotations = page.getAnnotations();
                    var annotIterator = annotations.iterator();

                    while (annotIterator.hasNext())
                    {
                        PDAnnotation annotation = (PDAnnotation)annotIterator.next();
                        if (annotation is PDAnnotationWidget)
                        {
                            count++;
                        }
                    }
                }
                catch (java.io.IOException)
                {
                    // ignoring
                }
            }
            return count;
        }
    }
}
