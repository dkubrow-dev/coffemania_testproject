using DistanceCalc.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DistanceCalc.Abstractions;

/// <summary>
/// Базовый класс, предоставляющий для всех сервисов основной функционал:
/// реализацию интерфейса IDistanceCalculationService, а также обёртку для получения и выдачи данных.
/// </summary>
/// <remarks>Для дальнейших реализация будет унифицировано поведение разных реализаций с вызывающим кодом</remarks>
internal abstract class CalculationServiceBase : IDistanceCalculationService
{
    /// <summary>
    /// Настройки текущего исполнителя
    /// </summary>
    private protected readonly ISettingsProvider _settings;

    /// <summary>
    /// Логгер, провайдер которого передан вызывающим кодом
    /// </summary>
    private protected readonly ILogger _logger;

    /// <summary>
    /// Возвращает абстрактного исполнителя для IDistanceCalculationService
    /// </summary>
    /// <param name="settings">Настройки режима работы калькулятора</param>
    /// <param name="logger">Логгер, провайдер которого обеспечивается вызывающим кодом</param>
    internal CalculationServiceBase(ISettingsProvider settings, ILogger? logger = null)
    {
        _settings = settings;
        _logger = logger ?? NullLogger.Instance;
    }

    /// <summary>
    /// Возвращает раcстояние между двумя точками по прямой линии, соединяющей их наплоскости.
    /// </summary>
    /// <param name="data">Входные данные</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <remarks>Генерируемые исключения ложатся во внутренний объект. Не выбрасывает исключения вверх</remarks>
    /// <returns>Задачу с результирующим объектом.</returns>
    public async Task<Result> CalculateAsync(Input data, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Distance calculation started. PointA={PointA}, PointB={PointB}, Mode={Mode}",
            data.PointA, data.PointB, _settings.Mode);

        try
        {
            double distance = await GetDistanceAsync(data.PointA, data.PointB, cancellationToken);
            _logger.LogDebug("Distance calculation completed successfully.");

            return new Result
            {
                Success = true,
                Distance = distance,
                ErrorCode = ErrorCodes.NoErrors
            };
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Distance calculation was canceled.");

            return new Result
            {
                Success = false,
                ErrorCode = ErrorCodes.Canceled,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Distance calculation failed.");

            return new Result
            {
                Success = false,
                ErrorCode = ErrorCodes.InternalException,
                Message = ex.Message
            };
        }
    }

    /// <summary>
    /// Метод фактического расчёта расстояния для конкретной реализации.
    /// </summary>
    /// <param name="A">Точка А</param>
    /// <param name="B">Точка Б</param>
    /// <param name="cancellationToken">Токен отмены работы запроса на расчёт</param>
    /// <returns>Расстояние в километрах от точки А до точки Б</returns>
    protected abstract Task<double> GetDistanceAsync(Point2D A, Point2D B, CancellationToken cancellationToken);
}
