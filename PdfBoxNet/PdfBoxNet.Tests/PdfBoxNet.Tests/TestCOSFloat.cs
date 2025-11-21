/*
 * Converted from Apache PDFBox TestCOSFloat.java
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
/// Tests COSFloat
/// Converted from org.apache.pdfbox.cos.TestCOSFloat
/// </summary>
public class TestCOSFloat : TestCOSNumber
{
    public TestCOSFloat()
    {
        try
        {
            testCOSBase = COSNumber.get("1.1");
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("Failed to create a COSNumber in setUp()");
        }
    }

    /// <summary>
    /// Base class to run looped tests with float numbers.
    /// To use it, derive a class and just implement runTest(). Then either call
    /// runTests for a series of random and pseudorandom tests, or runTest to
    /// test with corner values.
    /// </summary>
    abstract class BaseTester
    {
        private int low = -100000;
        private int high = 300000;
        private int step = 20000;

        public void SetLoop(int low, int high, int step)
        {
            this.low = low;
            this.high = high;
            this.step = step;
        }

        // deterministic and non-deterministic test
        public void RunTests()
        {
            // deterministic test
            Loop(123456);

            // non-deterministic test
            Loop(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
        }

        // look through a series of pseudorandom tests influenced by a seed
        private void Loop(long seed)
        {
            var rnd = new java.util.Random(seed);
            for (int i = low; i < high; i += step)
            {
                float num = i * rnd.nextFloat();
                try
                {
                    RunTest(num);
                }
                catch (Exception a)
                {
                    Assert.Fail("num = " + num + ", seed = " + seed + ", message: " + a.Message);
                }
            }
        }

        public abstract void RunTest(float num);
    }

    /// <summary>
    /// Tests equals() - ensures that the Object.equals() contract is obeyed.
    /// These are tested over a range of arbitrary values to ensure Consistency,
    /// Reflexivity, Symmetry, Transitivity and non-nullity.
    /// </summary>
    [Fact]
    public void TestEquals()
    {
        new EqualsTester().RunTests();
    }

    class EqualsTester : BaseTester
    {
        public override void RunTest(float num)
        {
            COSFloat test1 = new COSFloat(num);
            COSFloat test2 = new COSFloat(num);
            COSFloat test3 = new COSFloat(num);
            // Reflexive (x == x)
            Assert.Equal(test1, test1);
            // Symmetric is preserved ( x==y then y==x)
            Assert.Equal(test2, test3);
            Assert.Equal(test3, test2);
            // Transitive (if x==y && y==z then x==z)
            Assert.Equal(test1, test2);
            Assert.Equal(test2, test3);
            Assert.Equal(test1, test3);

            float nf = java.lang.Float.intBitsToFloat(java.lang.Float.floatToIntBits(num) + 1);
            COSFloat test4 = new COSFloat(nf);
            Assert.NotEqual(test4, test1);
        }
    }

    /// <summary>
    /// Tests hashCode() - ensures that the Object.hashCode() contract is obeyed
    /// over a range of arbitrary values.
    /// </summary>
    [Fact]
    public void TestHashCode()
    {
        new HashCodeTester().RunTests();
    }

    class HashCodeTester : BaseTester
    {
        public override void RunTest(float num)
        {
            COSFloat test1 = new COSFloat(num);
            COSFloat test2 = new COSFloat(num);
            Assert.Equal(test1.hashCode(), test2.hashCode());

            float nf = java.lang.Float.intBitsToFloat(java.lang.Float.floatToIntBits(num) + 1);
            COSFloat test3 = new COSFloat(nf);
            Assert.NotSame(test3.hashCode(), test1.hashCode());
        }
    }

    [Fact]
    public override void TestFloatValue()
    {
        new FloatValueTester().RunTests();
    }

    class FloatValueTester : BaseTester
    {
        public override void RunTest(float num)
        {
            COSFloat testFloat = new COSFloat(num);
            Assert.Equal(num, testFloat.floatValue());
        }
    }

    [Fact]
    public override void TestIntValue()
    {
        new IntValueTester().RunTests();
    }

    class IntValueTester : BaseTester
    {
        public override void RunTest(float num)
        {
            COSFloat testFloat = new COSFloat(num);
            Assert.Equal((int)num, testFloat.intValue());
        }
    }

    [Fact]
    public override void TestLongValue()
    {
        new LongValueTester().RunTests();
    }

    class LongValueTester : BaseTester
    {
        public override void RunTest(float num)
        {
            COSFloat testFloat = new COSFloat(num);
            Assert.Equal((long)num, testFloat.longValue());
        }
    }

    [Fact]
    public override void TestAccept()
    {
        new AcceptTester().RunTests();
    }

    class AcceptTester : BaseTester
    {
        private readonly java.io.ByteArrayOutputStream outStream = new java.io.ByteArrayOutputStream();
        private readonly COSWriter visitor;

        public AcceptTester()
        {
            visitor = new COSWriter(outStream);
        }

        public override void RunTest(float num)
        {
            try
            {
                COSFloat cosFloat = new COSFloat(num);
                cosFloat.accept(visitor);
                string expected = FloatToString(cosFloat.floatValue());
                Assert.Equal(expected, Encoding.GetEncoding("ISO-8859-1").GetString(outStream.toByteArray()));
                outStream.reset();
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("Failed to write " + num + " exception: " + e.getMessage());
            }
        }

        private string FloatToString(float value)
        {
            // use a BigDecimal as intermediate state to avoid
            // a floating point string representation of the float value
            // Use Java's String.valueOf() to match Java behavior
            string javaString = java.lang.String.valueOf(value);
            return RemoveTrailingNull(new java.math.BigDecimal(javaString).toPlainString());
        }

        private string RemoveTrailingNull(string value)
        {
            // remove fraction digit "0" only
            if (value.IndexOf('.') > -1 && !value.EndsWith(".0"))
            {
                while (value.EndsWith("0") && !value.EndsWith(".0"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }
            return value;
        }
    }

    /// <summary>
    /// Tests writePDF() - this method takes an OutputStream and writes
    /// this object to it.
    /// </summary>
    [Fact]
    public void TestWritePDF()
    {
        WritePDFTester writePDFTester = new WritePDFTester();
        writePDFTester.RunTests();

        // test a corner case as described in PDFBOX-1778
        writePDFTester.RunTest(0.000000000000000000000000000000001f);
    }

    class WritePDFTester : BaseTester
    {
        private readonly java.io.ByteArrayOutputStream outStream = new java.io.ByteArrayOutputStream();

        public WritePDFTester()
        {
            SetLoop(-1000, 3000, 200);
        }

        public override void RunTest(float num)
        {
            try
            {
                COSFloat cosFloat = new COSFloat(num);
                cosFloat.writePDF(outStream);

                string expected = FloatToString(cosFloat.floatValue());
                Assert.Equal(expected, Encoding.GetEncoding("ISO-8859-1").GetString(outStream.toByteArray()));
                Assert.Equal("COSFloat{" + expected + "}", cosFloat.toString());

                outStream.reset();
            }
            catch (java.io.IOException e)
            {
                Assert.Fail("Failed to write " + num + " exception: " + e.getMessage());
            }
        }

        private string FloatToString(float value)
        {
            // use a BigDecimal as intermediate state to avoid
            // a floating point string representation of the float value
            // Use Java's String.valueOf() to match Java behavior
            string javaString = java.lang.String.valueOf(value);
            return RemoveTrailingNull(new java.math.BigDecimal(javaString).toPlainString());
        }

        private string RemoveTrailingNull(string value)
        {
            // remove fraction digit "0" only
            if (value.IndexOf('.') > -1 && !value.EndsWith(".0"))
            {
                while (value.EndsWith("0") && !value.EndsWith(".0"))
                {
                    value = value.Substring(0, value.Length - 1);
                }
            }
            return value;
        }
    }

    [Fact]
    public void TestDoubleNegative()
    {
        try
        {
            // PDFBOX-4289
            COSFloat cosFloat = new COSFloat("--16.33");
            Assert.Equal(-16.33f, cosFloat.floatValue(), 2);
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public void TestVerySmallValues()
    {
        try
        {
            double smallValue = java.lang.Float.MIN_VALUE / 10d;

            Assert.Equal(-1, java.lang.Double.compare(smallValue, java.lang.Float.MIN_VALUE));

            // 1.4012984643248171E-46
            string asString = smallValue.ToString();
            COSFloat cosFloat = new COSFloat(asString);
            Assert.Equal(0.0f, cosFloat.floatValue());

            // 0.00000000000000000000000000000000000000000000014012984643248171
            asString = new java.math.BigDecimal(asString).toPlainString();
            cosFloat = new COSFloat(asString);
            Assert.Equal(0.0f, cosFloat.floatValue());

            smallValue *= -1;

            // -1.4012984643248171E-46
            asString = smallValue.ToString();
            cosFloat = new COSFloat(asString);
            Assert.Equal(0.0f, cosFloat.floatValue());

            // -0.00000000000000000000000000000000000000000000014012984643248171
            asString = new java.math.BigDecimal(asString).toPlainString();
            cosFloat = new COSFloat(asString);
            Assert.Equal(0.0f, cosFloat.floatValue());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public void TestVeryLargeValues()
    {
        try
        {
            double largeValue = java.lang.Float.MAX_VALUE * 10d;

            Assert.Equal(1, java.lang.Double.compare(largeValue, java.lang.Float.MAX_VALUE));

            // 1.4012984643248171E-46
            string asString = largeValue.ToString();
            COSFloat cosFloat = new COSFloat(asString);
            Assert.Equal(java.lang.Float.MAX_VALUE, cosFloat.floatValue());

            // 0.00000000000000000000000000000000000000000000014012984643248171
            asString = new java.math.BigDecimal(asString).toPlainString();
            cosFloat = new COSFloat(asString);
            Assert.Equal(java.lang.Float.MAX_VALUE, cosFloat.floatValue());

            largeValue *= -1;

            // -1.4012984643248171E-46
            asString = largeValue.ToString();
            cosFloat = new COSFloat(asString);
            Assert.Equal(-java.lang.Float.MAX_VALUE, cosFloat.floatValue());

            // -0.00000000000000000000000000000000000000000000014012984643248171
            asString = new java.math.BigDecimal(asString).toPlainString();
            cosFloat = new COSFloat(asString);
            Assert.Equal(-java.lang.Float.MAX_VALUE, cosFloat.floatValue());
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public void TestMisplacedNegative()
    {
        try
        {
            // PDFBOX-2990, PDFBOX-3369 have 0.00000-33917698
            // PDFBOX-3500 has 0.-262
            COSFloat cosFloat = new COSFloat("0.00000-33917698");
            Assert.Equal(new COSFloat("-0.0000033917698"), cosFloat);

            cosFloat = new COSFloat("0.-262");
            Assert.Equal(new COSFloat("-0.262"), cosFloat);

            cosFloat = new COSFloat("-0.-262");
            Assert.Equal(new COSFloat("-0.262"), cosFloat);

            cosFloat = new COSFloat("-12.-1");
            Assert.Equal(new COSFloat("-12.1"), cosFloat);
        }
        catch (java.io.IOException e)
        {
            Assert.Fail("IOException thrown: " + e.getMessage());
        }
    }

    [Fact]
    public void TestDuplicateMisplacedNegative()
    {
        Assert.Throws<java.io.IOException>(() => new COSFloat("0.-26-2"));
        Assert.Throws<java.io.IOException>(() => new COSFloat("---0.262"));
        Assert.Throws<java.io.IOException>(() => new COSFloat("--0.2-62"));
    }

    [Fact]
    public void TestStubOperatorMinMaxValues()
    {
        float largeValue = 32768f;
        float largeNegativeValue = -32768f;

        Assert.Equal(largeValue, new COSFloat(largeValue).floatValue());
        Assert.Equal(largeNegativeValue, new COSFloat(largeNegativeValue).floatValue());
    }
}
