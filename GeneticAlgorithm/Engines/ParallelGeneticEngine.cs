using System.Collections.Concurrent;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticEngine<T> : ParallelBaseGeneticEngine<T>
{
    public ParallelGeneticEngine(IFitnessEvaluator<T> fitnessEvaluator, ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy, ISelectionStrategy<T> selectionStrategy, IIndividualFactory<T> factory,
        int populationSize, int elitismCount, int mutationRate, int threadCount) : base(fitnessEvaluator,
        crossoverStrategy, mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate,
        threadCount)
    {
    }

    protected override IList<Individual<T>> FitPopulation(IEnumerable<T> chromosomes)
    {
        var evaluatedPopulation = new ConcurrentBag<Individual<T>>();
        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = ThreadCount
        };

        Parallel.ForEach(chromosomes, options, c =>
        {
            evaluatedPopulation.Add(new Individual<T>
            {
                Chromosome = c,
                Fitness = FitnessEvaluator.EvaluateFitness(c)
            });
        });

        return evaluatedPopulation.ToList();
    }
}