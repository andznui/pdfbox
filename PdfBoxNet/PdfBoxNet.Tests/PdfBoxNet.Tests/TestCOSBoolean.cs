/*
 * Converted from Apache PDFBox TestCOSBoolean.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using System.Text;
using Xunit;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdfwriter;

namespace PdfBoxNet.Tests;

/// <summary>
/// Unittests for COSBoolean
/// Converted from org.apache.pdfbox.cos.TestCOSBoolean
/// </summary>
public class TestCOSBoolean : TestCOSBase
{
    private readonly COSBoolean cosBooleanTrue = COSBoolean.TRUE;
    private readonly COSBoolean cosBooleanFalse = COSBoolean.FALSE;

    public TestCOSBoolean()
    {
        testCOSBase = COSBoolean.TRUE;
    }

    [Fact]
    public void TestGetValue()
    {
        Assert.True(cosBooleanTrue.getValue());
        Assert.False(cosBooleanFalse.getValue());
    }

    [Fact]
    public void TestGetValueAsObject()
    {
        Assert.True(cosBooleanTrue.getValueAsObject() is java.lang.Boolean);
        Assert.Equal(java.lang.Boolean.TRUE, cosBooleanTrue.getValueAsObject());
        Assert.True(cosBooleanFalse.getValueAsObject() is java.lang.Boolean);
        Assert.Equal(java.lang.Boolean.FALSE, cosBooleanFalse.getValueAsObject());
    }

    [Fact]
    public void TestGetBoolean()
    {
        Assert.Equal(cosBooleanTrue, COSBoolean.getBoolean(java.lang.Boolean.TRUE));
        Assert.Equal(cosBooleanFalse, COSBoolean.getBoolean(java.lang.Boolean.FALSE));
    }

    [Fact]
    public void TestEquals()
    {
        COSBoolean test1 = COSBoolean.TRUE;
        COSBoolean test2 = COSBoolean.TRUE;
        COSBoolean test3 = COSBoolean.TRUE;
        // Reflexive (x == x)
        Assert.Equal(test1, test1);
        // Symmetric is preserved ( x==y then y===x)
        Assert.Equal(test2, test1);
        Assert.Equal(test1, test2);
        // Transitive (if x==y && y==z then x===z)
        Assert.Equal(test1, test2);
        Assert.Equal(test2, test3);
        Assert.Equal(test1, test3);

        Assert.NotEqual(COSBoolean.TRUE, COSBoolean.FALSE);
        // same 'value' but different type
        Assert.NotEqual((object)true, COSBoolean.TRUE);
        Assert.NotEqual((object)false, COSBoolean.FALSE);
    }

    [Fact]
    public override void TestAccept()
    {
        var outStream = new java.io.ByteArrayOutputStream();
        COSWriter visitor = new COSWriter(outStream);
        int index = 0;
        try
        {
            cosBooleanTrue.accept(visitor);
            testByteArrays(Encoding.GetEncoding("ISO-8859-1").GetBytes(cosBooleanTrue.toString()), outStream.toByteArray());
            outStream.reset();
            cosBooleanFalse.accept(visitor);
            testByteArrays(Encoding.GetEncoding("ISO-8859-1").GetBytes(cosBooleanFalse.toString()), outStream.toByteArray());
            outStream.reset();
        }
        catch (Exception e)
        {
            Assert.Fail("Failed to write " + index + " exception: " + e.Message);
        }
    }
}
