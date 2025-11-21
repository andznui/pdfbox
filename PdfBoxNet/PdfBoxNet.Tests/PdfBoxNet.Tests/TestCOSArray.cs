/*
 * Converted from Apache PDFBox TestCOSArray.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using System.Collections.Generic;
using Xunit;
using org.apache.pdfbox.cos;

namespace PdfBoxNet.Tests;

/// <summary>
/// Unittests for COSArray
/// </summary>
public class TestCOSArray
{
    [Fact]
    public void TestCreate()
    {
        COSArray cosArray = new COSArray();
        Assert.Equal(0, cosArray.size());
        // In C#/IKVM, this may throw different exception types
        Assert.ThrowsAny<Exception>(() => new COSArray(null));

        cosArray = new COSArray(java.util.Arrays.asList(COSName.A, COSName.B, COSName.C));
        Assert.Equal(3, cosArray.size());
        Assert.Equal(COSName.A, cosArray.get(0));
        Assert.Equal(COSName.B, cosArray.get(1));
        Assert.Equal(COSName.C, cosArray.get(2));
    }

    [Fact]
    public void TestConvertString2COSNameAndBack()
    {
        COSArray cosArray = COSArray.ofCOSNames(
                java.util.Arrays.asList(COSName.A.getName(), COSName.B.getName(), COSName.C.getName()));
        Assert.Equal(3, cosArray.size());
        Assert.Equal(COSName.A, cosArray.get(0));
        Assert.Equal(COSName.B, cosArray.get(1));
        Assert.Equal(COSName.C, cosArray.get(2));

        java.util.List cosNameStringList = cosArray.toCOSNameStringList();
        Assert.Equal(3, cosNameStringList.size());
        Assert.Equal(COSName.A.getName(), cosNameStringList.get(0));
        Assert.Equal(COSName.B.getName(), cosNameStringList.get(1));
        Assert.Equal(COSName.C.getName(), cosNameStringList.get(2));
    }

    [Fact]
    public void TestConvertString2COSStringAndBack()
    {
        COSArray cosArray = COSArray.ofCOSStrings(java.util.Arrays.asList("A", "B", "C"));
        Assert.Equal(3, cosArray.size());
        Assert.Equal("A", cosArray.getString(0));
        Assert.Equal("B", cosArray.getString(1));
        Assert.Equal("C", cosArray.getString(2));

        java.util.List cosStringStringList = cosArray.toCOSStringStringList();
        Assert.Equal(3, cosStringStringList.size());
        Assert.Equal("A", cosStringStringList.get(0));
        Assert.Equal("B", cosStringStringList.get(1));
        Assert.Equal("C", cosStringStringList.get(2));
    }

    [Fact]
    public void TestConvertInteger2COSStringAndBack()
    {
        COSArray cosArray = COSArray.ofCOSIntegers(java.util.Arrays.asList(
            java.lang.Integer.valueOf(1),
            java.lang.Integer.valueOf(2),
            java.lang.Integer.valueOf(3)));
        Assert.Equal(3, cosArray.size());
        Assert.Equal(1, cosArray.getInt(0));
        Assert.Equal(2, cosArray.getInt(1));
        Assert.Equal(3, cosArray.getInt(2));

        java.util.List cosNumberIntegerList = cosArray.toCOSNumberIntegerList();
        Assert.Equal(3, cosNumberIntegerList.size());
        Assert.Equal(1, ((java.lang.Integer)cosNumberIntegerList.get(0)).intValue());
        Assert.Equal(2, ((java.lang.Integer)cosNumberIntegerList.get(1)).intValue());
        Assert.Equal(3, ((java.lang.Integer)cosNumberIntegerList.get(2)).intValue());

        // check arrays with null values
        cosArray = new COSArray(java.util.Arrays.asList(COSInteger.get(1), null, COSInteger.get(3)));
        Assert.Equal(3, cosArray.size());
        Assert.Equal(1, cosArray.getInt(0));
        Assert.Null(cosArray.get(1));
        Assert.Equal(3, cosArray.getInt(2));
        cosNumberIntegerList = cosArray.toCOSNumberIntegerList();
        Assert.Equal(3, cosNumberIntegerList.size());
        Assert.Equal(1, ((java.lang.Integer)cosNumberIntegerList.get(0)).intValue());
        Assert.Null(cosNumberIntegerList.get(1));
        Assert.Equal(3, ((java.lang.Integer)cosNumberIntegerList.get(2)).intValue());
    }

    [Fact]
    public void TestConvertFloat2COSStringAndBack()
    {
        float[] floatArrayStart = { 1f, 0.1f, 0.02f };
        COSArray cosArray = new COSArray();
        cosArray.setFloatArray(floatArrayStart);

        Assert.Equal(3, cosArray.size());
        Assert.Equal(COSFloat.ONE, cosArray.get(0));
        Assert.Equal(new COSFloat(0.1f), cosArray.get(1));
        Assert.Equal(new COSFloat(0.02f), cosArray.get(2));

        java.util.List cosNumberFloatList = cosArray.toCOSNumberFloatList();
        Assert.Equal(3, cosNumberFloatList.size());
        Assert.Equal(1f, ((java.lang.Float)cosNumberFloatList.get(0)).floatValue(), 5);
        Assert.Equal(0.1f, ((java.lang.Float)cosNumberFloatList.get(1)).floatValue(), 5);
        Assert.Equal(0.02f, ((java.lang.Float)cosNumberFloatList.get(2)).floatValue(), 5);

        float[] floatArrayEnd = cosArray.toFloatArray();
        Assert.Equal(1f, ((java.lang.Float)cosNumberFloatList.get(0)).floatValue(), 5);
        Assert.Equal(0.1f, ((java.lang.Float)cosNumberFloatList.get(1)).floatValue(), 5);
        Assert.Equal(0.02f, ((java.lang.Float)cosNumberFloatList.get(2)).floatValue(), 5);
        Assert.Equal(floatArrayStart, floatArrayEnd);

        // check arrays with null values
        cosArray = new COSArray(java.util.Arrays.asList(COSFloat.ONE, null, new COSFloat(0.02f)));
        Assert.Equal(3, cosArray.size());
        Assert.Equal(COSFloat.ONE, cosArray.get(0));
        Assert.Null(cosArray.get(1));
        Assert.Equal(new COSFloat(0.02f), cosArray.get(2));

        cosNumberFloatList = cosArray.toCOSNumberFloatList();
        Assert.Equal(3, cosNumberFloatList.size());
        Assert.Equal(1f, ((java.lang.Float)cosNumberFloatList.get(0)).floatValue(), 5);
        Assert.Null(cosNumberFloatList.get(1));
        Assert.Equal(0.02f, ((java.lang.Float)cosNumberFloatList.get(2)).floatValue(), 5);

        floatArrayEnd = cosArray.toFloatArray();
        // due to the null value the second value of the array is set to 0
        Assert.Equal(new float[] { 1f, 0, 0.02f }, floatArrayEnd);
    }

    [Fact]
    public void TestGetSetName()
    {
        COSArray cosArray = new COSArray();
        cosArray.growToSize(3);
        cosArray.setName(0, "A");
        cosArray.setName(1, "B");
        cosArray.setName(2, "C");
        Assert.Equal(3, cosArray.size());
        Assert.Equal("A", cosArray.getName(0));
        Assert.Equal("B", cosArray.getName(1));
        Assert.Equal("C", cosArray.getName(2));
        Assert.Equal("NULL", cosArray.getName(3, "NULL"));
        Assert.Equal(0, cosArray.indexOf(COSName.A));
        Assert.Equal(1, cosArray.indexOf(COSName.B));
        Assert.Equal(2, cosArray.indexOf(COSName.C));
        Assert.Equal(-1, cosArray.indexOf(COSName.D));
        cosArray.setName(1, "D");
        Assert.Equal(3, cosArray.size());
        Assert.Equal("D", cosArray.getName(1));
    }

    [Fact]
    public void TestGetSetInt()
    {
        COSArray cosArray = new COSArray();
        cosArray.growToSize(3);
        cosArray.setInt(0, 0);
        cosArray.setInt(1, 1);
        cosArray.setInt(2, 2);
        Assert.Equal(3, cosArray.size());
        Assert.Equal(0, cosArray.getInt(0));
        Assert.Equal(1, cosArray.getInt(1));
        Assert.Equal(2, cosArray.getInt(2));
        Assert.Equal(0, cosArray.getInt(3, 0));
        Assert.Equal(0, cosArray.indexOf(COSInteger.get(0)));
        Assert.Equal(1, cosArray.indexOf(COSInteger.get(1)));
        Assert.Equal(2, cosArray.indexOf(COSInteger.get(2)));
        Assert.Equal(-1, cosArray.indexOf(COSInteger.get(3)));
        cosArray.setInt(1, 3);
        Assert.Equal(3, cosArray.size());
        Assert.Equal(3, cosArray.getInt(1));
    }

    [Fact]
    public void TestGetSetString()
    {
        COSArray cosArray = new COSArray();
        cosArray.growToSize(3);
        cosArray.setString(0, "Test1");
        cosArray.setString(1, "Test2");
        cosArray.setString(2, "Test3");
        Assert.Equal(3, cosArray.size());
        Assert.Equal("Test1", cosArray.getString(0));
        Assert.Equal("Test2", cosArray.getString(1));
        Assert.Equal("Test3", cosArray.getString(2));
        Assert.Equal("NULL", cosArray.getString(3, "NULL"));
        Assert.Equal(0, cosArray.indexOf(new COSString("Test1")));
        Assert.Equal(1, cosArray.indexOf(new COSString("Test2")));
        Assert.Equal(2, cosArray.indexOf(new COSString("Test3")));
        Assert.Equal(-1, cosArray.indexOf(new COSString("Test4")));
        cosArray.setString(1, "Test4");
        Assert.Equal(3, cosArray.size());
        Assert.Equal("Test4", cosArray.getString(1));
    }

    [Fact]
    public void TestRemove()
    {
        COSArray cosArray = COSArray.ofCOSIntegers(java.util.Arrays.asList(
            java.lang.Integer.valueOf(1), java.lang.Integer.valueOf(2), java.lang.Integer.valueOf(3),
            java.lang.Integer.valueOf(4), java.lang.Integer.valueOf(5), java.lang.Integer.valueOf(6)));
        cosArray.clear();
        Assert.Equal(0, cosArray.size());

        cosArray = COSArray.ofCOSIntegers(java.util.Arrays.asList(
            java.lang.Integer.valueOf(1), java.lang.Integer.valueOf(2), java.lang.Integer.valueOf(3),
            java.lang.Integer.valueOf(4), java.lang.Integer.valueOf(5), java.lang.Integer.valueOf(6)));
        Assert.Equal(COSInteger.get(3), cosArray.remove(2));
        // 1,2,4,5,6 should be left
        Assert.Equal(5, cosArray.size());
        Assert.Equal(1, cosArray.getInt(0));
        Assert.Equal(4, cosArray.getInt(2));

        // 1,2,4,6 should be left
        Assert.True(cosArray.removeObject(COSInteger.get(5)));
        Assert.Equal(4, cosArray.size());
        Assert.Equal(1, cosArray.getInt(0));
        Assert.Equal(4, cosArray.getInt(2));
        Assert.Equal(6, cosArray.getInt(3));

        cosArray = COSArray.ofCOSIntegers(java.util.Arrays.asList(
            java.lang.Integer.valueOf(1), java.lang.Integer.valueOf(2), java.lang.Integer.valueOf(3),
            java.lang.Integer.valueOf(4), java.lang.Integer.valueOf(5), java.lang.Integer.valueOf(6)));
        cosArray.removeAll(java.util.Arrays.asList(COSInteger.get(3), COSInteger.get(4)));
        // 1,2,5,6 should be left
        Assert.Equal(4, cosArray.size());
        Assert.Equal(2, cosArray.getInt(1));
        Assert.Equal(5, cosArray.getInt(2));

        cosArray = COSArray.ofCOSIntegers(java.util.Arrays.asList(
            java.lang.Integer.valueOf(1), java.lang.Integer.valueOf(2), java.lang.Integer.valueOf(3),
            java.lang.Integer.valueOf(4), java.lang.Integer.valueOf(5), java.lang.Integer.valueOf(6)));
        cosArray.retainAll(java.util.Arrays.asList(COSInteger.get(3), COSInteger.get(4)));
        // 3,4 should be left
        Assert.Equal(2, cosArray.size());
        Assert.Equal(3, cosArray.getInt(0));
        Assert.Equal(4, cosArray.getInt(1));
    }

    [Fact]
    public void TestGrowToSize()
    {
        COSArray cosArray = new COSArray();
        Assert.Equal(0, cosArray.size());
        cosArray.growToSize(2);
        // COSArray has 2 empty elements
        Assert.Equal(2, cosArray.size());
        // size is already 2 -> nothing happens
        cosArray.growToSize(2, COSInteger.get(0));
        Assert.Equal(2, cosArray.size());
        // increase size, fill the new elements with the given value
        cosArray.growToSize(4, COSInteger.get(1));
        Assert.Equal(4, cosArray.size());
        java.util.List cosNumberIntegerList = cosArray.toCOSNumberIntegerList();
        Assert.Equal(4, cosNumberIntegerList.size());
        Assert.Null(cosNumberIntegerList.get(0));
        Assert.Equal(1, ((java.lang.Integer)cosNumberIntegerList.get(2)).intValue());
        Assert.Equal(1, ((java.lang.Integer)cosNumberIntegerList.get(3)).intValue());
    }

    [Fact]
    public void TestToList()
    {
        COSArray cosArray = COSArray.ofCOSIntegers(java.util.Arrays.asList(
            java.lang.Integer.valueOf(0), java.lang.Integer.valueOf(1), java.lang.Integer.valueOf(2),
            java.lang.Integer.valueOf(3), java.lang.Integer.valueOf(4), java.lang.Integer.valueOf(5)));
        java.util.List list = cosArray.toList();
        Assert.Equal(6, list.size());
        Assert.Equal(COSInteger.get(0), list.get(0));
        Assert.Equal(COSInteger.get(5), list.get(5));
    }
}
