using GeneticAlgorithm;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgoKnapsack;

public record KnapsackItem(string Name, double Weight, double Value);

// 1. Fitness Evaluator
// Calculates total value. If weight exceeds capacity, fitness is 0 (or heavily penalized).
public class KnapsackFitnessEvaluator : IFitnessEvaluator<bool[]>
{
    private readonly IList<KnapsackItem> _items;
    private readonly double _maxCapacity;

    public KnapsackFitnessEvaluator(IList<KnapsackItem> items, double maxCapacity)
    {
        _items = items;
        _maxCapacity = maxCapacity;
    }

    public double EvaluateFitness(bool[] chromosome)
    {
        double totalWeight = 0;
        double totalValue = 0;

        for (int i = 0; i < chromosome.Length; i++)
        {
            if (chromosome[i])
            {
                totalWeight += _items[i].Weight;
                totalValue += _items[i].Value;
            }
        }

        // If the knapsack is too heavy, the solution is invalid.
        if (totalWeight > _maxCapacity) return 0; 
        
        return totalValue;
    }
}

// 2. Individual Factory
// Generates completely random initial guesses (true/false for each item).
public class KnapsackFactory : IIndividualFactory<bool[]>
{
    private readonly int _numberOfItems;
    private readonly Random _random = new Random();

    public KnapsackFactory(int numberOfItems)
    {
        _numberOfItems = numberOfItems;
    }

    public bool[] CreateRandomChromosome()
    {
        var chromosome = new bool[_numberOfItems];
        for (int i = 0; i < _numberOfItems; i++)
        {
            chromosome[i] = _random.NextDouble() >= 0.5;
        }
        return chromosome;
    }
}

// 3. Crossover Strategy
// Single-point crossover: Splits two parents and combines them.
public class SinglePointCrossover : ICrossoverStrategy<bool[]>
{
    private readonly Random _random = new Random();

    public bool[] Crossover(bool[] parent1, bool[] parent2)
    {
        var child = new bool[parent1.Length];
        int splitPoint = _random.Next(parent1.Length);

        for (int i = 0; i < parent1.Length; i++)
        {
            child[i] = i < splitPoint ? parent1[i] : parent2[i];
        }
        return child;
    }
}

// 4. Mutation Strategy
// Flips a single random bit in the array to introduce new genetic material.
public class BitFlipMutation : IMutationStrategy<bool[]>
{
    private readonly Random _random = new Random();

    public bool[] Mutate(bool[] chromosome)
    {
        var mutated = (bool[])chromosome.Clone();
        int flipIndex = _random.Next(mutated.Length);
        mutated[flipIndex] = !mutated[flipIndex]; // Flip the bit
        return mutated;
    }
}

// 5. Selection Strategy
// Tournament Selection: Picks two random individuals and returns the better one.
public class TournamentSelection : ISelectionStrategy<bool[]>
{
    private readonly Random _random = new Random();

    public IEnumerable<Individual<bool[]>> Select(IList<Individual<bool[]>> population, int count)
    {
        var selected = new List<Individual<bool[]>>();
        for (int i = 0; i < count; i++)
        {
            var p1 = population[_random.Next(population.Count)];
            var p2 = population[_random.Next(population.Count)];
            
            selected.Add(p1.Fitness > p2.Fitness ? p1 : p2);
        }
        return selected;
    }
    
}