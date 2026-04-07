using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        var engine = new SequentialGeneticEngine<List<ColoredPolygon>>(
            fitness, crossover, mutation, selection, factory, BasePopulationSize, 2, 15);

        RunGenerationTest(engine, [250, 500, 750, 1000, 1250, 1500, 1750, 2000]);
        RunPopulationTest(engine, [10, 20, 30, 40, 50, 60, 70, 100]);
    }

    private static void RunGenerationTest(SequentialGeneticEngine<List<ColoredPolygon>> engine, int[] generations)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Engine;ThreadCount;Generations;AvgTimeMs");
        
        foreach (var generation in generations)
        {
            long averageMs = MeasureAverageExecutionTime(
                warmupAction: () => engine.Run(WarmupGenerations),
                testAction: () => engine.Run(generation)
            );

            Console.WriteLine($"Sequential;{generation};{averageMs}");
            csv.AppendLine($"Sequential;{generation};{averageMs}");
        }

        File.WriteAllText("resultSequentialGeneration.csv", csv.ToString());
    }

    private static void RunPopulationTest(SequentialGeneticEngine<List<ColoredPolygon>> engine, int[] populationSizes)
    {
        var csv = new StringBuilder();
        csv.AppendLine("Engine;Population;AvgTimeMs");
        
        foreach (var population in populationSizes)
        {
            engine.PopulationSize = population;

            long averageMs = MeasureAverageExecutionTime(
                warmupAction: () => engine.Run(WarmupGenerations),
                testAction: () => engine.Run(StaticGenerations)
            );

            Console.WriteLine($"Sequential;{population};{averageMs}");
            csv.AppendLine($"Sequential;{population};{averageMs}");
        }

        File.WriteAllText("resultSequentialPopulation.csv", csv.ToString());
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