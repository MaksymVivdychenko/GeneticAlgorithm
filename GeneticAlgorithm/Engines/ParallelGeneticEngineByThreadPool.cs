using System.Collections.Concurrent;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticEngineByThreadPool<TChromosome> : ParallelBaseGeneticEngine<TChromosome>
{
    public ParallelGeneticEngineByThreadPool(IFitnessEvaluator<TChromosome> fitnessEvaluator, ICrossoverStrategy<TChromosome> crossoverStrategy,
        IMutationStrategy<TChromosome> mutationStrategy, ISelectionStrategy<TChromosome> selectionStrategy, IIndividualFactory<TChromosome> factory,
        int populationSize, int elitismCount, int mutationRate, int threadCount) : base(fitnessEvaluator,
        crossoverStrategy, mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate,
        threadCount)
    {
    }

    protected override IList<Individual<TChromosome>> FitPopulation(IEnumerable<TChromosome> chromosomes)
    {
        var array = chromosomes as TChromosome[] ?? chromosomes.ToArray();
        int totalItems = array.Length;
        var results = new Individual<TChromosome>[totalItems];
        
        int actualThreads = Math.Min(ThreadCount, totalItems);
    
        int baseChunkSize = totalItems / actualThreads;
        int remainder = totalItems % actualThreads;

        using var finishedSignal = new CountdownEvent(actualThreads);

        int currentStart = 0;
        for (int i = 0; i < actualThreads; i++)
        {
            int currentChunkSize = baseChunkSize + (i < remainder ? 1 : 0);
            int start = currentStart;
            int end = start + currentChunkSize;
            currentStart = end;
            
            ThreadPool.QueueUserWorkItem(_ => 
            {
                try
                {
                    for (int j = start; j < end; j++)
                    {
                        results[j] = new Individual<TChromosome>
                        {
                            Chromosome = array[j],
                            Fitness = FitnessEvaluator.EvaluateFitness(array[j])
                        };
                    }
                }
                finally
                {
                    finishedSignal.Signal();
                }
            });
        }
        
        finishedSignal.Wait();
    
        return results;
    }
}