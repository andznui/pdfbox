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
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.pdmodel.interactive.form;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Test for the PDButton class.
    /// </summary>
    public class PDButtonTest : IDisposable
    {
        // Use absolute path relative to the pdfbox root directory
        private static readonly string PDFBOX_ROOT = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", ".."));
        private static readonly java.io.File IN_DIR = new java.io.File(System.IO.Path.Combine(PDFBOX_ROOT, "pdfbox", "src", "test", "resources", "org", "apache", "pdfbox", "pdmodel", "interactive", "form"));
        private static readonly string NAME_OF_PDF = "AcroFormsBasicFields.pdf";
        private static readonly java.io.File TARGET_PDF_DIR = new java.io.File(System.IO.Path.Combine(PDFBOX_ROOT, "target", "pdfs"));

        private PDDocument document;
        private PDAcroForm acroForm;

        private PDDocument acrobatDocument;
        private PDAcroForm acrobatAcroForm;

        public PDButtonTest()
        {
            try
            {
                document = new PDDocument();
                acroForm = new PDAcroForm(document);

                acrobatDocument = Loader.loadPDF(new java.io.File(IN_DIR, NAME_OF_PDF));
                acrobatAcroForm = acrobatDocument.getDocumentCatalog().getAcroForm();
            }
            catch (IOException)
            {
                // Constructor doesn't throw in C#, handled in tests
            }
        }

        [Fact]
        public void createCheckBox()
        {
            PDButton buttonField = new PDCheckBox(acroForm);

            Assert.Equal(buttonField.getFieldType(), buttonField.getCOSObject().getNameAsString(COSName.FT));
            Assert.Equal("Btn", buttonField.getFieldType());
            Assert.False(buttonField.isPushButton());
            Assert.False(buttonField.isRadioButton());
        }

        [Fact]
        public void createPushButton()
        {
            PDButton buttonField = new PDPushButton(acroForm);

            Assert.Equal(buttonField.getFieldType(), buttonField.getCOSObject().getNameAsString(COSName.FT));
            Assert.Equal("Btn", buttonField.getFieldType());
            Assert.True(buttonField.isPushButton());
            Assert.False(buttonField.isRadioButton());
        }

        [Fact]
        public void createRadioButton()
        {
            PDButton buttonField = new PDRadioButton(acroForm);

            Assert.Equal(buttonField.getFieldType(), buttonField.getCOSObject().getNameAsString(COSName.FT));
            Assert.Equal("Btn", buttonField.getFieldType());
            Assert.True(buttonField.isRadioButton());
            Assert.False(buttonField.isPushButton());
        }

        /// <summary>
        /// PDFBOX-3656
        ///
        /// Test a radio button with options.
        /// This was causing an ArrayIndexOutOfBoundsException when trying to set to "Off", as this
        /// wasn't treated to be a valid option.
        /// </summary>
        [Fact]
        public void testRadioButtonWithOptions()
        {
            try
            {
                java.io.File file = new java.io.File(TARGET_PDF_DIR, "PDFBOX-3656.pdf");

                if (!file.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                using (PDDocument pdfDocument = Loader.loadPDF(file))
                {
                    PDRadioButton radioButton = (PDRadioButton)pdfDocument.getDocumentCatalog().getAcroForm().getField("Checking/Savings");
                    radioButton.setValue("Off");

                    var widgets = radioButton.getWidgets();
                    var iterator = widgets.iterator();
                    while (iterator.hasNext())
                    {
                        var widget = (org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)iterator.next();
                        Assert.Equal(COSName.Off, widget.getCOSObject().getItem(COSName.AS));
                    }
                }
            }
            catch (IOException e)
            {
                Assert.Fail("Unexpected IOException " + e.Message);
            }
        }

        /// <summary>
        /// PDFBOX-3682
        ///
        /// Test a radio button with options.
        /// Special handling for a radio button with /Opt and the On state not being named
        /// after the index.
        /// </summary>
        [Fact]
        public void testOptionsAndNamesNotNumbers()
        {
            try
            {
                java.io.File file = new java.io.File(TARGET_PDF_DIR, "PDFBOX-3682.pdf");

                if (!file.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                using (PDDocument pdfDocument = Loader.loadPDF(file))
                {
                    pdfDocument.getDocumentCatalog().getAcroForm().getField("RadioButton").setValue("c");
                    PDRadioButton radioButton = (PDRadioButton)pdfDocument.getDocumentCatalog().getAcroForm().getField("RadioButton");
                    radioButton.setValue("c");

                    // test that the old behavior is now invalid
                    Assert.NotEqual("2", radioButton.getValueAsString());
                    Assert.NotEqual("2",
                        ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(2)).getCOSObject().getNameAsString(COSName.AS));

                    // test for the correct behavior
                    Assert.Equal("c", radioButton.getValueAsString());
                    Assert.Equal("c",
                        ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(2)).getCOSObject().getNameAsString(COSName.AS));
                }
            }
            catch (IOException e)
            {
                Assert.Fail("Unexpected IOException " + e.Message);
            }
        }

        [Fact]
        public void retrieveAcrobatCheckBoxProperties()
        {
            PDCheckBox checkbox = (PDCheckBox)acrobatAcroForm.getField("Checkbox");
            Assert.NotNull(checkbox);
            Assert.Equal("Yes", checkbox.getOnValue());
            Assert.Equal(1, checkbox.getOnValues().size());
            Assert.True(checkbox.getOnValues().contains("Yes"));
        }

        [Fact]
        public void testAcrobatCheckBoxProperties()
        {
            try
            {
                PDCheckBox checkbox = (PDCheckBox)acrobatAcroForm.getField("Checkbox");
                Assert.Equal("Off", checkbox.getValue());
                Assert.Equal(false, checkbox.isChecked());

                checkbox.check();
                Assert.Equal(checkbox.getValue(), checkbox.getOnValue());
                Assert.Equal(true, checkbox.isChecked());

                checkbox.setValue("Yes");
                Assert.Equal(checkbox.getValue(), checkbox.getOnValue());
                Assert.Equal(true, checkbox.isChecked());
                Assert.Equal(COSName.YES, checkbox.getCOSObject().getDictionaryObject(COSName.AS));

                checkbox.setValue("Off");
                Assert.Equal(COSName.Off.getName(), checkbox.getValue());
                Assert.Equal(false, checkbox.isChecked());
                Assert.Equal(COSName.Off, checkbox.getCOSObject().getDictionaryObject(COSName.AS));

                checkbox = (PDCheckBox)acrobatAcroForm.getField("Checkbox-DefaultValue");
                Assert.Equal(checkbox.getDefaultValue(), checkbox.getOnValue());

                checkbox.setDefaultValue("Off");
                Assert.Equal(COSName.Off.getName(), checkbox.getDefaultValue());
            }
            catch (IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void setValueForAbstractedAcrobatCheckBox()
        {
            try
            {
                PDField checkbox = acrobatAcroForm.getField("Checkbox");

                checkbox.setValue("Yes");
                Assert.Equal(checkbox.getValueAsString(), ((PDCheckBox)checkbox).getOnValue());
                Assert.Equal(true, ((PDCheckBox)checkbox).isChecked());
                Assert.Equal(COSName.YES, checkbox.getCOSObject().getDictionaryObject(COSName.AS));

                checkbox.setValue("Off");
                Assert.Equal(COSName.Off.getName(), checkbox.getValueAsString());
                Assert.Equal(false, ((PDCheckBox)checkbox).isChecked());
                Assert.Equal(COSName.Off, checkbox.getCOSObject().getDictionaryObject(COSName.AS));
            }
            catch (IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void testAcrobatCheckBoxGroupProperties()
        {
            try
            {
                PDCheckBox checkbox = (PDCheckBox)acrobatAcroForm.getField("CheckboxGroup");
                Assert.Equal("Off", checkbox.getValue());
                Assert.Equal(false, checkbox.isChecked());

                checkbox.check();
                Assert.Equal(checkbox.getValue(), checkbox.getOnValue());
                Assert.Equal(true, checkbox.isChecked());

                Assert.Equal(3, checkbox.getOnValues().size());
                Assert.True(checkbox.getOnValues().contains("Option1"));
                Assert.True(checkbox.getOnValues().contains("Option2"));
                Assert.True(checkbox.getOnValues().contains("Option3"));

                // test a value which sets one of the individual checkboxes within the group
                checkbox.setValue("Option1");
                Assert.Equal("Option1", checkbox.getValue());
                Assert.Equal("Option1", checkbox.getValueAsString());

                // ensure that for the widgets representing the individual checkboxes
                // the AS entry has been set
                Assert.Equal("Option1", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(0)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(1)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(2)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(3)).getAppearanceState().getName());

                // test a value which sets two of the individual chekboxes within the group
                // as the have the same name entry for being checked
                checkbox.setValue("Option3");
                Assert.Equal("Option3", checkbox.getValue());
                Assert.Equal("Option3", checkbox.getValueAsString());

                // ensure that for both widgets representing the individual checkboxes
                // the AS entry has been set
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(0)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(1)).getAppearanceState().getName());
                Assert.Equal("Option3", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(2)).getAppearanceState().getName());
                Assert.Equal("Option3", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(3)).getAppearanceState().getName());
            }
            catch (IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void setValueForAbstractedCheckBoxGroup()
        {
            try
            {
                PDField checkbox = acrobatAcroForm.getField("CheckboxGroup");

                // test a value which sets one of the individual checkboxes within the group
                checkbox.setValue("Option1");
                Assert.Equal("Option1", checkbox.getValueAsString());

                // ensure that for the widgets representing the individual checkboxes
                // the AS entry has been set
                Assert.Equal("Option1", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(0)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(1)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(2)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(3)).getAppearanceState().getName());

                // test a value which sets two of the individual chekboxes within the group
                // as the have the same name entry for being checked
                checkbox.setValue("Option3");
                Assert.Equal("Option3", checkbox.getValueAsString());

                // ensure that for both widgets representing the individual checkboxes
                // the AS entry has been set
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(0)).getAppearanceState().getName());
                Assert.Equal("Off", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(1)).getAppearanceState().getName());
                Assert.Equal("Option3", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(2)).getAppearanceState().getName());
                Assert.Equal("Option3", ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)checkbox.getWidgets().get(3)).getAppearanceState().getName());
            }
            catch (IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void setCheckboxInvalidValue()
        {
            PDCheckBox checkbox = (PDCheckBox)acrobatAcroForm.getField("Checkbox");
            // Set a value which doesn't match the radio button list
            Assert.Throws<java.lang.IllegalArgumentException>(() => checkbox.setValue("InvalidValue"));
        }

        [Fact]
        public void setCheckboxGroupInvalidValue()
        {
            PDCheckBox checkbox = (PDCheckBox)acrobatAcroForm.getField("CheckboxGroup");
            // Set a value which doesn't match the radio button list
            Assert.Throws<java.lang.IllegalArgumentException>(() => checkbox.setValue("InvalidValue"));
        }

        [Fact]
        public void setAbstractedCheckboxInvalidValue()
        {
            PDField checkbox = acrobatAcroForm.getField("Checkbox");
            // Set a value which doesn't match the radio button list
            Assert.Throws<java.lang.IllegalArgumentException>(() => checkbox.setValue("InvalidValue"));
        }

        [Fact]
        public void setAbstractedCheckboxGroupInvalidValue()
        {
            PDField checkbox = acrobatAcroForm.getField("CheckboxGroup");
            // Set a value which doesn't match the radio button list
            Assert.Throws<java.lang.IllegalArgumentException>(() => checkbox.setValue("InvalidValue"));
        }

        [Fact]
        public void retrieveAcrobatRadioButtonProperties()
        {
            PDRadioButton radioButton = (PDRadioButton)acrobatAcroForm.getField("RadioButtonGroup");
            Assert.NotNull(radioButton);
            Assert.Equal(2, radioButton.getOnValues().size());
            Assert.True(radioButton.getOnValues().contains("RadioButton01"));
            Assert.True(radioButton.getOnValues().contains("RadioButton02"));
        }

        [Fact]
        public void testAcrobatRadioButtonProperties()
        {
            try
            {
                PDRadioButton radioButton = (PDRadioButton)acrobatAcroForm.getField("RadioButtonGroup");

                // Set value so that first radio button option is selected
                radioButton.setValue("RadioButton01");
                Assert.Equal("RadioButton01", radioButton.getValue());
                // First option shall have /RadioButton01, second shall have /Off
                Assert.Equal(COSName.getPDFName("RadioButton01"),
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(0)).getCOSObject().getDictionaryObject(COSName.AS));
                Assert.Equal(COSName.Off,
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(1)).getCOSObject().getDictionaryObject(COSName.AS));

                // Set value so that second radio button option is selected
                radioButton.setValue("RadioButton02");
                Assert.Equal("RadioButton02", radioButton.getValue());
                // First option shall have /Off, second shall have /RadioButton02
                Assert.Equal(COSName.Off,
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(0)).getCOSObject().getDictionaryObject(COSName.AS));
                Assert.Equal(COSName.getPDFName("RadioButton02"),
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(1)).getCOSObject().getDictionaryObject(COSName.AS));
            }
            catch (IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void setValueForAbstractedAcrobatRadioButton()
        {
            try
            {
                PDField radioButton = acrobatAcroForm.getField("RadioButtonGroup");

                // Set value so that first radio button option is selected
                radioButton.setValue("RadioButton01");
                Assert.Equal("RadioButton01", radioButton.getValueAsString());
                // First option shall have /RadioButton01, second shall have /Off
                Assert.Equal(COSName.getPDFName("RadioButton01"),
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(0)).getCOSObject().getDictionaryObject(COSName.AS));
                Assert.Equal(COSName.Off,
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(1)).getCOSObject().getDictionaryObject(COSName.AS));

                // Set value so that second radio button option is selected
                radioButton.setValue("RadioButton02");
                Assert.Equal("RadioButton02", radioButton.getValueAsString());
                // First option shall have /Off, second shall have /RadioButton02
                Assert.Equal(COSName.Off,
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(0)).getCOSObject().getDictionaryObject(COSName.AS));
                Assert.Equal(COSName.getPDFName("RadioButton02"),
                    ((org.apache.pdfbox.pdmodel.interactive.annotation.PDAnnotationWidget)radioButton.getWidgets().get(1)).getCOSObject().getDictionaryObject(COSName.AS));
            }
            catch (IOException e)
            {
                Assert.Fail("IOException occurred: " + e.Message);
            }
        }

        [Fact]
        public void setRadioButtonInvalidValue()
        {
            PDRadioButton radioButton = (PDRadioButton)acrobatAcroForm.getField("RadioButtonGroup");
            // Set a value which doesn't match the radio button list
            Assert.Throws<java.lang.IllegalArgumentException>(() => radioButton.setValue("InvalidValue"));
        }

        [Fact]
        public void setAbstractedRadioButtonInvalidValue()
        {
            PDField radioButton = acrobatAcroForm.getField("RadioButtonGroup");
            // Set a value which doesn't match the radio button list
            Assert.Throws<java.lang.IllegalArgumentException>(() => radioButton.setValue("InvalidValue"));
        }

        public void Dispose()
        {
            try
            {
                document?.close();
                acrobatDocument?.close();
            }
            catch (IOException)
            {
                // Ignore disposal errors
            }
        }
    }
}
