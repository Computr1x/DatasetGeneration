using DataSetGen.Generators;
using DataSetGen.Utils;
using ExNihilo.Base;
using ExNihilo.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.Json;

namespace DataSetGen
{
    internal class GenerationPipeline
    {
        private Func<Container> containerGenerator;
        private Func<Container, Container> maskGenerator;

        public string ResultFolder { get; set; } = "Results";
        public int SamplesCount { get; set; } = 3;
        public bool AsSeparateFolder { get; set; } = false;

        public delegate void RunProgresHandler(string message);
        public event RunProgresHandler? ProgresNotify;

        public GenerationPipeline(Func<Container> containerGenerator, Func<Container, Container> maskGenerator)
        {
            this.containerGenerator = containerGenerator;
            this.maskGenerator = maskGenerator;
        }

        public async Task Run()
        {
            List<Annotation> annotations = new();
            List<Utils.Image> images = new();
            string resFolder = AsSeparateFolder ? Path.Combine(ResultFolder, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) : ResultFolder;
            string path = CreateFolder(resFolder);


            Random r;
            for (int i = 0; i < SamplesCount; i++)
            {
                var container = containerGenerator();
                r = new Random(i);
                container.Randomize(r);
                using var image = container.Render();
                string imageName = $"{i}.png";
                await image.SaveAsPngAsync(Path.Combine(path, imageName));

                var maskContainer = maskGenerator(container);
                using var maskImage = maskContainer.Render();
                //await maskImage.SaveAsPngAsync(Path.Combine(path, $"{i}_mask.png"));

                var contours = BitMaskConverter.FindContours((Image<Rgba32>)maskImage, Color.White);
                var annotatedContours = CocoFormatter.Annotate(1, 1, contours);
                annotations.Add(annotatedContours);

                Utils.Image imageData = new()
                {
                    DateCaptured = DateTime.Now.ToString(),
                    FileName = imageName,
                    Height = image.Height,
                    Width = image.Width,
                    Id = i,
                };
                images.Add(imageData);

                ProgresNotify?.Invoke($"Container {i} is generated");
            }

            var cocoData = new CocoData()
            {
                Annotations = annotations,
                Categories = new List<Category>
                {
                    new Category() { Id = 0, Name = "Background" },
                    new Category() { Id = 1, Name = "Text" },
                },
                Images = images,
                Info = new Info()
                {
                    Contributor = "Me",
                    DateCreated = DateTime.Now.ToString(),
                    Description = "Text recognition dataset",
                    Version = "1.0",
                    Year = DateTime.Now.Year
                },
                Licenses = new List<License>()
                {
                    new License() { Id = 0, Name = "Apache License, Version 2.0", Url = "https://www.apache.org/licenses/LICENSE-2.0.txt" }
                },
            };
            string cocoJson = JsonSerializer.Serialize(cocoData);
            await File.WriteAllTextAsync(Path.Combine(path, "annotation.json"), cocoJson);

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
