using DataSetGen.Generators;
using DataSetGen.Utils;
using ExNihilo.Base;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace DataSetGen
{
    internal class GenerationPipeline
    {
        private Func<Container> containerGenerator;
        private Func<Container, Container> maskGenerator;

        public string ResultFolder { get; set; } = "Result";
        public int SamplesCount { get; set; } = 3;

        public delegate void RunProgresHandler(string message);
        public event RunProgresHandler? ProgresNotify;

        public GenerationPipeline(Func<Container> containerGenerator, Func<Container, Container> maskGenerator)
        {
            this.containerGenerator = containerGenerator;
            this.maskGenerator = maskGenerator;
        }

        public async Task Run()
        {
            string path = CreateFolder(ResultFolder);

            Random r;
            for (int i = 0; i < SamplesCount; i++)
            {
                var container = containerGenerator();
                r = new Random(i);
                container.Randomize(r);
                var image = container.Render();
                await image.SaveAsPngAsync(Path.Combine(path, $"{i}.png"));

                var maskContainer = maskGenerator(container);
                image = maskContainer.Render();
                await image.SaveAsPngAsync(Path.Combine(path, $"{i}_mask.png"));

                BitMaskConverter.FindContours(image as Image<Rgba32>, Color.White);

                ProgresNotify?.Invoke($"Container {i} is generated");
            }

            ProgresNotify?.Invoke("Done!");
        }

        private static string CreateFolder(string folderName)
        {
            string path = $"./{folderName}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
