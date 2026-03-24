using System;
using System.Collections.Generic;
using GeneticAlgorithm.Interfaces;
using MonaLisaApproximation.Domains;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using PointF = SixLabors.ImageSharp.PointF;

namespace MonaLisaApproximation.Strategies;

public class ImageApproximationFitness : IFitnessEvaluator<List<ColoredPolygon>>
{
    private readonly Pixel[] _targetPixels;
    private readonly int _width;
    private readonly int _height;

    public ImageApproximationFitness(Pixel[] targetPixels, int width, int height)
    {
        _targetPixels = targetPixels;
        _width = width;
        _height = height;
    }

    public double EvaluateFitness(List<ColoredPolygon> chromosome)
    {
        // 1. Render polygons to an isolated memory buffer
        Pixel[] renderedPixels = RasterizeToMemory(chromosome, _width, _height);

        // 2. Calculate the difference (MSE)
        double totalError = 0;
        
        for (int i = 0; i < _targetPixels.Length; i++)
        {
            var target = _targetPixels[i];
            var rendered = renderedPixels[i];

            // Calculate squared error for each color channel
            double rDiff = target.R - rendered.R;
            double gDiff = target.G - rendered.G;
            double bDiff = target.B - rendered.B;

            totalError += (rDiff * rDiff) + (gDiff * gDiff) + (bDiff * bDiff);
        }

        // We want the HIGHEST fitness score. 
        // Zero error is perfect, so we return a negative error value.
        return -totalError; 
    }

    private Pixel[] RasterizeToMemory(List<ColoredPolygon> polygons, int width, int height)
    {
        var buffer = new Pixel[width * height];

        // Modern ImageSharp requires a pure Rgba32 struct to initialize the background color
        using (Image<Rgba32> canvas = new Image<Rgba32>(width, height, new Rgba32(0, 0, 0, 255)))
        {
            canvas.Mutate(ctx =>
            {
                var options = new DrawingOptions 
                { 
                    GraphicsOptions = new GraphicsOptions { Antialias = false } 
                };

                foreach (var poly in polygons)
                {
                    // Fixed: Color.FromRgba is deprecated. Use the Color constructor with Rgba32.
                    var color = new Color(new Rgba32(poly.R, poly.G, poly.B, poly.A));

                    var points = new PointF[poly.Vertices.Length];
                    for (int i = 0; i < poly.Vertices.Length; i++)
                    {
                        points[i] = new PointF(poly.Vertices[i].X, poly.Vertices[i].Y);
                    }

                    // This will now resolve perfectly with the Drawing package installed
                    ctx.FillPolygon(options, color, points);
                }
            });

            // OPTIMIZED: Ultra-fast pixel reading using Memory Spans instead of the [x,y] indexer
            canvas.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        // Get a reference to the pixel in memory
                        ref Rgba32 pixel = ref pixelRow[x];
                    
                        // Map it directly to our 1D flat buffer
                        buffer[y * width + x] = new Pixel { R = pixel.R, G = pixel.G, B = pixel.B };
                    }
                }
            });
        } 

        return buffer;
    }
}