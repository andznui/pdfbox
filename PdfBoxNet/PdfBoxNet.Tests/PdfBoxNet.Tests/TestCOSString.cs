/*
 * Converted from Apache PDFBox TestCOSString.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 * This C# conversion demonstrates IKVM.NET wrapper functionality
 */

using System;
using System.Linq;
using System.Text;
using Xunit;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdfwriter;

namespace PdfBoxNet.Tests;

/// <summary>
/// This will test all of the filters in the PDFBox system
/// Converted from org.apache.pdfbox.cos.TestCOSString
/// </summary>
public class TestCOSString : TestCOSBase
{
    private const string ESC_CHAR_STRING = "( test#some) escaped< \\chars>!~1239857 ";
    private const string ESC_CHAR_STRING_PDF_FORMAT = "\\( test#some\\) escaped< \\\\chars>!~1239857 ";

    public TestCOSString()
    {
        testCOSBase = new COSString("test cos string");
    }

    /// <summary>
    /// Test setForceHexForm() and setForceLiteralForm() - tests these two methods do enforce the
    /// different String output forms within PDF.
    /// </summary>
    [Fact]
    public void TestSetForceHexLiteralForm()
    {
        string inputString = "Test with a text and a few numbers 1, 2 and 3";
        string pdfHex = "<" + CreateHex(inputString) + ">";
        COSString cosStr = new COSString(inputString);
        cosStr.setForceHexForm(true);
        WritePDFTests(pdfHex, cosStr);

        COSString escStr = new COSString(ESC_CHAR_STRING);
        WritePDFTests("(" + ESC_CHAR_STRING_PDF_FORMAT + ")", escStr);
        COSString escStrHex = new COSString(ESC_CHAR_STRING);
        escStrHex.setForceHexForm(true);
        // Escape characters not escaped in hex version
        WritePDFTests("<" + CreateHex(ESC_CHAR_STRING) + ">", escStrHex);
    }

    /// <summary>
    /// Helper method for testing writePDF().
    /// </summary>
    /// <param name="expected">the String expected when writePDF() is invoked</param>
    /// <param name="testSubj">the test subject</param>
    private void WritePDFTests(string expected, COSString testSubj)
    {
        var outStream = new java.io.ByteArrayOutputStream();
        try
        {
            COSWriter.writeString(testSubj, outStream);
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException: " + e.getMessage());
        }
        Assert.Equal(expected, outStream.toString());
    }

    /// <summary>
    /// Test parseHex() - tests that the proper String is created from a hex string input.
    /// </summary>
    [Fact]
    public void TestFromHex()
    {
        string expected = "Quick and simple test";
        string hexForm = CreateHex(expected);
        try
        {
            COSString test1 = COSString.parseHex(hexForm);
            WritePDFTests("(" + expected + ")", test1);
            COSString test2 = COSString.parseHex(CreateHex(ESC_CHAR_STRING));
            WritePDFTests("(" + ESC_CHAR_STRING_PDF_FORMAT + ")", test2);
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
        Assert.Throws<java.io.IOException>(() => COSString.parseHex(hexForm + "xx"));
    }

    private string CreateHex(string str)
    {
        var sb = new StringBuilder();
        foreach (char c in str)
        {
            sb.Append(Convert.ToInt32(c).ToString("X"));
        }
        return sb.ToString();
    }

    /// <summary>
    /// Tests getHex() - ensure the hex String returned is properly formatted.
    /// </summary>
    [Fact]
    public void TestGetHex()
    {
        string expected = "Test subject for testing getHex";
        COSString test1 = new COSString(expected);
        string hexForm = CreateHex(expected);
        Assert.Equal(hexForm, test1.toHexString());
        COSString escCS = new COSString(ESC_CHAR_STRING);
        // Not sure whether the escaped characters should be escaped or not, presumably since
        // writePDF() gives you the proper formatted text, getHex() should ONLY convert to hex.
        Assert.Equal(CreateHex(ESC_CHAR_STRING), escCS.toHexString());
    }

    /// <summary>
    /// Test testGetString() - ensure getString() are returned in the correct format.
    /// </summary>
    [Fact]
    public void TestGetString()
    {
        try
        {
            string testStr = "Test subject for getString()";
            COSString test1 = new COSString(testStr);
            Assert.Equal(testStr, test1.getString());

            COSString hexStr = COSString.parseHex(CreateHex(testStr));
            Assert.Equal(testStr, hexStr.getString());

            COSString escapedString = new COSString(ESC_CHAR_STRING);
            Assert.Equal(ESC_CHAR_STRING, escapedString.getString());

            testStr = "Line1\nLine2\nLine3\n";
            COSString lineFeedString = new COSString(testStr);
            Assert.Equal(testStr, lineFeedString.getString());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    /// <summary>
    /// Test getBytes() - again not much to test, just ensure the proper byte array is returned.
    /// </summary>
    [Fact]
    public void TestGetBytes()
    {
        COSString str = new COSString(ESC_CHAR_STRING);
        testByteArrays(Encoding.GetEncoding("ISO-8859-1").GetBytes(ESC_CHAR_STRING), str.getBytes());
    }

    /// <summary>
    /// Tests writePDF() - tests that the string is in PDF format.
    /// </summary>
    [Fact]
    public void TestWritePDF()
    {
        // This has been tested quite thorougly above but do a couple tests anyway
        COSString testSubj = new COSString(ESC_CHAR_STRING);
        WritePDFTests("(" + ESC_CHAR_STRING_PDF_FORMAT + ")", testSubj);
        string textString = "This is just an arbitrary piece of text for testing";
        COSString testSubj2 = new COSString(textString);
        WritePDFTests("(" + textString + ")", testSubj2);
    }

    /// <summary>
    /// This will test all of the filters in the system.
    /// </summary>
    [Fact]
    public void TestUnicode()
    {
        try
        {
            string theString = "\u4e16";
            COSString cosString = new COSString(theString);
            Assert.Equal(theString, cosString.getString());

            string textAscii = "This is some regular text. It should all be expressible in ASCII";
            // En français où les choses sont accentués. En español, así
            string text8Bit = "En fran\u00e7ais o\u00f9 les choses sont accentu\u00e9s. En espa\u00f1ol, as\u00ed";
            // をクリックしてく
            string textHighBits = "\u3092\u30af\u30ea\u30c3\u30af\u3057\u3066\u304f";

            // Testing the getString method
            COSString stringAscii = new COSString(textAscii);
            Assert.Equal(textAscii, stringAscii.getString());

            COSString string8Bit = new COSString(text8Bit);
            Assert.Equal(text8Bit, string8Bit.getString());

            COSString stringHighBits = new COSString(textHighBits);
            Assert.Equal(textHighBits, stringHighBits.getString());

            // Testing the getBytes method
            // The first two strings should be stored as ISO-8859-1 because they only contain chars in the range 0..255
            Assert.Equal(textAscii, Encoding.GetEncoding("ISO-8859-1").GetString(stringAscii.getBytes()));
            // likewise for the 8bit characters.
            Assert.Equal(text8Bit, Encoding.GetEncoding("ISO-8859-1").GetString(string8Bit.getBytes()));

            // The japanese text contains high bits so must be stored as big endian UTF-16
            // getBytes() includes the BOM (U+FEFF), so we need to skip it
            byte[] highBitsBytes = stringHighBits.getBytes();
            string decoded = Encoding.BigEndianUnicode.GetString(highBitsBytes);
            // Remove BOM if present
            if (decoded.Length > 0 && decoded[0] == '\uFEFF')
            {
                decoded = decoded.Substring(1);
            }
            Assert.Equal(textHighBits, decoded);

            // Test the writePDF method to ensure that the Strings are correct when written into PDF.
            var outStream = new java.io.ByteArrayOutputStream();
            COSWriter.writeString(stringAscii, outStream);
            Assert.Equal("(" + textAscii + ")", Encoding.ASCII.GetString(outStream.toByteArray()));

            outStream.reset();
            COSWriter.writeString(string8Bit, outStream);
            var hex = new StringBuilder();
            foreach (char c in text8Bit)
            {
                hex.Append(Convert.ToInt32(c).ToString("X"));
            }
            Assert.Equal("<" + hex + ">", Encoding.ASCII.GetString(outStream.toByteArray()));

            outStream.reset();
            COSWriter.writeString(stringHighBits, outStream);
            hex = new StringBuilder();
            hex.Append("FEFF"); // Byte Order Mark
            foreach (char c in textHighBits)
            {
                hex.Append(Convert.ToInt32(c).ToString("X"));
            }
            Assert.Equal("<" + hex + ">", Encoding.ASCII.GetString(outStream.toByteArray()));
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public override void TestAccept()
    {
        try
        {
            var outStream = new java.io.ByteArrayOutputStream();
            ICOSVisitor visitor = new COSWriter(outStream);
            COSString testSubj = new COSString(ESC_CHAR_STRING);
            testSubj.accept(visitor);
            Assert.Equal("(" + ESC_CHAR_STRING_PDF_FORMAT + ")", outStream.toString());
            outStream.reset();
            COSString testSubjHex = new COSString(ESC_CHAR_STRING);
            testSubjHex.setForceHexForm(true);
            testSubjHex.accept(visitor);
            Assert.Equal("<" + CreateHex(ESC_CHAR_STRING) + ">", outStream.toString());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    /// <summary>
    /// Tests equals(Object) - ensure that the Object.equals() contract is obeyed.
    /// </summary>
    [Fact]
    public void TestEquals()
    {
        // Check all these several times for consistency
        for (int i = 0; i < 10; i++)
        {
            // Reflexive
            COSString x1 = new COSString("Test");
            Assert.Equal(x1, x1);

            // Symmetry i.e. if x == y then y == x
            COSString y1 = new COSString("Test");
            Assert.Equal(x1, y1);
            Assert.Equal(y1, x1);
            COSString x2 = new COSString("Test");
            x2.setForceHexForm(true);
            // also if x != y then y != x
            Assert.NotEqual(x1, x2);
            Assert.NotEqual(x2, x1);

            // Transitive if x == y && y == z then x == z
            COSString z1 = new COSString("Test");
            Assert.Equal(x1, y1);
            Assert.Equal(y1, z1);
            Assert.Equal(x1, z1);
            // Test the negative as well if x1 == y1 && y1 != x2 then x1 != x2
            Assert.Equal(x1, y1);
            Assert.NotEqual(y1, x2);
            Assert.NotEqual(x1, x2);
        }
    }

    /// <summary>
    /// Test hashCode() - tests that the Object.hashCode() contract is obeyed.
    /// </summary>
    [Fact]
    public void TestHashCode()
    {
        COSString str1 = new COSString("Test1");
        COSString str2 = new COSString("Test2");
        Assert.NotEqual(str1.hashCode(), str2.hashCode());
        COSString str3 = new COSString("Test1");
        Assert.Equal(str1.hashCode(), str3.hashCode());
        COSString str3Hex = new COSString("Test1");
        str3Hex.setForceHexForm(true);
        Assert.NotEqual(str1.hashCode(), str3Hex.hashCode());
    }

    /// <summary>
    /// Test testCompareFromHexString() - tests that Strings created from hex
    /// compare correctly (PDFBOX-2401)
    /// </summary>
    [Fact]
    public void TestCompareFromHexString()
    {
        try
        {
            COSString test1 = COSString.parseHex("000000FF000000");
            COSString test2 = COSString.parseHex("000000FF00FFFF");
            Assert.Equal(test1, test1);
            Assert.Equal(test2, test2);
            Assert.NotEqual(test1.toHexString(), test2.toHexString());
            Assert.False(test1.getBytes().SequenceEqual(test2.getBytes()));
            Assert.NotEqual(test1, test2);
            Assert.NotEqual(test2, test1);
            Assert.NotEqual(test1.getString(), test2.getString());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    /// <summary>
    /// PDFBOX-3881: Test that if String has only the BOM, that it be an empty string.
    /// </summary>
    [Fact]
    public void TestEmptyStringWithBOM()
    {
        try
        {
            Assert.True(string.IsNullOrEmpty(COSString.parseHex("FEFF").getString()));
            Assert.True(string.IsNullOrEmpty(COSString.parseHex("FFFE").getString()));
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }
}
