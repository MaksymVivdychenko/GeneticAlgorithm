using GeneticAlgorithm.ExtensionMethods;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class SequentialGeneticEngine<T> : BaseGeneticEngine<T>
{
    public SequentialGeneticEngine(IFitnessEvaluator<T> fitnessEvaluator, ICrossoverStrategy<T> crossoverStrategy,
        IMutationStrategy<T> mutationStrategy, ISelectionStrategy<T> selectionStrategy, IIndividualFactory<T> factory,
        int populationSize, int elitismCount, int mutationRate) : base(fitnessEvaluator, crossoverStrategy,
        mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate)
    {
    }

    protected override IList<Individual<T>> FitPopulation(IEnumerable<T> chromosomes)
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