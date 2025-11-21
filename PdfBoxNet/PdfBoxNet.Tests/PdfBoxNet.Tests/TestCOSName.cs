/*
 * Converted from Apache PDFBox TestCOSName.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test for COSName
/// Converted from org.apache.pdfbox.cos.TestCOSName
/// </summary>
public class TestCOSName
{
    /// <summary>
    /// PDFBOX-4076: Check that characters outside of US_ASCII are not replaced with "?".
    /// </summary>
    [Fact]
    public void PDFBox4076()
    {
        try
        {
            string special = "中国你好!";
            var baos = new java.io.ByteArrayOutputStream();

            using (var document = new PDDocument())
            {
                PDPage page = new PDPage();
                document.addPage(page);
                document.getDocumentCatalog().getCOSObject().setString(COSName.getPDFName(special), special);

                document.save(baos);
            }

            using (var document = Loader.loadPDF(baos.toByteArray()))
            {
                COSDictionary catalogDict = document.getDocumentCatalog().getCOSObject();
                Assert.True(catalogDict.containsKey(special));
                Assert.Equal(special, catalogDict.getString(special));
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }
}
