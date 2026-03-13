namespace GeneticAlgorithm.Interfaces;

public interface ISelectionStrategy<T>
{
    IEnumerable<Individual<T>> Select(IEnumerable<Individual<T>> population, int count);
}