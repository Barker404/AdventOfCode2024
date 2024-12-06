namespace Day06;

public readonly record struct Coordinate(int X, int Y)
{
    public static Coordinate operator +(Coordinate a, Coordinate b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Coordinate operator -(Coordinate a, Coordinate b)
        => new(a.X - b.X, a.Y - b.Y);
}
