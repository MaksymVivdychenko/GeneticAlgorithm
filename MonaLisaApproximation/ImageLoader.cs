using MonaLisaApproximation.Domains;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MonaLisaApproximation;

public static class ImageLoader
{
    public static Pixel[] LoadTargetImage(string filePath, out int width, out int height)
    {
        // Load the image from disk into memory using ImageSharp
        using Image<Rgba32> image = Image.Load<Rgba32>(filePath);
        width = image.Width;
        height = image.Height;

        var pixels = new Pixel[width * height];
    
        // Flatten the 2D image into our 1D Pixel array
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var rgba = image[x, y];
                int index = y * width + x; // Calculate the 1D index
            
                pixels[index] = new Pixel 
                { 
                    R = rgba.R, 
                    G = rgba.G, 
                    B = rgba.B 
                };
            }
        }

        return pixels;
    }
}