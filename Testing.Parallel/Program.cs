using System.Diagnostics;
using System.Text;
using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

namespace Testing.Parallel;

public class Program
{
    private const int RunsPerTest = 20;
    private const int WarmupGenerations = 10;
    
    private const int PolygonCount = 40;
    private const int BasePopulationSize = 50;
    private const int StaticGenerations = 100;
    
    private const string TargetImagePath = @"C:\Users\user\Downloads\5375221304493021630.jpg";

    public static void Main()
    {
        Pixel[] targetPixels = ImageLoader.LoadTargetImage(TargetImagePath, out int width, out int height);

        var fitness = new ImageApproximationFitness(targetPixels, width, height);
        var factory = new PolygonFactory(PolygonCount, width, height);
        var crossover = new UniformPolygonCrossover();
        var mutation = new PolygonMutation(width, height);
        var selection = new TournamentSelectionStrategy<List<ColoredPolygon>>();

        var engines = new Dictionary<string, ParallelBaseGeneticEngine<List<ColoredPolygon>>>
        {
            {
                "Engine by ForEach.Parallel", 
                new ParallelGeneticEngine<List<ColoredPolygon>>(
                    fitness, crossover, mutation, selection, factory, BasePopulationSize, 2, 15, 2)
            },
            {
                "Engine by ThreadPool", 
                new ParallelGeneticEngineByThreadPool<List<ColoredPolygon>>(
                    fitness, crossover, mutation, selection, factory, BasePopulationSize, 2, 15, 2)
            }
        };

        int[] threadCounts = [2, 3, 4, 5, 6, 7, 8, 9, 10];

        RunGenerationTest(engines, threadCounts, [250, 500, 750, 1000, 1250, 1500, 1750, 2000]);
        RunPopulationTest(engines, threadCounts, [10, 20, 30, 40, 50, 60, 70, 100]);
    }

    private static void RunGenerationTest(
        Dictionary<string, ParallelBaseGeneticEngine<List<ColoredPolygon>>> engines, 
        int[] threadCounts, 
        int[] generations)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Engine;ThreadCount;Generations;AvgTimeMs");

        foreach (var engineEntry in engines)
        {
            string engineName = engineEntry.Key;
            var engine = engineEntry.Value;

            foreach (var threadCount in threadCounts)
            {
                engine.ThreadCount = threadCount;

                foreach (var generation in generations)
                {
                    long averageMs = MeasureAverageExecutionTime(
                        warmupAction: () => engine.Run(WarmupGenerations),
                        testAction: () => engine.Run(generation)
                    );

                    Console.WriteLine($"{engineName};{threadCount};{generation};{averageMs}");
                    csv.AppendLine($"{engineName};{threadCount};{generation};{averageMs}");
                }
            }
        }

        File.WriteAllText("resultsGeneration.csv", csv.ToString());
    }

    private static void RunPopulationTest(
        Dictionary<string, ParallelBaseGeneticEngine<List<ColoredPolygon>>> engines, 
        int[] threadCounts, 
        int[] populationSizes)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Engine;ThreadCount;Population;AvgTimeMs");

        foreach (var engineEntry in engines)
        {
            string engineName = engineEntry.Key;
            var engine = engineEntry.Value;

            foreach (var threadCount in threadCounts)
            {
                engine.ThreadCount = threadCount;

                foreach (var population in populationSizes)
                {
                    engine.PopulationSize = population;

                    long averageMs = MeasureAverageExecutionTime(
                        warmupAction: () => engine.Run(WarmupGenerations),
                        testAction: () => engine.Run(StaticGenerations)
                    );

                    Console.WriteLine($"{engineName};{threadCount};{population};{averageMs}");
                    csv.AppendLine($"{engineName};{threadCount};{population};{averageMs}");
                }
            }
        }

        File.WriteAllText("resultsPopulation.csv", csv.ToString());
    }

    private static long MeasureAverageExecutionTime(Action warmupAction, Action testAction)
    {
        warmupAction();

        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < RunsPerTest; i++)
        {
            testAction();
        }
        stopwatch.Stop();

        return stopwatch.ElapsedMilliseconds / RunsPerTest;
    }
}