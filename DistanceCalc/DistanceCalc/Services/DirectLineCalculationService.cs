using DistanceCalc.Abcstractions;
using DistanceCalc.Models;

namespace DistanceCalc.Services;

/// <summary>
/// Сервис расчёта растояния "По прямой линии"
/// </summary>
internal class DirectLineCalculationService : CalculationServiceAbstraction
{
    /// <summary>
    /// Рассчитывает фактическое расстояние между точками по прямой линии
    /// </summary>
    /// <remarks>Применена простая теорема Пифагора</remarks>
    /// <param name="A">Точка А</param>
    /// <param name="B">Точка Б</param>
    /// <param name="cancellationToken">Токен отмены работы запроса на расчёт</param>
    /// <returns>Расстояние в километрах от точки А до точки Б</returns>
    protected override async Task<double> GetDistanceAsync(Point2D A, Point2D B, CancellationToken cancellationToken)
    {
        // Имитация долгой поэтапной работы с возможной отменой задачи (в текущей задаче бессмысленно - демонстрирует будущее расширение)
        for (int i = 0; i < 5; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(300, cancellationToken); // освобождает процессор для выполнения других потоков пула
        }

        return Math.Sqrt(Math.Pow(A.X + B.X, 2) + Math.Pow(A.Y + B.Y, 2));
    }
}
