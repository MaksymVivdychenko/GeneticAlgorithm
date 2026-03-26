using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class ParallelGeneticAlgorithmEngineByThread<T> : GeneticAlgorithmEngine<T>
{
    public ParallelGeneticAlgorithmEngineByThread(IFitnessEvaluator<T> fitnessEvaluator,
        ICrossoverStrategy<T> crossoverStrategy, IMutationStrategy<T> mutationStrategy,
        ISelectionStrategy<T> selectionStrategy, IIndividualFactory<T> factory, int threadCount) : base(fitnessEvaluator,
        crossoverStrategy, mutationStrategy, selectionStrategy, factory, threadCount)
    {
    }

    protected override IList<Individual<T>> FitPopulation(IEnumerable<T> chromosomes)
    {
        var chromosomesArray = chromosomes.ToArray();
        int totalItems = chromosomesArray.Length;

        var evaluatedPopulation = new Individual<T>[totalItems];
        int threadCount = totalItems > ThreadCount ? ThreadCount : totalItems;
        var threads =  new Thread[threadCount];
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

            threads[i] = new Thread(() =>
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

            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return evaluatedPopulation;
    }
}