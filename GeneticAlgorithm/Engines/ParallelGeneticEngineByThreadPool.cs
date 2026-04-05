using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticEngineByThreadPool<TChromosome> : ParallelBaseGeneticEngine<TChromosome>
{
    public ParallelGeneticEngineByThreadPool(IFitnessEvaluator<TChromosome> fitnessEvaluator,
        ICrossoverStrategy<TChromosome> crossoverStrategy, IMutationStrategy<TChromosome> mutationStrategy,
        ISelectionStrategy<TChromosome> selectionStrategy, IIndividualFactory<TChromosome> factory, int populationSize, int elitismCount,
        int mutationRate, int threadCount) : base(fitnessEvaluator, crossoverStrategy, mutationStrategy,
        selectionStrategy, factory, populationSize, elitismCount, mutationRate, threadCount)
    {
    }

    protected override IList<Individual<TChromosome>> FitPopulation(IEnumerable<TChromosome> chromosomes)
    {
        var chromosomesArray = chromosomes.ToArray();
        int totalItems = chromosomesArray.Length;
        var evaluatedPopulation = new Individual<TChromosome>[totalItems];

        int threadCount = totalItems > ThreadCount ? ThreadCount : totalItems;
        int chunkSize = totalItems / threadCount;
        int remainder = totalItems % threadCount;

        int pendingTasks = threadCount;
        int startIndex;
        int endIndex = 0;

        using (var doneEvent = new ManualResetEvent(false))
        {
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
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        for (int j = finalStartIndex; j < finalEndIndex; j++)
                        {
                            var fitness = FitnessEvaluator.EvaluateFitness(chromosomesArray[j]);
                            evaluatedPopulation[j] = new Individual<TChromosome>
                            {
                                Chromosome = chromosomesArray[j],
                                Fitness = fitness
                            };
                        }
                    }
                    finally
                    {
                        if (Interlocked.Decrement(ref pendingTasks) == 0)
                        {
                            doneEvent.Set();
                        }
                    }
                });
            }

            doneEvent.WaitOne();
        }

        return evaluatedPopulation;
    }
}