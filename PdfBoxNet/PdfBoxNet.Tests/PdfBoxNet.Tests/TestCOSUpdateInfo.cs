/*
 * Converted from Apache PDFBox TestCOSUpdateInfo.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using Xunit;
using org.apache.pdfbox.cos;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test class for COSUpdateInfo
/// </summary>
public class TestCOSUpdateInfo
{
    /// <summary>
    /// Tests isNeedToBeUpdate() and setNeedToBeUpdate() - tests the getter/setter methods.
    /// </summary>
    [Fact]
    public void TestIsSetNeedToBeUpdate()
    {
        COSDocumentState origin = new COSDocumentState();
        origin.setParsing(false);

        // COSDictionary
        COSUpdateInfo testCOSDictionary = new COSDictionary();
        testCOSDictionary.setNeedToBeUpdated(true);
        Assert.False(testCOSDictionary.isNeedToBeUpdated());
        testCOSDictionary.getUpdateState().setOriginDocumentState(origin);
        testCOSDictionary.setNeedToBeUpdated(true);
        Assert.True(testCOSDictionary.isNeedToBeUpdated());
        testCOSDictionary.setNeedToBeUpdated(false);
        Assert.False(testCOSDictionary.isNeedToBeUpdated());

        // COSObject
        COSUpdateInfo testCOSObject;
        testCOSObject = new COSObject(null);
        testCOSObject.setNeedToBeUpdated(true);
        Assert.False(testCOSObject.isNeedToBeUpdated());
        testCOSObject.getUpdateState().setOriginDocumentState(origin);
        testCOSObject.setNeedToBeUpdated(true);
        Assert.True(testCOSObject.isNeedToBeUpdated());
        testCOSObject.setNeedToBeUpdated(false);
        Assert.False(testCOSObject.isNeedToBeUpdated());
    }
}
