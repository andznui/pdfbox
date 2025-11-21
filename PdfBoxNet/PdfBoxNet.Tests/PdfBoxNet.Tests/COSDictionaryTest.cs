/*
 * Converted from Apache PDFBox COSDictionaryTest.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using Xunit;
using org.apache.pdfbox.cos;

namespace PdfBoxNet.Tests;

/// <summary>
/// Tests for COSDictionary
/// </summary>
public class COSDictionaryTest
{
    [Fact]
    public void TestCOSDictionaryNotEqualsCOSStream()
    {
        COSDictionary cosDictionary = new COSDictionary();
        COSStream cosStream = new COSStream();
        cosDictionary.setItem(COSName.BE, COSName.BE);
        cosDictionary.setInt(COSName.LENGTH, 0);
        cosStream.setItem(COSName.BE, COSName.BE);
        Assert.NotEqual(cosDictionary, cosStream);
        Assert.NotEqual(cosStream, cosDictionary);
    }
}
