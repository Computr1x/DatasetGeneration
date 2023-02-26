using DataSetGen;
using DataSetGen.Generators;
using ExNihilo.Base;

List<Func<Container>> generators = new()
{
    ContainerGenerator.GenerateAdvancedContainer1,
    ContainerGenerator.GenerateAdvancedContainer2,
};

var generationPipeline = new GenerationPipeline(generators)
{
    AsSeparateFolder = true,
    SamplesCount = 200
};
generationPipeline.ProgresNotify += (string message) => Console.WriteLine(message);
await generationPipeline.Run();