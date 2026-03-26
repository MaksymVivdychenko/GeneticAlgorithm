using GeneticAlgorithm.ExtensionMethods;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class GeneticAlgorithmEngine<T>
{
    protected readonly IFitnessEvaluator<T> FitnessEvaluator;
    private readonly ICrossoverStrategy<T> _crossoverStrategy;
    private readonly IMutationStrategy<T> _mutationStrategy;
    private readonly ISelectionStrategy<T> _selectionStrategy;
    private readonly IIndividualFactory<T> _factory;
    protected readonly int ThreadCount;

    public GeneticAlgorithmEngine(IFitnessEvaluator<T> fitnessEvaluator,
        ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy,
        ISelectionStrategy<T> selectionStrategy,
        IIndividualFactory<T> factory,
        int threadCount
        )
    {
        this.FitnessEvaluator = fitnessEvaluator;
        _crossoverStrategy = crossoverStrategy;
        _mutationStrategy = mutationStrategy;
        _selectionStrategy = selectionStrategy;
        _factory = factory;
        ThreadCount = threadCount;
    }

    public T Run(int generations, int populationSize, int elitismCount, int mutationRate)
    {
        var initialChromosomes = new List<T>(populationSize);
        for (int i = 0; i < populationSize; i++)
        {
            initialChromosomes.Add(_factory.CreateRandomChromosome());
        }

        Random rand = new Random();

        var population = FitPopulation(initialChromosomes);
        for (int i = 0; i < generations; i++)
        {
            if (i % 100 == 0)
            {
                Console.WriteLine($"Generation: {i}");
            }
            var elitaries = population
                .TakeMax(p => p.Fitness, elitismCount);
            List<T> chromosomes = new List<T>();
            for (int j = 0; j < population.Count - elitismCount; j++)
            {
                var parents = _selectionStrategy.Select(population, 2).ToArray();
                var child = _crossoverStrategy.Crossover(parents[0].Chromosome, parents[1].Chromosome);
                if (rand.Next(100) < mutationRate)
                {
                    child = _mutationStrategy.Mutate(child);
                }

                chromosomes.Add(child);
            }

            var evaluatedPopulation = FitPopulation(chromosomes).ToList();
            evaluatedPopulation.AddRange(elitaries);
            population = evaluatedPopulation;
        }

        return population.MaxBy(p => p.Fitness)!.Chromosome;
    }

    protected virtual IList<Individual<T>> FitPopulation(IEnumerable<T> chromosomes)
    {
        var evaluatedPopulation = new List<Individual<T>>();

        foreach (var c in chromosomes)
        {
            evaluatedPopulation.Add(new Individual<T>
            {
                Chromosome = c,
                Fitness = FitnessEvaluator.EvaluateFitness(c)
            });
        }

        return evaluatedPopulation;
    }
}