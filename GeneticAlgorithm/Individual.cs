namespace GeneticAlgorithm;

public class Individual<T>
{
    public required T Chromosome { get; init; }
    public required double Fitness { get; init; }
}