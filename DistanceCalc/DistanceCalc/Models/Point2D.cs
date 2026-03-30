namespace DistanceCalc.Models;

/// <summary>
/// Точка двухмерного пространства для определения положения
/// </summary>
/// <remarks>Не наследуемый</remarks>
/// <remarks>
/// Возвращает новую точку пространства по заданным координатам.
/// </remarks>
/// <remarks>При отсутствии координат вернёт точку (0, 0)</remarks>
/// <param name="x">Абсциса</param>
/// <param name="y">Ордината</param>
[Serializable]
public sealed class Point2D(double x = default, double y = default)
{
    /// <summary>
    /// Координата по оси oX
    /// </summary>
    public double X { get; set; } = x;

    /// <summary>
    /// Координата по оси oY
    /// </summary>
    public double Y { get; set; } = y;

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
