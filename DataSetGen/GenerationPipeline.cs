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
            string resFolder = AsSeparateFolder ? Path.Combine(ResultFolder, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()) : ResultFolder;
            string path = CreateFolder(resFolder);

            string pngImagesPath = CreateFolder(Path.Combine(path, "PNGImages"));
            string pngMasksPath = CreateFolder(Path.Combine(path, "PNGMasks"));

            var images = new Dictionary<int, Utils.Image>();
            var annotations = new Dictionary<int, Annotation>();

            Parallel.For(0, SamplesCount, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, async (int index) =>
            {
                var (imageInfo, annotation) = await GenerateData(seed, index, pngImagesPath, pngMasksPath);
                images[index] = imageInfo;
                annotations[index] = annotation;
            });

            var cocoData = new CocoData()
            {
                Annotations = annotations.OrderBy(x => x.Key).Select(x => x.Value).ToList(),
                Categories = new List<Category>
                {
                    new Category() { Id = 0, Name = "Background" },
                    new Category() { Id = 1, Name = "Text" },
                },
                Images = images.OrderBy(x => x.Key).Select(x => x.Value).ToList(),
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

        private async Task<(Utils.Image imageInfo, Annotation annotation)> GenerateData(int seed, int index, string pngImagesPath, string pngMasksPath)
        {
            Random r = new Random(seed + index);
            var container = containerGenerators[index % containerGenerators.Count]();
            SixLabors.ImageSharp.Image? image = null, maskImage = null;
            Annotation annotation;
            string imageName = $"{index}.png";

            try
            {
                do
                {
                    container.Randomize(r);

                    image = container.Render();

                    var maskContainer = MaskGenerator(container);
                    maskImage = maskContainer.Render();

                    var contours = BitMaskConverter.FindContours((Image<Rgba32>)maskImage, Color.White);
                    annotation = CocoFormatter.Annotate(1, 1, contours);
                    annotation.Id = index;

                    // if we not found any contours image not valid
                    if (annotation.Segmentation.Count < 1)
                    {
                        ProgresNotify?.Invoke($"Container {index} will regenerated");
                    }

                } while (annotation.Segmentation.Count < 1);

                // save images
                await image.SaveAsPngAsync(Path.Combine(pngImagesPath, imageName));
                await maskImage.SaveAsPngAsync(Path.Combine(pngMasksPath, imageName));

                Utils.Image imageData = new()
                {
                    DateCaptured = DateTime.Now.ToString(),
                    FileName = imageName,
                    Height = image.Height,
                    Width = image.Width,
                    Id = index,
                };

                ProgresNotify?.Invoke($"Container {index} is generated");
                return (imageData, annotation);
            }
            finally
            {
                image?.Dispose();
                maskImage?.Dispose();
            }
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
