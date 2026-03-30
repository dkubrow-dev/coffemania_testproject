using DistanceCalc.Abstractions;
using DistanceCalc.Models;

namespace DistanceCalc.Services;

/// <summary>
/// Сервис расчёта расстояния "По прямой линии"
/// </summary>
internal class DirectLineCalculationService : CalculationServiceBase
{
    /// <summary>
    /// Возвращает настроенный сервис расчёта расстояния "По прямой линии"
    /// </summary>
    /// <param name="settings">Сервис настроек</param>
    internal DirectLineCalculationService(ISettingsProvider settings) : base(settings) { }

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
        if (_settings.SimulateDelayMs > 0)
        {
            // Имитация долгой работы с возможной отменой задачи
            //    (в текущей задаче исключительно отладочная вещь, в реальности весь блок и настройка не требуются)
            await Task.Delay(_settings.SimulateDelayMs, cancellationToken);
        }

        return (B - A).Length;
    }
}
