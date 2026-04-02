using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public abstract class ParallelBaseGeneticEngine<T> : BaseGeneticEngine<T>
{
    protected readonly int ThreadCount;

    protected ParallelBaseGeneticEngine(IFitnessEvaluator<T> fitnessEvaluator, ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy, ISelectionStrategy<T> selectionStrategy, IIndividualFactory<T> factory,
        int populationSize, int elitismCount, int mutationRate, int threadCount) : base(fitnessEvaluator,
        crossoverStrategy, mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate)
    {
        ThreadCount = threadCount;
    }
}