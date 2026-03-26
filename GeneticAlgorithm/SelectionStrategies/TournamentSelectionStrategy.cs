using GeneticAlgorithm.Interfaces;

namespace GeneticAlgorithm.SelectionStrategies;

public class TournamentSelectionStrategy<TChromosome> : ISelectionStrategy<TChromosome>
{
        private readonly Random _random = new Random();

        public IEnumerable<Individual<TChromosome>> Select(IList<Individual<TChromosome>> population, int count)
        {
            var selected = new List<Individual<TChromosome>>();
            for (int i = 0; i < count; i++)
            {
                var p1 = population[_random.Next(population.Count)];
                var p2 = population[_random.Next(population.Count)];
            
                selected.Add(p1.Fitness > p2.Fitness ? p1 : p2);
            }
            return selected;
        }
}