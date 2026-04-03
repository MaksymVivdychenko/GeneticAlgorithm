using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticEngineByThread<T> : ParallelBaseGeneticEngine<T>
{
    private readonly Thread[] _threads;


    public ParallelGeneticEngineByThread(IFitnessEvaluator<T> fitnessEvaluator, ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy, ISelectionStrategy<T> selectionStrategy, IIndividualFactory<T> factory,
        int populationSize, int elitismCount, int mutationRate, int threadCount) : base(
        fitnessEvaluator, crossoverStrategy, mutationStrategy, selectionStrategy, factory, populationSize, elitismCount,
        mutationRate, threadCount)
    {
        _threads = new Thread[threadCount];
    }

    protected override IList<Individual<T>> FitPopulation(IEnumerable<T> chromosomes)
    {
        var chromosomesArray = chromosomes.ToArray();
        int totalItems = chromosomesArray.Length;

        var evaluatedPopulation = new Individual<T>[totalItems];
        int threadCount = totalItems > ThreadCount ? ThreadCount : totalItems;
        int chunkSize = totalItems / threadCount;
        int remainder = totalItems % threadCount;
        int startIndex;
        int endIndex = 0;
        for (int i = 0; i < threadCount; i++)
        {
            startIndex = endIndex;
            endIndex = startIndex + chunkSize;
            if (remainder > 0)
            {
                endIndex++;
                remainder--;
            }

            int finalStartIndex = startIndex;
            int finalEndIndex = endIndex;

            _threads[i] = new Thread(() =>
            {
                for (int j = finalStartIndex; j < finalEndIndex; j++)
                {
                    var fitness = FitnessEvaluator.EvaluateFitness(chromosomesArray[j]);
                    evaluatedPopulation[j] = new Individual<T>
                    {
                        Chromosome = chromosomesArray[j],
                        Fitness = fitness
                    };
                }
            });

            _threads[i].Start();
        }

        foreach (var thread in _threads)
        {
            thread.Join();
        }

        return evaluatedPopulation;
    }
}