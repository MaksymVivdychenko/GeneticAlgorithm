namespace MonaLisaApproximation.Domains;

public class ColoredPolygon
{
    public Point2D[] Vertices { get; set; }
    
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public ColoredPolygon Copy()
    {
        var clonedVertices = new Point2D[this.Vertices.Length];
        Array.Copy(this.Vertices, clonedVertices, this.Vertices.Length);

        return new ColoredPolygon
        {
            Vertices = clonedVertices,
            R = this.R,
            G = this.G,
            B = this.B,
            A = this.A
        };
    }
}