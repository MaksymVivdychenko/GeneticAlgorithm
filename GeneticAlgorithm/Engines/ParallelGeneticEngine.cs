using System.Collections.Concurrent;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticEngine<TChromosome> : ParallelBaseGeneticEngine<TChromosome>
{
    public ParallelGeneticEngine(IFitnessEvaluator<TChromosome> fitnessEvaluator, ICrossoverStrategy<TChromosome> crossoverStrategy,
        IMutationStrategy<TChromosome> mutationStrategy, ISelectionStrategy<TChromosome> selectionStrategy, IIndividualFactory<TChromosome> factory,
        int populationSize, int elitismCount, int mutationRate, int threadCount) : base(fitnessEvaluator,
        crossoverStrategy, mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate,
        threadCount)
    {
    }

    protected override IList<Individual<TChromosome>> FitPopulation(IEnumerable<TChromosome> chromosomes)
    {
        var evaluatedPopulation = new ConcurrentBag<Individual<TChromosome>>();
        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = ThreadCount
        };

        Parallel.ForEach(chromosomes, options, c =>
        {
            evaluatedPopulation.Add(new Individual<TChromosome>
            {
                Chromosome = c,
                Fitness = FitnessEvaluator.EvaluateFitness(c)
            });
        });

        return evaluatedPopulation.ToList();
    }
}