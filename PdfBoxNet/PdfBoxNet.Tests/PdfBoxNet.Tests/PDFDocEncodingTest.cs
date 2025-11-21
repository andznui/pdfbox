/*
 * Converted from Apache PDFBox PDFDocEncodingTest.java
 * Original: Licensed to the Apache Software Foundation (ASF) under Apache License 2.0
 */

using System;
using System.Collections.Generic;
using Xunit;
using org.apache.pdfbox.cos;

namespace PdfBoxNet.Tests;

/// <summary>
/// Test for PDFDocEncoding
/// </summary>
public class PDFDocEncodingTest
{
    private static readonly java.util.List deviations = new java.util.ArrayList();

    static PDFDocEncodingTest()
    {
        // all deviations (based on the table in ISO 32000-1:2008)
        // block 1
        deviations.add(((char)0x02D8).ToString()); // BREVE
        deviations.add(((char)0x02C7).ToString()); // CARON
        deviations.add(((char)0x02C6).ToString()); // MODIFIER LETTER CIRCUMFLEX ACCENT
        deviations.add(((char)0x02D9).ToString()); // DOT ABOVE
        deviations.add(((char)0x02DD).ToString()); // DOUBLE ACUTE ACCENT
        deviations.add(((char)0x02DB).ToString()); // OGONEK
        deviations.add(((char)0x02DA).ToString()); // RING ABOVE
        deviations.add(((char)0x02DC).ToString()); // SMALL TILDE
        // block 2
        deviations.add(((char)0x2022).ToString()); // BULLET
        deviations.add(((char)0x2020).ToString()); // DAGGER
        deviations.add(((char)0x2021).ToString()); // DOUBLE DAGGER
        deviations.add(((char)0x2026).ToString()); // HORIZONTAL ELLIPSIS
        deviations.add(((char)0x2014).ToString()); // EM DASH
        deviations.add(((char)0x2013).ToString()); // EN DASH
        deviations.add(((char)0x0192).ToString()); // LATIN SMALL LETTER SCRIPT F
        deviations.add(((char)0x2044).ToString()); // FRACTION SLASH (solidus)
        deviations.add(((char)0x2039).ToString()); // SINGLE LEFT-POINTING ANGLE QUOTATION MARK
        deviations.add(((char)0x203A).ToString()); // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK
        deviations.add(((char)0x2212).ToString()); // MINUS SIGN
        deviations.add(((char)0x2030).ToString()); // PER MILLE SIGN
        deviations.add(((char)0x201E).ToString()); // DOUBLE LOW-9 QUOTATION MARK (quotedblbase)
        deviations.add(((char)0x201C).ToString()); // LEFT DOUBLE QUOTATION MARK (quotedblleft)
        deviations.add(((char)0x201D).ToString()); // RIGHT DOUBLE QUOTATION MARK (quotedblright)
        deviations.add(((char)0x2018).ToString()); // LEFT SINGLE QUOTATION MARK (quoteleft)
        deviations.add(((char)0x2019).ToString()); // RIGHT SINGLE QUOTATION MARK (quoteright)
        deviations.add(((char)0x201A).ToString()); // SINGLE LOW-9 QUOTATION MARK (quotesinglbase)
        deviations.add(((char)0x2122).ToString()); // TRADE MARK SIGN
        deviations.add(((char)0xFB01).ToString()); // LATIN SMALL LIGATURE FI
        deviations.add(((char)0xFB02).ToString()); // LATIN SMALL LIGATURE FL
        deviations.add(((char)0x0141).ToString()); // LATIN CAPITAL LETTER L WITH STROKE
        deviations.add(((char)0x0152).ToString()); // LATIN CAPITAL LIGATURE OE
        deviations.add(((char)0x0160).ToString()); // LATIN CAPITAL LETTER S WITH CARON
        deviations.add(((char)0x0178).ToString()); // LATIN CAPITAL LETTER Y WITH DIAERESIS
        deviations.add(((char)0x017D).ToString()); // LATIN CAPITAL LETTER Z WITH CARON
        deviations.add(((char)0x0131).ToString()); // LATIN SMALL LETTER DOTLESS I
        deviations.add(((char)0x0142).ToString()); // LATIN SMALL LETTER L WITH STROKE
        deviations.add(((char)0x0153).ToString()); // LATIN SMALL LIGATURE OE
        deviations.add(((char)0x0161).ToString()); // LATIN SMALL LETTER S WITH CARON
        deviations.add(((char)0x017E).ToString()); // LATIN SMALL LETTER Z WITH CARON
        deviations.add(((char)0x20AC).ToString()); // EURO SIGN
        // end of deviations
    }

    [Fact]
    public void TestDeviations()
    {
        java.util.Iterator iterator = deviations.iterator();
        while (iterator.hasNext())
        {
            string deviation = (string)iterator.next();
            COSString cosString = new COSString(deviation);
            Assert.Equal(deviation, cosString.getString());
        }
    }

    /// <summary>
    /// PDFBOX-3864: Test that chars smaller than 256 which are NOT part of PDFDocEncoding are
    /// handled correctly.
    /// </summary>
    [Fact]
    public void TestPDFBox3864()
    {
        try
        {
            for (int i = 0; i < 256; i++)
            {
                string hex = string.Format("FEFF{0:X4}", i);
                COSString cs1 = COSString.parseHex(hex);
                COSString cs2 = new COSString(cs1.getString());
                Assert.Equal(cs1, cs2);
            }
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }
}
