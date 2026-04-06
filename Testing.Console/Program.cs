using System.Diagnostics;
using System.Text;
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
        int times = 20;
        string targetImagePath = "C:\\Users\\user\\Downloads\\5375221304493021630.jpg";

        Pixel[] targetPixels = ImageLoader.LoadTargetImage(targetImagePath, out int width, out int height);
        int[] generations = [250, 500, 750, 1000, 1250, 1500, 1750, 2000];

        int polygonCount = 40; //змінити в звіті
        int populationSize = 50;

        var fitness = new ImageApproximationFitness(targetPixels, width, height);
        var factory = new PolygonFactory(polygonCount, width, height);
        var crossover = new UniformPolygonCrossover();
        var mutation = new PolygonMutation(width, height);
        var selection = new TournamentSelectionStrategy<List<ColoredPolygon>>();

        var engine = new SequentialGeneticEngine<List<ColoredPolygon>>(
            fitness, crossover, mutation, selection, factory, populationSize, 2, 15);

        // var csvGeneration = new StringBuilder();
        // csvGeneration.AppendLine("Engine;ThreadCount;Generations;AvgTimeMs");
        // foreach (var generation in generations)
        // {
        //     Stopwatch stopwatch = Stopwatch.StartNew();
        //     engine.Run(10);
        //     for (int i = 0; i < times; i++)
        //     {
        //         engine.Run(generation);    
        //     }
        //     
        //     stopwatch.Stop();
        //     var averageMs = (long)(stopwatch.ElapsedMilliseconds / times);
        //     Console.WriteLine($"Sequential;{generation};{averageMs}");
        //     csvGeneration.AppendLine($"Sequential;{generation};{averageMs}");
        // }
        //
        // string csvGenerationPath = "resultSequentialGeneration.csv";
        // File.WriteAllText(csvGenerationPath, csvGeneration.ToString());
        
        int[] populationSizes = [10, 20, 30, 40, 50, 60, 70, 100];
        int generationStatic = 2; // змінити в звіті
        
        var csvPopulation = new StringBuilder();
        csvPopulation.AppendLine("Engine;Population;AvgTimeMs");
        foreach (var population in populationSizes)
        {
            engine.PopulationSize = population;
            engine.Run(10);
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                engine.Run(generationStatic);    
            }
            
            stopwatch.Stop();
            var averageMs = (long)(stopwatch.ElapsedMilliseconds / times);
            Console.WriteLine($"Sequential;{population};{averageMs}");
            csvPopulation.AppendLine($"Sequential;{population};{averageMs}");
        }
        
        string csvPopulationPath = "resultSequentialGeneration.csv";
        File.WriteAllText(csvPopulationPath, csvPopulation.ToString());
    }
}