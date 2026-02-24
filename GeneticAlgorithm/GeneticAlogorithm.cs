namespace GeneticAlgorithm;

public class GeneticAlgorithm<T>
{
    private List<Individual<T>> Population { get; set; }
    private int Generations { get; set; }
    private Random Random { get; set; } = new Random();

    public GeneticAlgorithm(List<Individual<T>> individuals, int generations)
    {
        Population = individuals;
        Generations = generations;
    }

    private Individual<T> SelectParentTournament()
    {
        List<Individual<T>> pretenders = new List<Individual<T>>();
        for (int i = 0; i < 3; i++)
        {
            pretenders.Add(Population[Random.Next(Population.Count)]);
        }

        return pretenders.MaxBy(p => p.Fitness)!;
    }

    public void Run()
    {
        CalculateFitness();
        for (int i = 0; i < Generations; i++)
        {
            var elite1 = Population.MaxBy(q => q.Fitness) ?? throw new Exception();
            Population.Remove(elite1);
            var elite2 = Population.MaxBy(q => q.Fitness) ?? throw new Exception();
            Population.Clear();
            
            for (int j = 0; j < Population.Count - 2; j++)
            {
                var par1 = SelectParentTournament();
                var par2 = SelectParentTournament();
                Population.Add(par1.Crossing(par2));
            }
            
            CalculateFitness();

            Population.Add(elite1);
            Population.Add(elite2);
        }
    }

    private void CalculateFitness()
    {
        foreach (var p in Population)
        {
            p.CalculateFitness();
        }
    }
}