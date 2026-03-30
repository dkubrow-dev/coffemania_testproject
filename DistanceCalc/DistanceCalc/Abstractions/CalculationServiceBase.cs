using DistanceCalc.Models;

namespace DistanceCalc.Abstractions;

/// <summary>
/// Базовый класс, предоставляющий для всех сервисов основной функционал: реализацию интерфейса IDistanceCalculationService
/// счётчик, а также обёртку для получения и выдачи данных.
/// </summary>
/// <remarks>Для дальнейших реализация будет унифицировано поведение разных реализаций с вызывающим кодом</remarks>
internal abstract class CalculationServiceBase : IDistanceCalculationService
{
    /// <summary>
    /// Настройки текущего исполнителя
    /// </summary>
    private protected ISettingsProvider _settings;

    /// <summary>
    /// Возвращает абстрактного исполнителя для IDistanceCalculationService
    /// </summary>
    /// <param name="settings">Настройки режима работы калькулятора</param>
    internal CalculationServiceBase(ISettingsProvider settings) => _settings = settings;

    /// <summary>
    /// Возвращает раcстояние между двумя точками по прямой линии, соединяющей их наплоскости.
    /// </summary>
    /// <param name="data">Входные данные</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <remarks>Генерируемые исключения ложатся во внутренний объект. Не выбрасывает исключения вверх</remarks>
    /// <returns>Задачу с результирующим объектом.</returns>
    public async Task<Result> CalculateAsync(Input data, CancellationToken cancellationToken = default)
    {
        try
        {
            return new Result
            {
                Success = true,
                Distance = await GetDistanceAsync(data.PointA, data.PointB, cancellationToken),
                ErrorCode = ErrorCodes.NoErrors
            };
        }
        catch (OperationCanceledException ex)
        {
            return new Result
            {
                Success = false,
                ErrorCode = ErrorCodes.Canceled,
                Message = ex.Message
            };
        }
        catch (Exception ex)
        {
            return await Task.FromResult(new Result
            {
                Success = false,
                ErrorCode = ErrorCodes.InternalException,
                Message = ex.Message
            });
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
