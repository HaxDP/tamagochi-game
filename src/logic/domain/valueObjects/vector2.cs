namespace ConsoleTamagotchi.Domain.ValueObjects;

public readonly record struct Vector2(double X, double Y)
{
    public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);
    public static Vector2 operator *(Vector2 left, double scalar) => new(left.X * scalar, left.Y * scalar);
}
