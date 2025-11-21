/*
 * Converted from Apache PDFBox TestCOSInteger.java
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
/// A test case for COSInteger
/// Converted from org.apache.pdfbox.cos.TestCOSInteger
/// </summary>
public class TestCOSInteger : TestCOSNumber
{
    public TestCOSInteger()
    {
        try
        {
            testCOSBase = COSNumber.get("0");
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Failed to create a COSNumber in setUp()");
        }
    }

    /// <summary>
    /// Tests equals() - ensures that the Object.equals() contract is obeyed. These are tested over
    /// a range of arbitrary values to ensure Consistency, Reflexivity, Symmetry, Transitivity and
    /// non-nullity.
    /// </summary>
    [Fact]
    public void TestEquals()
    {
        // Consistency
        for (int i = -1000; i < 3000; i += 200)
        {
            COSInteger test1 = COSInteger.get(i);
            COSInteger test2 = COSInteger.get(i);
            COSInteger test3 = COSInteger.get(i);
            // Reflexive (x == x)
            Assert.Equal(test1, test1);
            // Symmetric is preserved ( x==y then y===x)
            Assert.Equal(test2, test1);
            Assert.Equal(test1, test2);
            // Transitive (if x==y && y==z then x===z)
            Assert.Equal(test1, test2);
            Assert.Equal(test2, test3);
            Assert.Equal(test1, test3);

            COSInteger test4 = COSInteger.get(i + 1);
            Assert.NotEqual(test4, test1);
        }
    }

    /// <summary>
    /// Tests hashCode() - ensures that the Object.hashCode() contract is obeyed over a range of
    /// arbitrary values.
    /// </summary>
    [Fact]
    public void TestHashCode()
    {
        for (int i = -1000; i < 3000; i += 200)
        {
            COSInteger test1 = COSInteger.get(i);
            COSInteger test2 = COSInteger.get(i);
            Assert.Equal(test1.hashCode(), test2.hashCode());

            COSInteger test3 = COSInteger.get(i + 1);
            Assert.NotSame(test3.hashCode(), test1.hashCode());
        }
    }

    [Fact]
    public override void TestFloatValue()
    {
        for (int i = -1000; i < 3000; i += 200)
        {
            Assert.Equal((float)i, COSInteger.get(i).floatValue());
        }
    }

    [Fact]
    public override void TestIntValue()
    {
        for (int i = -1000; i < 3000; i += 200)
        {
            Assert.Equal(i, COSInteger.get(i).intValue());
        }
    }

    [Fact]
    public override void TestLongValue()
    {
        for (int i = -1000; i < 3000; i += 200)
        {
            Assert.Equal((long)i, COSInteger.get(i).longValue());
        }
    }

    [Fact]
    public override void TestAccept()
    {
        var outStream = new java.io.ByteArrayOutputStream();
        COSWriter visitor = new COSWriter(outStream);
        int index = 0;
        try
        {
            for (int i = -1000; i < 3000; i += 200)
            {
                index = i;
                COSInteger cosInt = COSInteger.get(i);
                cosInt.accept(visitor);
                testByteArrays(Encoding.GetEncoding("ISO-8859-1").GetBytes(i.ToString()), outStream.toByteArray());
                outStream.reset();
            }
        }
        catch (Exception e)
        {
            Assert.Fail("Failed to write " + index + " exception: " + e.Message);
        }
    }

    /// <summary>
    /// Tests writePDF() - this method takes an OutputStream and writes this object to it.
    /// </summary>
    [Fact]
    public void TestWritePDF()
    {
        var outStream = new java.io.ByteArrayOutputStream();
        int index = 0;
        try
        {
            for (int i = -1000; i < 3000; i += 200)
            {
                index = i;
                COSInteger cosInt = COSInteger.get(i);
                cosInt.writePDF(outStream);
                testByteArrays(Encoding.GetEncoding("ISO-8859-1").GetBytes(i.ToString()), outStream.toByteArray());
                outStream.reset();
            }
        }
        catch (Exception e)
        {
            Assert.Fail("Failed to write " + index + " exception: " + e.Message);
        }
    }
}
