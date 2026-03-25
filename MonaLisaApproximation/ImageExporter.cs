using MonaLisaApproximation.Domains;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MonaLisaApproximation;

public static class ImageExporter
{
    public static void SaveChromosomeToDisk(List<ColoredPolygon> polygons, int width, int height, string filePath)
    {
        // Ensure the directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (Image<Rgba32> canvas = new Image<Rgba32>(width, height, new Rgba32(0, 0, 0, 255)))
        {
            canvas.Mutate((IImageProcessingContext ctx) =>
            {
                var options = new DrawingOptions { GraphicsOptions = new GraphicsOptions { Antialias = false } };

                foreach (var poly in polygons)
                {
                    SixLabors.ImageSharp.Color color = new SixLabors.ImageSharp.Color(new Rgba32(poly.R, poly.G, poly.B, poly.A));
                    var points = new SixLabors.ImageSharp.PointF[poly.Vertices.Length];
                    
                    for (int i = 0; i < poly.Vertices.Length; i++)
                    {
                        points[i] = new SixLabors.ImageSharp.PointF(poly.Vertices[i].X, poly.Vertices[i].Y);
                    }

                    ctx.FillPolygon(options, color, points);
                }
            });
            
            canvas.SaveAsPng(filePath);
        }
    }
}