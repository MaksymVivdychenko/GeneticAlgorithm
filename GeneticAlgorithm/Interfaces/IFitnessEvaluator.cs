namespace GeneticAlgorithm.Interfaces;

public interface IFitnessEvaluator<in T>
{
    public double EvaluateFitness(T chromosome);
}