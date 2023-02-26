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
        private readonly List<Func<Container>> containerGenerators;

        public delegate void RunProgresHandler(string message);
        public event RunProgresHandler? ProgresNotify;

        public GenerationPipeline(Func<Container> containerGenerator) : this(new List<Func<Container>>() { containerGenerator })
        {
        }

        public GenerationPipeline(List<Func<Container>> containerGenerators)
        {
            this.containerGenerators = containerGenerators;
        }

        public string ResultFolder { get; set; } = "Results";
        public int SamplesCount { get; set; } = 3;
        public bool AsSeparateFolder { get; set; } = false;

        public Func<Container, Container> MaskGenerator { get; set; } = CaptchaMaskGenerator.GenerateMask;


        public async Task Run(int seed = 0)
        {
            List<Annotation> annotations = new();
            List<Utils.Image> images = new();
            string resFolder = AsSeparateFolder ? Path.Combine(ResultFolder, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) : ResultFolder;
            string path = CreateFolder(resFolder);

            string pngImagesPath = CreateFolder(Path.Combine(path, "PNGImages"));
            string pngMasksPath = CreateFolder(Path.Combine(path, "PNGMasks"));

            Random r = new(seed);
            int curContainerIndex = 0;
            for (int i = 0; i < SamplesCount; i++)
            {
                if (curContainerIndex >= containerGenerators.Count)
                    curContainerIndex = 0;

                var container = containerGenerators[curContainerIndex++]();
                container.Randomize(r);
                using var image = container.Render();
                string imageName = $"{i}.png";

                var maskContainer = MaskGenerator(container);
                using var maskImage = maskContainer.Render();

                var contours = BitMaskConverter.FindContours((Image<Rgba32>)maskImage, Color.White);
                var annotatedContours = CocoFormatter.Annotate(1, 1, contours);
                // if we not found any contours image not valid
                if(annotatedContours.Segmentation.Count < 1)
                {
                    ProgresNotify?.Invoke($"Container {i} will regenerated");
                    i--;
                    continue;
                }

                // save images
                await image.SaveAsPngAsync(Path.Combine(pngImagesPath, imageName));
                await maskImage.SaveAsPngAsync(Path.Combine(pngMasksPath, imageName));

                annotatedContours.Id = i;
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
