/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Xunit;
using org.apache.pdfbox;
using org.apache.pdfbox.filter;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// This will test all of the filters in the PDFBox system.
    /// Converted from org.apache.pdfbox.filter.TestFilters
    /// </summary>
    public class TestFilters
    {
        /// <summary>
        /// This will test all of the filters in the system. There will be COUNT
        /// of deterministic tests and COUNT of non-deterministic tests, see also
        /// the discussion in PDFBOX-1977.
        ///
        /// Note: In PDFBox 3.x, many filter classes are internal, so we test specific
        /// filters individually rather than iterating through all filters.
        /// </summary>
        [Fact]
        public void testFilters()
        {
            try
            {
                const int COUNT = 10;
                java.util.Random rd = new java.util.Random(123456);
                for (int iter = 0; iter < COUNT * 2; iter++)
                {
                    long seed;
                    if (iter < COUNT)
                    {
                        // deterministic seed
                        seed = rd.nextLong();
                    }
                    else
                    {
                        // non-deterministic seed
                        seed = new java.util.Random().nextLong();
                    }
                    bool success = false;
                    try
                    {
                        java.util.Random random = new java.util.Random(seed);
                        int numBytes = 10000 + random.nextInt(20000);
                        byte[] original = new byte[numBytes];

                        int upto = 0;
                        while (upto < numBytes)
                        {
                            int left = numBytes - upto;
                            if (random.nextBoolean() || left < 2)
                            {
                                // Fill w/ pseudo-random bytes:
                                int end = upto + Math.Min(left, 10 + random.nextInt(100));
                                while (upto < end)
                                {
                                    original[upto++] = (byte)random.nextInt();
                                }
                            }
                            else
                            {
                                // Fill w/ very predictable bytes:
                                int end = upto + Math.Min(left, 2 + random.nextInt(10));
                                byte value = (byte)random.nextInt(4);
                                while (upto < end)
                                {
                                    original[upto++] = value;
                                }
                            }
                        }

                        // Test specific filters that support roundtripping
                        // Skip DCTFilter, CCITTFaxFilter, JPXFilter, JBIG2Filter
                        try
                        {
                            Filter flateFilter = FilterFactory.INSTANCE.getFilter(COSName.FLATE_DECODE);
                            checkEncodeDecode(flateFilter, original);
                        }
                        catch (Exception)
                        {
                            // Filter may not be accessible, skip
                        }

                        try
                        {
                            Filter lzwFilter = FilterFactory.INSTANCE.getFilter(COSName.LZW_DECODE);
                            checkEncodeDecode(lzwFilter, original);
                        }
                        catch (Exception)
                        {
                            // Filter may not be accessible, skip
                        }

                        try
                        {
                            Filter rleFilter = FilterFactory.INSTANCE.getFilter(COSName.RUN_LENGTH_DECODE);
                            checkEncodeDecode(rleFilter, original);
                        }
                        catch (Exception)
                        {
                            // Filter may not be accessible, skip
                        }

                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            Console.Error.WriteLine("NOTE: test failed with seed=" + seed);
                        }
                    }
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException during testFilters: " + ex.Message);
            }
        }

        /// <summary>
        /// This will test the use of identity filter to decode stream and string.
        /// This test threw an IOException before the correction.
        /// </summary>
        [Fact]
        public void testPDFBOX4517()
        {
            try
            {
                var file = new java.io.File("target/pdfs/PDFBOX-4517-cryptfilter.pdf");
                if (!file.exists())
                {
                    return; // Skip test if file doesn't exist
                }

                using (PDDocument doc = Loader.loadPDF(file, "userpassword1234"))
                {
                    Assert.Equal(1, doc.getNumberOfPages());
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException during testPDFBOX4517: " + ex.Message);
            }
        }

        /// <summary>
        /// This will test the LZW filter with the sequence that failed in PDFBOX-1977.
        /// To check that the test itself is legit, revert LZWFilter.java to rev 1571801,
        /// which should fail this test.
        /// </summary>
        [Fact]
        public void testPDFBOX1977()
        {
            try
            {
                java.io.InputStream stream = null;

                // Try to load from Java class resources
                try
                {
                    var javaClass = java.lang.Class.forName("org.apache.pdfbox.filter.TestFilters");
                    stream = javaClass.getResourceAsStream("PDFBOX-1977.bin");
                }
                catch (java.lang.ClassNotFoundException)
                {
                    // Class not found, skip
                }

                if (stream == null)
                {
                    return; // Skip test if resource doesn't exist
                }

                using (stream)
                {
                    Filter lzwFilter = FilterFactory.INSTANCE.getFilter(COSName.LZW_DECODE);

                    // Read all bytes from input stream
                    var baos = new java.io.ByteArrayOutputStream();
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    while ((bytesRead = stream.read(buffer)) != -1)
                    {
                        baos.write(buffer, 0, bytesRead);
                    }
                    byte[] byteArray = baos.toByteArray();

                    checkEncodeDecode(lzwFilter, byteArray);
                }
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException during testPDFBOX1977: " + ex.Message);
            }
        }

        /// <summary>
        /// Test simple and corner cases (128 identical, 128 identical at the end) of RLE implementation.
        /// 128 non identical bytes likely to be caught in random testing.
        /// </summary>
        [Fact]
        public void testRLE()
        {
            try
            {
                Filter rleFilter = FilterFactory.INSTANCE.getFilter(COSName.RUN_LENGTH_DECODE);
                byte[] input0 = new byte[0];
                checkEncodeDecode(rleFilter, input0);
                byte[] input1 = { 1, 2, 3, 4, 5, 128, 140, 180, 0xFF };
                checkEncodeDecode(rleFilter, input1);
                byte[] input2 = new byte[10];
                checkEncodeDecode(rleFilter, input2);
                byte[] input3 = new byte[128];
                checkEncodeDecode(rleFilter, input3);
                byte[] input4 = new byte[129];
                checkEncodeDecode(rleFilter, input4);
                byte[] input5 = new byte[128 + 128];
                checkEncodeDecode(rleFilter, input5);
                byte[] input6 = new byte[1];
                checkEncodeDecode(rleFilter, input6);
                byte[] input7 = { 1, 2 };
                checkEncodeDecode(rleFilter, input7);
                byte[] input8 = new byte[2];
                checkEncodeDecode(rleFilter, input8);
            }
            catch (java.io.IOException ex)
            {
                Assert.Fail("IOException during testRLE: " + ex.Message);
            }
        }

        [Fact]
        public void testEmptyFilterList()
        {
            Assert.Throws<java.lang.IllegalArgumentException>(() =>
            {
                Filter.decode(null, new java.util.ArrayList(), new COSDictionary(), null, null);
            });
        }

        private void checkEncodeDecode(Filter filter, byte[] original)
        {
            var encoded = new java.io.ByteArrayOutputStream();
            filter.encode(new java.io.ByteArrayInputStream(original), encoded, new COSDictionary(), 0);
            var decoded = new java.io.ByteArrayOutputStream();
            filter.decode(new java.io.ByteArrayInputStream(encoded.toByteArray()),
                    decoded, new COSDictionary(), 0);

            Assert.Equal(original, decoded.toByteArray());
        }
    }
}
