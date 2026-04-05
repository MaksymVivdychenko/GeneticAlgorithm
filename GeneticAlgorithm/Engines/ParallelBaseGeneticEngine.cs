using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public abstract class ParallelBaseGeneticEngine<TChromosome> : BaseGeneticEngine<TChromosome>
{
    protected readonly int ThreadCount;

    protected ParallelBaseGeneticEngine(IFitnessEvaluator<TChromosome> fitnessEvaluator, ICrossoverStrategy<TChromosome> crossoverStrategy,
        IMutationStrategy<TChromosome> mutationStrategy, ISelectionStrategy<TChromosome> selectionStrategy, IIndividualFactory<TChromosome> factory,
        int populationSize, int elitismCount, int mutationRate, int threadCount) : base(fitnessEvaluator,
        crossoverStrategy, mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate)
    {
        ThreadCount = threadCount;
    }
}