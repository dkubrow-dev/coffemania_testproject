namespace DistanceCalc.Models;

/// <summary>
/// Точка двухмерного пространства для определения положения
/// </summary>
/// <remarks>Не наследуемый</remarks>
/// <remarks>При отсутствии координат вернёт точку (0, 0)</remarks>
[Serializable]
public sealed record Point2D
{
    /// <summary>
    /// Возвращает новую точку пространства по заданным координатам.
    /// </summary>
    /// <param name="x">Абсциса</param>
    /// <param name="y">Ордината</param>
    public Point2D(double x = default, double y = default)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Координата по оси oX
    /// </summary>
    public double X { get; init; }

    /// <summary>
    /// Координата по оси oY
    /// </summary>
    public double Y { get; init; }

    /// <summary>
    /// Возвращает результат математического сложения точек (вектора)
    /// </summary>
    public static Point2D operator +(Point2D left, Point2D right)
        => new(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Возвращает результат математического вычитания точек (вектора)
    /// </summary>
    public static Point2D operator -(Point2D left, Point2D right)
        => new(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Возвращает длину вектора от точки (0, 0) до текущих координат
    /// </summary>
    public double Length => Math.Sqrt(X * X + Y * Y);

    /// <summary>
    /// Строкове представление
    /// </summary>
    /// <remarks>Для удобства отладки</remarks>
    /// <returns>Строка в формате "(абсциса, ордината)"</returns>
    public override string ToString() => $"({X}, {Y})";
}