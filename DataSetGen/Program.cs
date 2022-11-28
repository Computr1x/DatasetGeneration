using DataSetGen.Generators;
using SixLabors.ImageSharp;

// generation
int seedCount = 3;
Random r;
for (int i = 0; i < seedCount; i++)
{
    var container = ContainerGenerator.GenerateSimpleContainer();
    r = new Random(i);
    container.Randomize(r);
    var image = container.Render();
    await image.SaveAsPngAsync($"./Results/{i}.png");

    image = CaptchaMaskGenerator.Generate(container);
    await image.SaveAsPngAsync($"./Results/{i}_mask.png");
}

Console.WriteLine("Done!");