using GeneticAlgoKnapsack;
using GeneticAlgorithm;
using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

namespace Testings;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Starting Genetic Algorithm...");

        // 1. Load the target image (Make sure you have a small image like 100x100 to start!)
        string targetImagePath = "/home/maksym_vivdychenko/Pictures/heart.png";
        
        Pixel[] targetPixels = ImageLoader.LoadTargetImage(targetImagePath, out int width, out int height);
        
        int polygonCount = 50;
        int populationSize = 30;
        int generations = 1000;
        
        var fitness = new ImageApproximationFitness(targetPixels, width, height);
        var factory = new PolygonFactory(polygonCount, width, height);
        var crossover = new UniformPolygonCrossover();
        var mutation = new PolygonMutation(width, height);
        var selection = new TournamentSelectionStrategy<List<ColoredPolygon>>();
        
        var engine = new ParallelGeneticEngine<List<ColoredPolygon>>(
            fitness, crossover, mutation, selection, factory, populationSize, 2, 15, Environment.ProcessorCount);
        var result = engine.Run(generations);

        var fitnessValue = fitness.EvaluateFitness(result);
        Console.WriteLine(fitnessValue);

        // var engineByThreadPool = new ParallelGeneticAlgorithmEngineByThreadPool<List<ColoredPolygon>>(
        //     fitness, crossover, mutation, selection, factory);

        // string outputFolder = Path.Combine(Environment.CurrentDirectory, "EvolutionFrames");
        //
        // // 6. Run the Engine!
        // Console.WriteLine($"Evolving {polygonCount} polygons over {generations} generations...");
        // var imageThread = engineByThread.Run(generations, populationSize, elitismCount: 2, mutationRate: 15);
        // //var imageThreadPool = engineByThread.Run(generations, populationSize, elitismCount: 2, mutationRate: 15);
        // ImageExporter.SaveChromosomeToDisk(imageThread, width, height, Path.Combine(outputFolder, "thread5000.png"));
        // //ImageExporter.SaveChromosomeToDisk(imageThread, width, height, Path.Combine(outputFolder, "threadPool5000.png"));
        // Console.WriteLine(Path.Combine(outputFolder, "thread5000.png"));
        // Console.WriteLine("Evolution Complete! Check the EvolutionFrames folder.");
    }
}
