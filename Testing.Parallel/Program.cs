using System.Diagnostics;
using System.Text;
using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

int times = 10; //змінити в звіті
string targetImagePath = "C:\\Users\\user\\Downloads\\5375221304493021630.jpg";

Pixel[] targetPixels = ImageLoader.LoadTargetImage(targetImagePath, out int width, out int height);
int[] generations = [250, 500, 750, 1000, 1250, 1500, 1750, 2000];
int[] threadCounts = [2, 3, 4, 5, 6, 7, 8, 9, 10];

int polygonCount = 40; //змінити в звіті
int populationSize = 50;

int[] populationSizes = [10, 20, 30, 40, 50, 60, 70, 100];
int generationStatic = 2; // змінити в звіті

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

var csvGeneration = new StringBuilder();
csvGeneration.AppendLine("Engine;ThreadCount;Generations;AvgTimeMs");

var csvPopulation = new StringBuilder();
csvPopulation.AppendLine("Engine;ThreadCount;Population;AvgTimeMs");

foreach (var engine in engines)
{
    foreach (var threadCount in threadCounts)
    {
        engine.Value.ThreadCount = threadCount;
        // foreach (var generation in generations)
        // {
        //     engine.Value.Run(10);
        //     Stopwatch stopwatch = Stopwatch.StartNew();
        //     for (int i = 0; i < times; i++)
        //     {
        //         engine.Value.Run(generation);
        //     }
        //
        //     stopwatch.Stop();
        //     var averageMs = (long)(stopwatch.ElapsedMilliseconds / times);
        //     Console.WriteLine($"{engine.Key};{threadCount};{generation};{averageMs}");
        //     csvGeneration.AppendLine($"{engine.Key};{threadCount};{generation};{averageMs}");
        // }
        
        foreach (var popSize in populationSizes)
        {
            engine.Value.PopulationSize = popSize;
            engine.Value.Run(10);
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < times; i++)
            {
                engine.Value.Run(generationStatic);
            }

            stopwatch.Stop();
            var averageMs = (long)(stopwatch.ElapsedMilliseconds / times);
            Console.WriteLine($"{engine.Key};{threadCount};{popSize};{averageMs}");
            csvPopulation.AppendLine($"{engine.Key};{threadCount};{popSize};{averageMs}");
        }
    }
}

//File.WriteAllText("resultsGeneration.csv", csvGeneration.ToString());
File.WriteAllText("resultsPopulation.csv", csvPopulation.ToString());
