namespace DistanceCalc.Models;

/// <summary>
/// Структура входных данных для подсчёта расстояний
/// </summary>
/// <remarks>Служит для получения стандартизированного входа</remarks>
public struct Input
{
    /// <summary>
    /// Первая точка для подсчёта расстояния
    /// </summary>
    public Point2D PointA { get; set; }

    /// <summary>
    /// Вторая точка для подсчёта расстояния
    /// </summary>
    public Point2D PointB { get; set; }

    /// <summary>
    /// Возвращает комплект входных параметров, задавая значения по умолчанию.
    /// </summary>
    public Input()
    {
        PointA = new();
        PointB = new();
    }

    /// <summary>
    /// Возвращает комплект входных параметров для расчёта расстояния по указанным точкам.
    /// </summary>
    /// <param name="A">Точка А</param>
    /// <param name="B">Точка Б</param>
    public Input(Point2D A, Point2D B)
    {
        PointA = A ?? new();
        PointB = B ?? new();
    }
}
