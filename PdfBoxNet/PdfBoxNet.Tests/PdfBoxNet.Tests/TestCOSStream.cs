/*
 * Converted from Apache PDFBox TestCOSStream.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using Xunit;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.filter;

namespace PdfBoxNet.Tests;

/// <summary>
/// Tests for COSStream
/// </summary>
public class TestCOSStream
{
    /// <summary>
    /// Tests encoding of a stream without any filter applied.
    /// </summary>
    [Fact]
    public void TestUncompressedStreamEncode()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        COSStream stream = CreateStream(testString, null);
        ValidateEncoded(stream, testString);
    }

    /// <summary>
    /// Tests decoding of a stream without any filter applied.
    /// </summary>
    [Fact]
    public void TestUncompressedStreamDecode()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        COSStream stream = CreateStream(testString, null);
        ValidateDecoded(stream, testString);
    }

    /// <summary>
    /// Tests encoding of a stream with one filter applied.
    /// </summary>
    [Fact]
    public void TestCompressedStream1Encode()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        byte[] testStringEncoded = EncodeData(testString, COSName.FLATE_DECODE);
        COSStream stream = CreateStream(testString, COSName.FLATE_DECODE);
        ValidateEncoded(stream, testStringEncoded);
    }

    /// <summary>
    /// Tests decoding of a stream with one filter applied.
    /// </summary>
    [Fact]
    public void TestCompressedStream1Decode()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        byte[] testStringEncoded = EncodeData(testString, COSName.FLATE_DECODE);
        COSStream stream = new COSStream();

        using (java.io.OutputStream output = stream.createRawOutputStream())
        {
            output.write(testStringEncoded);
        }

        stream.setItem(COSName.FILTER, COSName.FLATE_DECODE);
        ValidateDecoded(stream, testString);
    }

    /// <summary>
    /// Tests encoding of a stream with 2 filters applied.
    /// </summary>
    [Fact]
    public void TestCompressedStream2Encode()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        byte[] testStringEncoded = EncodeData(testString, COSName.FLATE_DECODE);
        testStringEncoded = EncodeData(testStringEncoded, COSName.ASCII85_DECODE);

        COSArray filters = new COSArray();
        filters.add(COSName.ASCII85_DECODE);
        filters.add(COSName.FLATE_DECODE);

        COSStream stream = CreateStream(testString, filters);
        ValidateEncoded(stream, testStringEncoded);
    }

    /// <summary>
    /// Tests decoding of a stream with 2 filters applied.
    /// </summary>
    [Fact]
    public void TestCompressedStream2Decode()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        byte[] testStringEncoded = EncodeData(testString, COSName.FLATE_DECODE);
        testStringEncoded = EncodeData(testStringEncoded, COSName.ASCII85_DECODE);
        COSStream stream = new COSStream();

        COSArray filters = new COSArray();
        filters.add(COSName.ASCII85_DECODE);
        filters.add(COSName.FLATE_DECODE);
        stream.setItem(COSName.FILTER, filters);

        using (java.io.OutputStream output = stream.createRawOutputStream())
        {
            output.write(testStringEncoded);
        }

        ValidateDecoded(stream, testString);
    }

    /// <summary>
    /// Tests tests that encoding is done correctly even if the stream is closed twice.
    /// Closeable.close() allows streams to be closed multiple times. The second and subsequent
    /// close() calls should have no effect.
    /// </summary>
    [Fact]
    public void TestCompressedStreamDoubleClose()
    {
        byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
        byte[] testStringEncoded = EncodeData(testString, COSName.FLATE_DECODE);
        COSStream stream = new COSStream();
        java.io.OutputStream output = stream.createOutputStream(COSName.FLATE_DECODE);
        output.write(testString);
        output.close();
        output.close();
        ValidateEncoded(stream, testStringEncoded);
    }

    [Fact]
    public void TestHasStreamData()
    {
        using (COSStream stream = new COSStream())
        {
            Assert.False(stream.hasData());
            Assert.Throws<java.io.IOException>(() => stream.createInputStream());

            byte[] testString = java.nio.charset.StandardCharsets.US_ASCII.encode("This is a test string to be used as input for TestCOSStream").array();
            using (java.io.OutputStream output = stream.createOutputStream())
            {
                output.write(testString);
            }
            Assert.True(stream.hasData());
        }
    }

    private byte[] EncodeData(byte[] original, COSName filter)
    {
        Filter encodingFilter = FilterFactory.INSTANCE.getFilter(filter);
        java.io.ByteArrayOutputStream encoded = new java.io.ByteArrayOutputStream();
        encodingFilter.encode(new java.io.ByteArrayInputStream(original), encoded, new COSDictionary(), 0);
        return encoded.toByteArray();
    }

    private COSStream CreateStream(byte[] testString, COSBase filters)
    {
        COSStream stream = new COSStream();
        using (java.io.OutputStream output = stream.createOutputStream(filters))
        {
            output.write(testString);
        }
        return stream;
    }

    private void ValidateEncoded(COSStream stream, byte[] expected)
    {
        using (stream)
        {
            java.io.InputStream inStream = stream.createRawInputStream();
            var baos = new java.io.ByteArrayOutputStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = inStream.read(buffer)) != -1)
            {
                baos.write(buffer, 0, bytesRead);
            }
            byte[] decoded = baos.toByteArray();
            Assert.True(java.util.Arrays.equals(expected, decoded));
        }
    }

    private void ValidateDecoded(COSStream stream, byte[] expected)
    {
        using (stream)
        {
            java.io.InputStream inStream = stream.createInputStream();
            var baos = new java.io.ByteArrayOutputStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = inStream.read(buffer)) != -1)
            {
                baos.write(buffer, 0, bytesRead);
            }
            byte[] encoded = baos.toByteArray();
            Assert.True(java.util.Arrays.equals(expected, encoded));
        }
    }
}
