using System.Diagnostics;
using System.Text;
using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

int times = 1; //змінити в звіті
string targetImagePath = "/home/maksym_vivdychenko/Pictures/heart.png";

Pixel[] targetPixels = ImageLoader.LoadTargetImage(targetImagePath, out int width, out int height);
//int[] generations = [250, 500, 750, 1000, 1250, 1500, 1750, 2000];
//int[] threadCounts = [2, 3, 4, 5, 6, 7, 8, 9, 10];
int[] threadCounts = [8, 9, 10];
int[] generations = [1750, 2000];

int polygonCount = 15; //змінити в звіті
int populationSize = 20;

var fitness = new ImageApproximationFitness(targetPixels, width, height);
var factory = new PolygonFactory(polygonCount, width, height);
var crossover = new UniformPolygonCrossover();
var mutation = new PolygonMutation(width, height);
var selection = new TournamentSelectionStrategy<List<ColoredPolygon>>();

var engines = new Dictionary<string, ParallelBaseGeneticEngine<List<ColoredPolygon>>>();
// engines.Add("Engine by ForEach.Parallel", new ParallelGeneticEngine<List<ColoredPolygon>>(
//     fitness, crossover, mutation, selection, factory, populationSize, 2, 15, 2));
engines.Add("Engine by ThreadPool", new ParallelGeneticEngineByThreadPool<List<ColoredPolygon>>(
    fitness, crossover, mutation, selection, factory, populationSize, 2, 15, 2));

var csv = new StringBuilder();
csv.AppendLine("Engine;ThreadCount;Generations;AvgTimeMs");

foreach (var engine in engines)
{
    foreach (var threadCount in threadCounts)
    {
        foreach (var generation in generations)
        {
            engine.Value.ThreadCount = threadCount;
            engine.Value.Run(10);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                engine.Value.Run(generation);
            }

            stopwatch.Stop();
            var averageMs = (long)(stopwatch.ElapsedMilliseconds / times);
            Console.WriteLine($"{engine.Key};{threadCount};{generation};{averageMs}");
            csv.AppendLine($"{engine.Key};{threadCount};{generation};{averageMs}");
        }
    }
}

string csvPath = "results.csv";
File.WriteAllText(csvPath, csv.ToString());
