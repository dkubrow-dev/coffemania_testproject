using DistanceCalc.Abstractions;
using DistanceCalc.Models;
using Microsoft.Extensions.Logging;

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
    /// <param name="logger">Логгер, провайдер которого обеспечивается вызывающим кодом</param>
    internal DirectLineCalculationService(ISettingsProvider settings, ILogger? logger = null)
        : base(settings, logger) { }

    /// <summary>
    /// Рассчитывает фактическое расстояние между точками по прямой линии
    /// </summary>
    /// <remarks>Применена простая теорема Пифагора</remarks>
    /// <param name="A">Точка А</param>
    /// <param name="B">Точка Б</param>
    /// <param name="logId">Идентификатор текущего запроса на расчёт для логгирования</param>
    /// <param name="cancellationToken">Токен отмены работы запроса на расчёт</param>
    /// <returns>Расстояние в километрах от точки А до точки Б</returns>
    protected override async Task<double> GetDistanceAsync(Point2D A, Point2D B, string logId, CancellationToken cancellationToken)
    {
        if (_settings.SimulateDelayMs > 0)
        {
            // Имитация долгой работы с возможной отменой задачи
            //    (в текущей задаче исключительно отладочная вещь, в реальности весь блок и настройка не требуются)

            _logger.LogDebug("{logId} - Simulating calculation delay. DelayMs={DelayMs}",
                logId, _settings.SimulateDelayMs);

            await Task.Delay(_settings.SimulateDelayMs, cancellationToken);
        }

        double distance = (B - A).Length;

        _logger.LogDebug("{logId} - Direct line distance calculated. PointA={PointA}, PointB={PointB}, Distance={Distance}",
            logId, A, B, distance);

        return distance;
    }
}
