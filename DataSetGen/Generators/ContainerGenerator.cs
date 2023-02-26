using ExNihilo.Base;
using ExNihilo.Effects;
using ExNihilo.Visuals;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = ExNihilo.Visuals.Rectangle;

namespace DataSetGen.Generators
{
    internal static class ContainerGenerator
    {
        private static readonly FontCollection fonts = new();

        static ContainerGenerator()
        {
            var fontFiles = Directory.GetFiles("./assets/fonts/");
            foreach (var fontFile in fontFiles)
                fonts.Add(fontFile);
        }

        internal static Container GenerateSimpleContainer()
        {
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
                    stringPropertySetter.WithRandomizedLength(4, 8);
                })
                .WithRandomizedFontFamily(fonts.Families)
                .WithRandomizedBrush(100);

            Container captchaContainer = new Container(containerSize)
                .WithChild(captcha)
                .WithEffect(
                    new Wave()
                        .WithRandomizedAmplitude(5, 10)
                        .WithRandomizedWaveLength(100, 150)
                        .WithWaveWaveType(ExNihilo.Processors.WaveType.Sine)
                    )
                .WithEffect(
                    new Rotate()
                        .WithRandomizedDegree(-7, 7)
                )
                .WithEffect(
                    new Shift()
                        .WithRandomizedXShift(-containerSize.Width / 10, containerSize.Width / 10)
                        .WithRandomizedYShift(-containerSize.Height / 10, containerSize.Height / 10)
                );

            Container backgroundContainter = new Container(containerSize)
                .WithChild(
                    new Rectangle()
                        .WithSize(containerSize)
                        .WithBrush(brush =>
                        {
                            brush.WithType(BrushType.Solid);
                            brush.WithRandomizedColor(50);
                        })
                        .WithType(VisualType.Filled)
                    );

            // create main container
            Container container = new Container(containerSize)
                .WithContainer(backgroundContainter)
                .WithContainer(captchaContainer);

            return container;
        }

        internal static Container GenerateAdvancedContainer1()
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

        internal static Container GenerateAdvancedContainer2()
        {
            var container = GenerateSimpleContainer();

            container.WithContainer(
                new Container(container.Size)
                    .WithColorBlendingMode(SixLabors.ImageSharp.PixelFormats.PixelColorBlendingMode.Add)
                    .WithChildren(
                        Enumerable.Range(0, 8).Select(x =>
                            new Polygon()
                                .WithRandomizedPoints(3, 6, container.Size.Width, 0, container.Size.Height)
                                .WithType(VisualType.Filled)
                                .WithBrush((BrushProperty brush) =>
                                {
                                    brush.WithRandomizedColor(32);
                                    brush.WithType(BrushType.Solid);
                                })
                                .WithEffect(
                                    new Wave()
                                        .WithRandomizedAmplitude(10, 50)
                                        .WithRandomizedWaveLength(50, 150)
                                )
                        ))
                    .WithEffect(new GaussianNoise(0.3f, false)))
                .WithBlendPercentage(0.5f);

            return container;
        }
    }
}
