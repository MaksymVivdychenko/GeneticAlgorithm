namespace GeneticAlgorithm.Interfaces;

public interface ICrossoverStrategy<T>
{
    public T Crossover(T parent1, T parent2);
}