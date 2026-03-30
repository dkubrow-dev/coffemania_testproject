namespace DistanceCalc.Models;

/// <summary>
/// Точка двухмерного пространства для определения положения
/// </summary>
/// <remarks>Не наследуемый</remarks>
public sealed class Point2D
{
    /// <summary>
    /// Координата по оси oX
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Координата по оси oY
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Возвращает новую точку пространства по заданным координатам.
    /// </summary>
    /// <remarks>При отсутствии координат вернёт точку (0, 0)</remarks>
    /// <param name="x">Абсциса</param>
    /// <param name="y">Ордината</param>
    public Point2D(double x = default, double y = default)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Возвращает результат математического сложения точек (вектора)
    /// </summary>
    public static Point2D operator +(Point2D left, Point2D right)
        => new Point2D(left.X + right.X, left.Y + right.Y);

    /// <summary>
    /// Возвращает результат математического вычитания точек (вектора)
    /// </summary>
    public static Point2D operator -(Point2D left, Point2D right)
        => new Point2D(left.X - right.X, left.Y - right.Y);

    /// <summary>
    /// Строкове представление
    /// </summary>
    /// <remarks>Для удобства отладки</remarks>
    /// <returns>Строка в формате "(абсциса, ордината)"</returns>
    public override string ToString() => $"({X}, {Y})";
}
