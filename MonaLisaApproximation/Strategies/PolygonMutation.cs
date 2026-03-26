using GeneticAlgorithm.Interfaces;
using MonaLisaApproximation.Domains;

namespace MonaLisaApproximation.Strategies;

using System;
using System.Collections.Generic;

public class PolygonMutation : IMutationStrategy<List<ColoredPolygon>>
{
    private readonly int _canvasWidth;
    private readonly int _canvasHeight;

    public PolygonMutation(int canvasWidth, int canvasHeight)
    {
        _canvasWidth = canvasWidth;
        _canvasHeight = canvasHeight;
    }

    public List<ColoredPolygon> Mutate(List<ColoredPolygon> chromosome)
    {
        // 1. Create a copy of the list so we don't alter the original
        var mutatedDna = new List<ColoredPolygon>(chromosome);

        // 2. Pick ONE random polygon to mutate
        int polyIndex = Random.Shared.Next(mutatedDna.Count);
        var targetPoly = mutatedDna[polyIndex];

        // 3. Roll a dice to decide WHAT to mutate on this polygon
        int mutationType = Random.Shared.Next(3);

        switch (mutationType)
        {
            case 0: // Mutate a coordinate (Nudge it by a few pixels)
                int vertexIndex = Random.Shared.Next(targetPoly.Vertices.Length);
                
                // Nudge between -10 and +10 pixels
                float nudgeX = (float)(Random.Shared.NextDouble() * 20 - 10);
                float nudgeY = (float)(Random.Shared.NextDouble() * 20 - 10);
                
                targetPoly.Vertices[vertexIndex].X = Math.Clamp(targetPoly.Vertices[vertexIndex].X + nudgeX, 0, _canvasWidth);
                targetPoly.Vertices[vertexIndex].Y = Math.Clamp(targetPoly.Vertices[vertexIndex].Y + nudgeY, 0, _canvasHeight);
                break;

            case 1: // Mutate Color (R, G, or B)
                int colorChannel = Random.Shared.Next(3);
                int colorShift = Random.Shared.Next(-20, 21); // Shift by up to 20 points
                
                if (colorChannel == 0) targetPoly.R = (byte)Math.Clamp(targetPoly.R + colorShift, 0, 255);
                if (colorChannel == 1) targetPoly.G = (byte)Math.Clamp(targetPoly.G + colorShift, 0, 255);
                if (colorChannel == 2) targetPoly.B = (byte)Math.Clamp(targetPoly.B + colorShift, 0, 255);
                break;

            case 2: // Mutate Alpha/Transparency
                int alphaShift = Random.Shared.Next(-10, 11);
                targetPoly.A = (byte)Math.Clamp(targetPoly.A + alphaShift, 10, 255); // Keep it slightly visible
                break;
        }

        // 4. Put the mutated polygon back into the list
        mutatedDna[polyIndex] = targetPoly;

        return mutatedDna;
    }
}