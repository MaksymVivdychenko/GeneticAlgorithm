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
        var arrayChromosomes = chromosomes.ToArray();
        var evaluatedPopulation = new Individual<TChromosome>[arrayChromosomes.Length];
        var options = new ParallelOptions()
        {
            MaxDegreeOfParallelism = ThreadCount
        };

        Parallel.For(0, arrayChromosomes.Length, options, i  =>
        {
            evaluatedPopulation[i] = new Individual<TChromosome>
            {
                Chromosome = arrayChromosomes[i],
                Fitness = FitnessEvaluator.EvaluateFitness(arrayChromosomes[i])
            };
        });

        return evaluatedPopulation;
    }
}