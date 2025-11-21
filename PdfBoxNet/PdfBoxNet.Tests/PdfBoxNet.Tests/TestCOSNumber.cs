/*
 * Converted from Apache PDFBox TestCOSNumber.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using Xunit;
using org.apache.pdfbox.cos;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test class for COSNumber
/// Converted from org.apache.pdfbox.cos.TestCOSNumber
/// </summary>
public abstract class TestCOSNumber : TestCOSBase
{
    /// <summary>
    /// Test floatValue() - test that the correct float value is returned
    /// </summary>
    public abstract void TestFloatValue();

    /// <summary>
    /// Test intValue() - test that the correct int value is returned
    /// </summary>
    public abstract void TestIntValue();

    /// <summary>
    /// Test longValue() - test that the correct long value is returned
    /// </summary>
    public abstract void TestLongValue();

    /// <summary>
    /// Tests get() - tests a static constructor for COSNumber classes
    /// </summary>
    [Fact]
    public void TestGet()
    {
        try
        {
            // Ensure the basic static numbers are recognized
            Assert.Equal(COSInteger.ZERO, COSNumber.get("0"));
            Assert.Equal(COSInteger.ZERO, COSNumber.get("-"));
            Assert.Equal(COSInteger.ZERO, COSNumber.get("."));
            Assert.Equal(COSInteger.ONE, COSNumber.get("1"));
            Assert.Equal(COSInteger.TWO, COSNumber.get("2"));
            Assert.Equal(COSInteger.THREE, COSNumber.get("3"));
            // Test some arbitrary ints
            Assert.Equal(COSInteger.get(100), COSNumber.get("100"));
            Assert.Equal(COSInteger.get(256), COSNumber.get("256"));
            Assert.Equal(COSInteger.get(-1000), COSNumber.get("-1000"));
            Assert.Equal(COSInteger.get(2000), COSNumber.get("+2000"));
            // Some arbitrary floats
            Assert.Equal(new COSFloat(1.1f), COSNumber.get("1.1"));
            Assert.Equal(new COSFloat(100f), COSNumber.get("100.0"));
            Assert.Equal(new COSFloat(-100.001f), COSNumber.get("-100.001"));
            // according to the specs the exponential shall not be used
            // but obviously there some
            Assert.NotNull(COSNumber.get("-2e-006"));
            Assert.NotNull(COSNumber.get("-8e+05"));

            // IKVM may throw System.NullReferenceException instead of java.lang.NullPointerException
            Assert.ThrowsAny<Exception>(() => COSNumber.get(null));
            Assert.Throws<java.io.IOException>(() => COSNumber.get("a"));
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Failed to convert a number " + e.getMessage());
        }
    }

    /// <summary>
    /// PDFBOX-5176: large number, too big for a long leads to a COSInteger value which is marked as invalid
    /// </summary>
    [Fact]
    public void TestLargeNumber()
    {
        try
        {
            // max value
            COSNumber cosNumber = COSNumber.get(java.lang.Long.toString(java.lang.Long.MAX_VALUE));
            Assert.True(cosNumber is COSInteger);
            COSInteger cosInteger = (COSInteger)cosNumber;
            Assert.True(cosInteger.isValid());
            // min value
            cosNumber = COSNumber.get(java.lang.Long.toString(java.lang.Long.MIN_VALUE));
            Assert.True(cosNumber is COSInteger);
            cosInteger = (COSInteger)cosNumber;
            Assert.True(cosInteger.isValid());

            // out of range, max value
            cosNumber = COSNumber.get("18446744073307448448");
            Assert.True(cosNumber is COSInteger);
            cosInteger = (COSInteger)cosNumber;
            Assert.False(cosInteger.isValid());
            // out of range, min value
            cosNumber = COSNumber.get("-18446744073307448448");
            Assert.True(cosNumber is COSInteger);
            cosInteger = (COSInteger)cosNumber;
            Assert.False(cosInteger.isValid());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public void TestInvalidNumber()
    {
        try
        {
            COSNumber.get("18446744073307F448448");
            Assert.Fail("Was expecting an IOException");
        }
        catch (java.io.IOException)
        {
            // Expected exception
        }
    }
}
