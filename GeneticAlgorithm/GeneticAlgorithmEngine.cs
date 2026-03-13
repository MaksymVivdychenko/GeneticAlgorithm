using GeneticAlgorithm.ExtensionMethods;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm;

public class GeneticAlgorithmEngine<T>
{
    private readonly IFitnessEvaluator<T> _fitnessEvaluator;
    private readonly ICrossoverStrategy<T> _crossoverStrategy;
    private readonly IMutationStrategy<T> _mutationStrategy;
    private readonly ISelectionStrategy<T> _selectionStrategy;
    private IEnumerable<Individual<T>> _population;
    private readonly int _generations;
    private readonly int _mutationRate;
    private Random _random = new Random();

    public GeneticAlgorithmEngine(IFitnessEvaluator<T> fitnessEvaluator,
        ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy,
        ISelectionStrategy<T> selectionStrategy,
        IEnumerable<Individual<T>> population,
        int generations,
        int mutationRate)
    {
        _fitnessEvaluator = fitnessEvaluator;
        _crossoverStrategy = crossoverStrategy;
        _mutationStrategy = mutationStrategy;
        _selectionStrategy = selectionStrategy;
        _population = population;
        _generations = generations;
        _mutationRate = mutationRate;
    }

    public void Run()
    {
        for (int i = 0; i < _generations; i++)
        {
            int elitariesCount = 2;
            var elitaries = _population.TakeMax(p => p.Fitness, elitariesCount);
            List<T> chromosomes = new List<T>();
            for (int j = 0; j < _population.Count() - elitariesCount; j++)
            {
                var parents = _selectionStrategy.Select(_population, 2).ToArray();
                var child = _crossoverStrategy.Crossover(parents[0].Chromosome, parents[1].Chromosome);
                chromosomes.Add(_crossoverStrategy.Crossover(parents[0].Chromosome, parents[1].Chromosome));
                if (_random.Next(100) < _mutationRate)
                {
                    child = _mutationStrategy.Mutate(child);
                }

                chromosomes.Add(child);
            }

            var evaluatedPopulation = FitnessPopulation(chromosomes).ToList();
            evaluatedPopulation.AddRange(elitaries);
            _population = evaluatedPopulation;
        }
    }

    private IEnumerable<Individual<T>> FitnessPopulation(IEnumerable<T> chromosomes)
    {
        var evaluatedPopulation = new List<Individual<T>>();

        foreach (var c in chromosomes)
        {
            evaluatedPopulation.Add(new Individual<T>()
            {
                Chromosome = c,
                Fitness = _fitnessEvaluator.EvaluateFitness(c)
            });
        }

        return evaluatedPopulation;
    }
}