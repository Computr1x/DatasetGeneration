using DataSetGen;
using DataSetGen.Generators;

var generationPipeline = new GenerationPipeline(
    ContainerGenerator.GenerateAdvancedSimpleContainer,
    CaptchaMaskGenerator.Generate)
{
    AsSeparateFolder = true,
    SamplesCount = 10,
};
generationPipeline.ProgresNotify += (string message) => Console.WriteLine(message);
await generationPipeline.Run();