namespace Day14;

public readonly record struct Coordinate(int X, int Y)
{
    public static Coordinate operator +(Coordinate a, Coordinate b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Coordinate operator -(Coordinate a, Coordinate b)
        => new(a.X - b.X, a.Y - b.Y);
    
    public static Coordinate operator *(Coordinate a, int i)
        => new(a.X * i, a.Y * i);
    
    public static Coordinate operator %(Coordinate a, Coordinate b)
        => new(a.X % b.X, a.Y % b.Y);
}
