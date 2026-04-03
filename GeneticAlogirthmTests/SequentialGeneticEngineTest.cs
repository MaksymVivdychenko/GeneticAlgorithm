using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

namespace GeneticAlogirthmTests;

public class SequentialGeneticEngineTest
{
    public SequentialGeneticEngineTest()
    {
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
        var engine = new SequentialGeneticEngine<List<ColoredPolygon>>(
            fitness, crossover, mutation, selection, factory, populationSize, 2, 15);
        var result = engine.Run(5000);
        
    }
    
    [Fact]
    public void Test1()
    {
    }
}