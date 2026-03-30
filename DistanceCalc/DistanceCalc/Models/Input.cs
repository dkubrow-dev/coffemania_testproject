namespace DistanceCalc.Models;

/// <summary>
/// Структура входных данных для подсчёта расстояний
/// </summary>
/// <remarks>Служит для получения стандартизированного входа</remarks>
[Serializable]
public sealed record Input
{
    /// <summary>
    /// Первая точка для подсчёта расстояния
    /// </summary>
    public Point2D PointA { get; init; } = new();

    /// <summary>
    /// Вторая точка для подсчёта расстояния
    /// </summary>
    public Point2D PointB { get; init; } = new();
}
