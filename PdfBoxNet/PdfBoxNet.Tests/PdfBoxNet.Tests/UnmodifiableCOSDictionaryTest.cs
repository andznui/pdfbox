/*
 * Converted from Apache PDFBox UnmodifiableCOSDictionaryTest.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using Xunit;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel.font.encoding;

namespace PdfBoxNet.Tests;

/// <summary>
/// Tests for UnmodifiableCOSDictionary
/// </summary>
public class UnmodifiableCOSDictionaryTest
{
    [Fact]
    public void TestUnmodifiableCOSDictionary()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.clear();
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.removeItem(COSName.A);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        COSDictionary cosDictionary = new COSDictionary();
        try
        {
            unmodifiableCOSDictionary.addAll(cosDictionary);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setFlag(COSName.A, 0, true);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setNeedToBeUpdated(true);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetItem()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setItem(COSName.A, COSName.A);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        Encoding standardEncoding = Encoding.getInstance(COSName.STANDARD_ENCODING);
        try
        {
            unmodifiableCOSDictionary.setItem(COSName.A, standardEncoding);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setItem("A", COSName.A);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setItem("A", standardEncoding);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetBoolean()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setBoolean(COSName.A, true);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setBoolean("A", true);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetName()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setName(COSName.A, "A");
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setName("A", "A");
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetDate()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();
        java.util.Calendar calendar = java.util.Calendar.getInstance();

        try
        {
            unmodifiableCOSDictionary.setDate(COSName.A, calendar);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setDate("A", calendar);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetEmbeddedDate()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();
        java.util.Calendar calendar = java.util.Calendar.getInstance();

        try
        {
            unmodifiableCOSDictionary.setEmbeddedDate(COSName.PARAMS, COSName.A, calendar);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetString()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setString(COSName.A, "A");
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setString("A", "A");
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetEmbeddedString()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setEmbeddedString(COSName.PARAMS, COSName.A, "A");
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetInt()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setInt(COSName.A, 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setInt("A", 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetEmbeddedInt()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setEmbeddedInt(COSName.PARAMS, COSName.A, 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetLong()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setLong(COSName.A, 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setLong("A", 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }

    [Fact]
    public void TestSetFloat()
    {
        COSDictionary unmodifiableCOSDictionary = new COSDictionary().asUnmodifiableDictionary();

        try
        {
            unmodifiableCOSDictionary.setFloat(COSName.A, 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }

        try
        {
            unmodifiableCOSDictionary.setFloat("A", 0);
            Assert.Fail("An UnsupportedOperationException should have been thrown");
        }
        catch (java.lang.UnsupportedOperationException)
        {
            // Expected
        }
    }
}
