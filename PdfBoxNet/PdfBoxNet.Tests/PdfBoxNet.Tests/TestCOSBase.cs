/*
 * Converted from Apache PDFBox TestCOSBase.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using Xunit;
using org.apache.pdfbox.cos;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test class for COSBase
/// Converted from org.apache.pdfbox.cos.TestCOSBase
/// </summary>
public abstract class TestCOSBase
{
    /// <summary>
    /// The COSBase abstraction of the object being tested
    /// </summary>
    protected static COSBase testCOSBase;

    /// <summary>
    /// Tests getCOSObject() - tests that the underlying object is returned
    /// </summary>
    [Fact]
    public void TestGetCOSObject()
    {
        Assert.Equal(testCOSBase, testCOSBase.getCOSObject());
    }

    /// <summary>
    /// Test accept() - tests the interface for visiting a document at the COS level
    /// </summary>
    public abstract void TestAccept();

    /// <summary>
    /// Tests isDirect() and setDirect() - tests the getter/setter methods
    /// </summary>
    [Fact]
    public void TestIsSetDirect()
    {
        testCOSBase.setDirect(true);
        Assert.True(testCOSBase.isDirect());
        testCOSBase.setDirect(false);
        Assert.False(testCOSBase.isDirect());
    }

    /// <summary>
    /// A simple utility function to compare two byte arrays
    /// </summary>
    /// <param name="byteArr1">the expected byte array</param>
    /// <param name="byteArr2">the byte array being compared</param>
    protected void testByteArrays(byte[] byteArr1, byte[] byteArr2)
    {
        Assert.Equal(byteArr1.Length, byteArr2.Length);
        for (int i = 0; i < byteArr1.Length; i++)
        {
            Assert.Equal(byteArr1[i], byteArr2[i]);
        }
    }
}
