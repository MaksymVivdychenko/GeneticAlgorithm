using System.Collections.Concurrent;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticAlgorithmEngine<T> : GeneticAlgorithmEngine<T>
{
    public ParallelGeneticAlgorithmEngine(IFitnessEvaluator<T> fitnessEvaluator,
        ICrossoverStrategy<T> crossoverStrategy, IMutationStrategy<T> mutationStrategy,
        ISelectionStrategy<T> selectionStrategy, IIndividualFactory<T> factory, int threadCount) : base(
        fitnessEvaluator, crossoverStrategy, mutationStrategy, selectionStrategy, factory, threadCount)
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