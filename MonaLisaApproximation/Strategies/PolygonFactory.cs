using GeneticAlgorithm.Interfaces;
using MonaLisaApproximation.Domains;

namespace MonaLisaApproximation.Strategies;

public class PolygonFactory : IIndividualFactory<List<ColoredPolygon>>
{
    private readonly int _polygonCount;
    private readonly int _canvasWidth;
    private readonly int _canvasHeight;

    public PolygonFactory(int polygonCount, int canvasWidth, int canvasHeight)
    {
        _polygonCount = polygonCount;
        _canvasWidth = canvasWidth;
        _canvasHeight = canvasHeight;
    }

    public List<ColoredPolygon> CreateRandomChromosome()
    {
        var dna = new List<ColoredPolygon>(_polygonCount);

        for (int i = 0; i < _polygonCount; i++)
        {
            dna.Add(CreateRandomPolygon());
        }

        return dna;
    }

    private ColoredPolygon CreateRandomPolygon()
    {
        // Using exactly 3 vertices for triangles (fastest to render)
        var vertices = new Point2D[3];
        for (int i = 0; i < 3; i++)
        {
            vertices[i] = new Point2D
            {
                X = (float)(Random.Shared.NextDouble() * _canvasWidth),
                Y = (float)(Random.Shared.NextDouble() * _canvasHeight)
            };
        }

        return new ColoredPolygon
        {
            Vertices = vertices,
            R = (byte)Random.Shared.Next(256),
            G = (byte)Random.Shared.Next(256),
            B = (byte)Random.Shared.Next(256),
            // Keep initial transparency low so we can see overlapping blends early on
            A = (byte)Random.Shared.Next(30, 100) 
        };
    }
}