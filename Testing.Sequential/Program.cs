using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

int tries = 30;
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
double[] fitnessMetrics = new double[tries];
for (int i = 0; i < tries; i++)
{
    var result = engine.Run(generations);
    var fitnessValue = fitness.EvaluateFitness(result);
    fitnessMetrics[i] = fitnessValue;
    Console.WriteLine($"Iteration: {i + 1}");
}

var (mean, sigma) = CalculateMeanAndSigma(fitnessMetrics);
Console.Write($"Mean: {mean}; Sigma: {sigma}; Boundaries: [{mean - 3 * sigma}; {mean + 3 * sigma}] ");
return;

(double Mean, double Sigma) CalculateMeanAndSigma(double[] fitnessResults)
{
    double mean = fitnessResults.Average();
    
    double sumOfSquaredDifferences = fitnessResults.Sum(x => Math.Pow(x - mean, 2));
    
    double sigma = Math.Sqrt(sumOfSquaredDifferences / (fitnessResults.Length - 1));

    return (mean, sigma);
}
