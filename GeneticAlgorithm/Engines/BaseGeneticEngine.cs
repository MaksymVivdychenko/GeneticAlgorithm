using GeneticAlgorithm.ExtensionMethods;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public abstract class BaseGeneticEngine<TChromosome>
{
    protected readonly IFitnessEvaluator<TChromosome> FitnessEvaluator;
    private readonly ICrossoverStrategy<TChromosome> _crossoverStrategy;
    private readonly IMutationStrategy<TChromosome> _mutationStrategy;
    private readonly ISelectionStrategy<TChromosome> _selectionStrategy;
    private readonly IIndividualFactory<TChromosome> _factory;
    private readonly int _populationSize;
    private readonly int _elitismCount;
    private readonly int _mutationRate;

    public BaseGeneticEngine(IFitnessEvaluator<TChromosome> fitnessEvaluator,
        ICrossoverStrategy<TChromosome> crossoverStrategy,
        IMutationStrategy<TChromosome> mutationStrategy,
        ISelectionStrategy<TChromosome> selectionStrategy,
        IIndividualFactory<TChromosome> factory,
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

    public TChromosome Run(int generations)
    {
        var initialChromosomes = new List<TChromosome>(_populationSize);
        for (int i = 0; i < _populationSize; i++)
        {
            initialChromosomes.Add(_factory.CreateRandomChromosome());
        }

        Random rand = new Random();

        var population = FitPopulation(initialChromosomes);
        for (int i = 0; i < generations; i++)
        {
            var eliteIndividuals = population
                .TakeMax(p => p.Fitness, _elitismCount);
            List<TChromosome> chromosomes = new List<TChromosome>();
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
            evaluatedPopulation.AddRange(eliteIndividuals);
            population = evaluatedPopulation;
        }

        return population.MaxBy(p => p.Fitness)!.Chromosome;
    }

    protected abstract IList<Individual<TChromosome>> FitPopulation(IEnumerable<TChromosome> chromosomes);
}