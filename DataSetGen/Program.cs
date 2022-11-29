using DataSetGen;
using DataSetGen.Generators;

var generationPipeline = new GenerationPipeline(ContainerGenerator.GenerateAdvancedSimpleContainer, CaptchaMaskGenerator.Generate);
generationPipeline.ProgresNotify += (string message) => Console.WriteLine(message);
await generationPipeline.Run();