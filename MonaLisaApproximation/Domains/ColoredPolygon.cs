namespace MonaLisaApproximation.Domains;

public class ColoredPolygon
{
    public Point2D[] Vertices { get; set; }
    
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; } 
}