using System;
using System.Collections.Generic;
using Xunit;
using GeneticAlgorithm.Engines;
using GeneticAlgorithm.SelectionStrategies;
using MonaLisaApproximation;
using MonaLisaApproximation.Domains;
using MonaLisaApproximation.Strategies;

namespace ParallelGATests;

public class ParallelGaTest
{
    private const int PolygonCount = 50;
    private const int PopulationSize = 30;
    private const int Generations = 1000;
    private const int TestRuns = 5;
    private const double ExpectedMinimumFitness = -345120468;
    
    private const string TargetImagePath = @"C:\Users\user\Downloads\5375221304493021630.jpg";

    [Fact]
    public void ParallelGeneticEngineTest()
    {
        double averageFitness = RunEngineMultipleTimesAndGetAverage((fitness, factory, crossover, mutation, selection) => 
            new ParallelGeneticEngine<List<ColoredPolygon>>(
                fitness, crossover, mutation, selection, factory, 
                PopulationSize, 2, 15, Environment.ProcessorCount));

        Assert.True(averageFitness >= ExpectedMinimumFitness, 
            $"Algorithm failed. Average fitness {averageFitness} is below the lower threshold {ExpectedMinimumFitness}.");
    }
    
    [Fact]
    public void ParallelGeneticEngineByThreadPoolTest()
    {
        double averageFitness = RunEngineMultipleTimesAndGetAverage((fitness, factory, crossover, mutation, selection) => 
            new ParallelGeneticEngineByThreadPool<List<ColoredPolygon>>(
                fitness, crossover, mutation, selection, factory, 
                PopulationSize, 2, 15, Environment.ProcessorCount));

        Assert.True(averageFitness >= ExpectedMinimumFitness, 
            $"Algorithm failed. Average fitness {averageFitness} is below the lower threshold {ExpectedMinimumFitness}.");
    }
    
    private double RunEngineMultipleTimesAndGetAverage(
        Func<ImageApproximationFitness, PolygonFactory, UniformPolygonCrossover, PolygonMutation, TournamentSelectionStrategy<List<ColoredPolygon>>, ParallelBaseGeneticEngine<List<ColoredPolygon>>> engineFactory)
    {
        Pixel[] targetPixels = ImageLoader.LoadTargetImage(TargetImagePath, out int width, out int height);
        
        var fitness = new ImageApproximationFitness(targetPixels, width, height);
        var factory = new PolygonFactory(PolygonCount, width, height);
        var crossover = new UniformPolygonCrossover();
        var mutation = new PolygonMutation(width, height);
        var selection = new TournamentSelectionStrategy<List<ColoredPolygon>>();

        double totalFitness = 0;
        
        for (int i = 0; i < TestRuns; i++)
        {
            var engine = engineFactory(fitness, factory, crossover, mutation, selection);
            
            var bestChromosome = engine.Run(Generations);
            
            totalFitness += fitness.EvaluateFitness(bestChromosome);
        }
        
        return totalFitness / TestRuns;
    }
}