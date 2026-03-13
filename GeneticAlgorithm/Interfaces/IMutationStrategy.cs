namespace GeneticAlgorithm.Interfaces;

public interface IMutationStrategy<T>
{
    public T Mutate(T chromosome);
}