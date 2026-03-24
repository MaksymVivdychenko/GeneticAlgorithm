namespace GeneticAlgorithm.Interfaces;

public interface ISelectionStrategy<T>
{
    IEnumerable<Individual<T>> Select(IList<Individual<T>> population, int count);
}