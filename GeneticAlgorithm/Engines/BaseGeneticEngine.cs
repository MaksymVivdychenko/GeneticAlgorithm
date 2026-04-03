using GeneticAlgorithm.ExtensionMethods;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public abstract class BaseGeneticEngine<T>
{
    protected readonly IFitnessEvaluator<T> FitnessEvaluator;
    private readonly ICrossoverStrategy<T> _crossoverStrategy;
    private readonly IMutationStrategy<T> _mutationStrategy;
    private readonly ISelectionStrategy<T> _selectionStrategy;
    private readonly IIndividualFactory<T> _factory;
    private readonly int _populationSize;
    private readonly int _elitismCount;
    private readonly int _mutationRate;

    public BaseGeneticEngine(IFitnessEvaluator<T> fitnessEvaluator,
        ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy,
        ISelectionStrategy<T> selectionStrategy,
        IIndividualFactory<T> factory,
        int populationSize,
        int elitismCount,
        int mutationRate
        )
    {
        FitnessEvaluator = fitnessEvaluator;
        _crossoverStrategy = crossoverStrategy;
        _mutationStrategy = mutationStrategy;
        _selectionStrategy = selectionStrategy;
        _factory = factory;
        _populationSize = populationSize;
        _elitismCount = elitismCount;
        _mutationRate = mutationRate;
    }

    public T Run(int generations)
    {
        var initialChromosomes = new List<T>(_populationSize);
        for (int i = 0; i < _populationSize; i++)
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
                .TakeMax(p => p.Fitness, _elitismCount);
            List<T> chromosomes = new List<T>();
            for (int j = 0; j < population.Count - _elitismCount; j++)
            {
                var parents = _selectionStrategy.Select(population, 2).ToArray();
                var child = _crossoverStrategy.Crossover(parents[0].Chromosome, parents[1].Chromosome);
                if (rand.Next(100) < _mutationRate)
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

    protected abstract IList<Individual<T>> FitPopulation(IEnumerable<T> chromosomes);
}