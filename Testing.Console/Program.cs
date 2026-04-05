using System.Diagnostics;
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
        int times = 3;
        string targetImagePath = "/home/maksym_vivdychenko/Pictures/heart.png";

        Pixel[] targetPixels = ImageLoader.LoadTargetImage(targetImagePath, out int width, out int height);
        int[] generations = [250, 500, 750, 1000, 1250, 1500, 1750, 2000];

        int polygonCount = 20; //змінити в звіті
        int populationSize = 30;

        var fitness = new ImageApproximationFitness(targetPixels, width, height);
        var factory = new PolygonFactory(polygonCount, width, height);
        var crossover = new UniformPolygonCrossover();
        var mutation = new PolygonMutation(width, height);
        var selection = new TournamentSelectionStrategy<List<ColoredPolygon>>();

        var engine = new SequentialGeneticEngine<List<ColoredPolygon>>(
            fitness, crossover, mutation, selection, factory, populationSize, 2, 15);

        foreach (var generation in generations)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                engine.Run(generation);    
            }
            
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed / times;
            Console.WriteLine($"Час виконання для {generation} генерацій: {(long)ts.TotalMilliseconds} мс");
        }
    }
}