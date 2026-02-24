namespace GeneticAlgorithm;

public abstract class Individual<T>
{
    public T[] Chromosome { get; private set; }
    public double Fitness { get; private set; }
    public int MutationRate { get; set; } = 10;

    public abstract void CalculateFitness();
    public abstract Individual<T> Crossing(Individual<T> individual);
    public abstract void Mutate();
    public abstract void Init();
}