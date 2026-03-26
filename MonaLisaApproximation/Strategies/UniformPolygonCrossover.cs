using GeneticAlgorithm.Interfaces;
using MonaLisaApproximation.Domains;

namespace MonaLisaApproximation.Strategies;

using System;
using System.Collections.Generic;

public class UniformPolygonCrossover : ICrossoverStrategy<List<ColoredPolygon>>
{
    public List<ColoredPolygon> Crossover(List<ColoredPolygon> parent1, List<ColoredPolygon> parent2)
    {
        var childDna = new List<ColoredPolygon>(parent1.Count);

        for (int i = 0; i < parent1.Count; i++)
        {
            // 50/50 chance to inherit the polygon from Parent 1 or Parent 2
            if (Random.Shared.NextDouble() < 0.5)
            {
                childDna.Add(parent1[i].Copy());
            }
            else
            {
                childDna.Add(parent2[i].Copy());
            }
        }

        return childDna;
    }
}