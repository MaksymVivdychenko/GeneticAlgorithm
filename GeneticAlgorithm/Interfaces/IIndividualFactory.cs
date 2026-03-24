namespace GeneticAlgorithm.Interfaces;

public interface IIndividualFactory<T>
{
    T CreateRandomChromosome();
}