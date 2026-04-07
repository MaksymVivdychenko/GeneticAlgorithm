using GeneticAlgorithm.ExtensionMethods;
using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.Engines;

public class SequentialGeneticEngine<TChromosome> : BaseGeneticEngine<TChromosome>
{
    public SequentialGeneticEngine(IFitnessEvaluator<TChromosome> fitnessEvaluator, ICrossoverStrategy<TChromosome> crossoverStrategy,
        IMutationStrategy<TChromosome> mutationStrategy, ISelectionStrategy<TChromosome> selectionStrategy, IIndividualFactory<TChromosome> factory,
        int populationSize, int elitismCount, int mutationRate) : base(fitnessEvaluator, crossoverStrategy,
        mutationStrategy, selectionStrategy, factory, populationSize, elitismCount, mutationRate)
    {
    }

    protected override IList<Individual<TChromosome>> FitPopulation(IEnumerable<TChromosome> chromosomes)
    {
        var evaluatedPopulation = new List<Individual<TChromosome>>();

        foreach (var c in chromosomes)
        {
            evaluatedPopulation.Add(new Individual<TChromosome>
            {
                Chromosome = c,
                Fitness = FitnessEvaluator.EvaluateFitness(c)
            });
        }

        return evaluatedPopulation;
    }
}