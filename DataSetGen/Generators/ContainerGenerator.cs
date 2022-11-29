﻿using ExNihilo.Base;
using ExNihilo.Effects;
using ExNihilo.Visuals;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGen.Generators
{
    internal static class ContainerGenerator
    {
        internal static Container GenerateSimpleContainer()
        {
            FontCollection collection = new();
            var robotoFont = collection.Add("./assets/fonts/Roboto-Regular.ttf");

            Size containerSize = new(512, 256);
            Point center = new(256, 128);

            Captcha captcha =
                new Captcha()
                .WithPoint(center)
                .WithFontSize(100)
                .WithType(VisualType.Filled)
                .WithRandomizedContent(stringPropertySetter =>
                {
                    stringPropertySetter.CharactersSet = StringProperty.asciiUpperCase;
                    stringPropertySetter.WithLength(6);
                })
                .WithFontFamily(robotoFont)
                .WithRandomizedBrush(100);

            Container captchaContainer = new Container(containerSize)
                .WithChild(captcha)
                .WithEffect(
                    new Wave()
                        .WithRandomizedAmplitude(5, 10)
                        .WithRandomizedWaveLength(100, 150)
                        .WithWaveWaveType(ExNihilo.Processors.WaveType.Sine)
                    );

            // create main container
            Container container = new Container(containerSize)
                .WithBackground(Color.White)
                .WithContainer(captchaContainer);

            return container;
        }

        internal static Container GenerateAdvancedSimpleContainer()
        {
            var container = GenerateSimpleContainer();

            container.WithContainer(
                new Container(container.Size)
                    .WithColorBlendingMode(SixLabors.ImageSharp.PixelFormats.PixelColorBlendingMode.Multiply)
                    .WithChildren(
                        Enumerable.Range(0, 8).Select(x =>
                            new Ellipse()
                                .WithRandomizedPoint(0, container.Size.Width, 0, container.Size.Height)
                                .WithRandomizedSize(50, 80)
                                .WithType(VisualType.Filled)
                                .WithBrush((BrushProperty brush) =>
                                {
                                    brush.WithRandomizedColor(32);
                                    brush.WithType(BrushType.Solid);
                                })
                        ))
                    .WithEffect(new Ripple()))
                .WithBlendPercentage(0.5f);

            return container;
        }
    }
}
