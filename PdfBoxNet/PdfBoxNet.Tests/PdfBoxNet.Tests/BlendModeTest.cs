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

using Xunit;
using org.apache.pdfbox.cos;
using org.apache.pdfbox.pdmodel.graphics.blend;

namespace PdfBoxNet.Tests
{
    /// <summary>
    /// Test for BlendMode class.
    /// Converted from org.apache.pdfbox.pdmodel.graphics.blend.BlendModeTest
    /// </summary>
    public class BlendModeTest
    {
        /// <summary>
        /// Check that BlendMode.* constant instances are not null.
        /// </summary>
        [Fact]
        public void testInstances()
        {
            Assert.Equal(BlendMode.NORMAL, BlendMode.getInstance(COSName.NORMAL));
            Assert.Equal(BlendMode.NORMAL, BlendMode.getInstance(COSName.COMPATIBLE));
            Assert.Equal(BlendMode.MULTIPLY, BlendMode.getInstance(COSName.MULTIPLY));
            Assert.Equal(BlendMode.SCREEN, BlendMode.getInstance(COSName.SCREEN));
            Assert.Equal(BlendMode.OVERLAY, BlendMode.getInstance(COSName.OVERLAY));
            Assert.Equal(BlendMode.DARKEN, BlendMode.getInstance(COSName.DARKEN));
            Assert.Equal(BlendMode.LIGHTEN, BlendMode.getInstance(COSName.LIGHTEN));
            Assert.Equal(BlendMode.COLOR_DODGE, BlendMode.getInstance(COSName.COLOR_DODGE));
            Assert.Equal(BlendMode.COLOR_BURN, BlendMode.getInstance(COSName.COLOR_BURN));
            Assert.Equal(BlendMode.HARD_LIGHT, BlendMode.getInstance(COSName.HARD_LIGHT));
            Assert.Equal(BlendMode.SOFT_LIGHT, BlendMode.getInstance(COSName.SOFT_LIGHT));
            Assert.Equal(BlendMode.DIFFERENCE, BlendMode.getInstance(COSName.DIFFERENCE));
            Assert.Equal(BlendMode.EXCLUSION, BlendMode.getInstance(COSName.EXCLUSION));
            Assert.Equal(BlendMode.HUE, BlendMode.getInstance(COSName.HUE));
            Assert.Equal(BlendMode.SATURATION, BlendMode.getInstance(COSName.SATURATION));
            Assert.Equal(BlendMode.LUMINOSITY, BlendMode.getInstance(COSName.LUMINOSITY));
            Assert.Equal(BlendMode.COLOR, BlendMode.getInstance(COSName.COLOR));

            COSArray cosArrayOverlay = new COSArray();
            cosArrayOverlay.add(COSName.OVERLAY);
            Assert.Equal(BlendMode.OVERLAY, BlendMode.getInstance(cosArrayOverlay));

            COSArray cosArrayInteger = new COSArray();
            cosArrayInteger.add(COSInteger.get(0));
            Assert.Equal(BlendMode.NORMAL, BlendMode.getInstance(cosArrayInteger));
        }

        [Fact]
        public void testBlendModeNormal()
        {
            Assert.True(BlendMode.NORMAL.isSeparableBlendMode());
            Assert.Null(BlendMode.NORMAL.getBlendFunction());
            Assert.NotNull(BlendMode.NORMAL.getBlendChannelFunction());
            Assert.Equal(COSName.NORMAL, BlendMode.NORMAL.getCOSName());
            Assert.Equal(3f, BlendMode.NORMAL.getBlendChannelFunction().blendChannel(3f, 5f));

            Assert.Equal(COSName.NORMAL, BlendMode.COMPATIBLE.getCOSName());
        }

        [Fact]
        public void testBlendModeMultiply()
        {
            Assert.True(BlendMode.MULTIPLY.isSeparableBlendMode());
            Assert.Null(BlendMode.MULTIPLY.getBlendFunction());
            Assert.NotNull(BlendMode.MULTIPLY.getBlendChannelFunction());
            Assert.Equal(COSName.MULTIPLY, BlendMode.MULTIPLY.getCOSName());
            Assert.Equal(15f, BlendMode.MULTIPLY.getBlendChannelFunction().blendChannel(3f, 5f));
        }

        [Fact]
        public void testBlendModeScreen()
        {
            Assert.True(BlendMode.SCREEN.isSeparableBlendMode());
            Assert.Null(BlendMode.SCREEN.getBlendFunction());
            Assert.NotNull(BlendMode.SCREEN.getBlendChannelFunction());
            Assert.Equal(COSName.SCREEN, BlendMode.SCREEN.getCOSName());
            Assert.Equal(-7f, BlendMode.SCREEN.getBlendChannelFunction().blendChannel(3f, 5f));
        }

        [Fact]
        public void testBlendModeOverlay()
        {
            Assert.True(BlendMode.OVERLAY.isSeparableBlendMode());
            Assert.Null(BlendMode.OVERLAY.getBlendFunction());
            Assert.NotNull(BlendMode.OVERLAY.getBlendChannelFunction());
            Assert.Equal(COSName.OVERLAY, BlendMode.OVERLAY.getCOSName());
            Assert.Equal(0f, BlendMode.OVERLAY.getBlendChannelFunction().blendChannel(1f, 0f));
            Assert.Equal(0.3f, BlendMode.OVERLAY.getBlendChannelFunction().blendChannel(0.5f, 0.3f));
        }

        [Fact]
        public void testBlendModeDarken()
        {
            Assert.True(BlendMode.DARKEN.isSeparableBlendMode());
            Assert.Null(BlendMode.DARKEN.getBlendFunction());
            Assert.NotNull(BlendMode.DARKEN.getBlendChannelFunction());
            Assert.Equal(COSName.DARKEN, BlendMode.DARKEN.getCOSName());
            Assert.Equal(3f, BlendMode.DARKEN.getBlendChannelFunction().blendChannel(3f, 5f));
        }

        [Fact]
        public void testBlendModeLighten()
        {
            Assert.True(BlendMode.LIGHTEN.isSeparableBlendMode());
            Assert.Null(BlendMode.LIGHTEN.getBlendFunction());
            Assert.NotNull(BlendMode.LIGHTEN.getBlendChannelFunction());
            Assert.Equal(COSName.LIGHTEN, BlendMode.LIGHTEN.getCOSName());
            Assert.Equal(5f, BlendMode.LIGHTEN.getBlendChannelFunction().blendChannel(3f, 5f));
        }

        [Fact]
        public void testBlendModeColorDodge()
        {
            Assert.True(BlendMode.COLOR_DODGE.isSeparableBlendMode());
            Assert.Null(BlendMode.COLOR_DODGE.getBlendFunction());
            Assert.NotNull(BlendMode.COLOR_DODGE.getBlendChannelFunction());
            Assert.Equal(COSName.COLOR_DODGE, BlendMode.COLOR_DODGE.getCOSName());
            Assert.Equal(0f, BlendMode.COLOR_DODGE.getBlendChannelFunction().blendChannel(1f, 0f));
            Assert.Equal(1f, BlendMode.COLOR_DODGE.getBlendChannelFunction().blendChannel(0.3f, 0.7f));
        }

        [Fact]
        public void testBlendModeColorBurn()
        {
            Assert.True(BlendMode.COLOR_BURN.isSeparableBlendMode());
            Assert.Null(BlendMode.COLOR_BURN.getBlendFunction());
            Assert.NotNull(BlendMode.COLOR_BURN.getBlendChannelFunction());
            Assert.Equal(COSName.COLOR_BURN, BlendMode.COLOR_BURN.getCOSName());
            Assert.Equal(1f, BlendMode.COLOR_BURN.getBlendChannelFunction().blendChannel(0f, 1f));
            Assert.Equal(0f, BlendMode.COLOR_BURN.getBlendChannelFunction().blendChannel(0.7f, 0.3f));
        }

        [Fact]
        public void testBlendModeHardLight()
        {
            Assert.True(BlendMode.HARD_LIGHT.isSeparableBlendMode());
            Assert.Null(BlendMode.HARD_LIGHT.getBlendFunction());
            Assert.NotNull(BlendMode.HARD_LIGHT.getBlendChannelFunction());
            Assert.Equal(COSName.HARD_LIGHT, BlendMode.HARD_LIGHT.getCOSName());
            Assert.Equal(0f, BlendMode.HARD_LIGHT.getBlendChannelFunction().blendChannel(0f, 0.5f));
            Assert.Equal(0.2f, BlendMode.HARD_LIGHT.getBlendChannelFunction().blendChannel(0.2f, 0.5f));
            Assert.Equal(0.52f,
                    BlendMode.HARD_LIGHT.getBlendChannelFunction().blendChannel(0.6f, 0.4f));
        }

        [Fact]
        public void testBlendModeSoftLight()
        {
            Assert.True(BlendMode.SOFT_LIGHT.isSeparableBlendMode());
            Assert.Null(BlendMode.SOFT_LIGHT.getBlendFunction());
            Assert.NotNull(BlendMode.SOFT_LIGHT.getBlendChannelFunction());
            Assert.Equal(COSName.SOFT_LIGHT, BlendMode.SOFT_LIGHT.getCOSName());
            Assert.Equal(0.25f, BlendMode.SOFT_LIGHT.getBlendChannelFunction().blendChannel(0f, 0.5f));
            Assert.Equal(0.35f,
                    BlendMode.SOFT_LIGHT.getBlendChannelFunction().blendChannel(0.2f, 0.5f));
            Assert.Equal(0.2f,
                    BlendMode.SOFT_LIGHT.getBlendChannelFunction().blendChannel(0.5f, 0.2f));
        }

        [Fact]
        public void testBlendModeDifference()
        {
            Assert.True(BlendMode.DIFFERENCE.isSeparableBlendMode());
            Assert.Null(BlendMode.DIFFERENCE.getBlendFunction());
            Assert.NotNull(BlendMode.DIFFERENCE.getBlendChannelFunction());
            Assert.Equal(COSName.DIFFERENCE, BlendMode.DIFFERENCE.getCOSName());
            Assert.Equal(2f, BlendMode.DIFFERENCE.getBlendChannelFunction().blendChannel(3f, 5f));
        }

        [Fact]
        public void testBlendModeExclusion()
        {
            Assert.True(BlendMode.EXCLUSION.isSeparableBlendMode());
            Assert.Null(BlendMode.EXCLUSION.getBlendFunction());
            Assert.NotNull(BlendMode.EXCLUSION.getBlendChannelFunction());
            Assert.Equal(COSName.EXCLUSION, BlendMode.EXCLUSION.getCOSName());
        }

        [Fact]
        public void testBlendModeHue()
        {
            Assert.False(BlendMode.HUE.isSeparableBlendMode());
            Assert.NotNull(BlendMode.HUE.getBlendFunction());
            Assert.Null(BlendMode.HUE.getBlendChannelFunction());
            Assert.Equal(COSName.HUE, BlendMode.HUE.getCOSName());
        }

        [Fact]
        public void testBlendModeSaturation()
        {
            Assert.False(BlendMode.SATURATION.isSeparableBlendMode());
            Assert.NotNull(BlendMode.SATURATION.getBlendFunction());
            Assert.Null(BlendMode.SATURATION.getBlendChannelFunction());
            Assert.Equal(COSName.SATURATION, BlendMode.SATURATION.getCOSName());
        }

        [Fact]
        public void testBlendModeLuminosity()
        {
            Assert.False(BlendMode.LUMINOSITY.isSeparableBlendMode());
            Assert.NotNull(BlendMode.LUMINOSITY.getBlendFunction());
            Assert.Null(BlendMode.LUMINOSITY.getBlendChannelFunction());
            Assert.Equal(COSName.LUMINOSITY, BlendMode.LUMINOSITY.getCOSName());
        }

        [Fact]
        public void testBlendModeColor()
        {
            Assert.False(BlendMode.COLOR.isSeparableBlendMode());
            Assert.NotNull(BlendMode.COLOR.getBlendFunction());
            Assert.Null(BlendMode.COLOR.getBlendChannelFunction());
            Assert.Equal(COSName.COLOR, BlendMode.COLOR.getCOSName());
        }
    }
}
