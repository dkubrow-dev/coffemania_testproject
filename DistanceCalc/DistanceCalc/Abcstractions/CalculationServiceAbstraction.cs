using DistanceCalc.Models;

namespace DistanceCalc.Abcstractions;

/// <summary>
/// Базовый класс, предоставляющий для всех сервисов основной функционал: реализацию интерфейса IDistanceCalculationService
/// счётчик, а также обёртку для получения и выдачи данных.
/// </summary>
/// <remarks>Для дальнейших реализация будет унифицировано поведение разных реализаций с вызывающим кодом</remarks>
internal class CalculationServiceAbstraction : IDistanceCalculationService
{
    /// <summary>
    /// Общий ресурс: счётчик вызовов
    /// </summary>
    private static ulong _callId = 0;
    private static object _callLock = new();

    /// <summary>
    /// Возвращает растояние между двумя точками по прямой линии, соединяющей их наплоскости.
    /// </summary>
    /// <param name="data">Входные данные</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <remarks>Генерируемые исключения ложатся во внутренний объект. Не выбрасывает исключения вверх</remarks>
    /// <returns>Задачу с результирующим объектом.</returns>
    public async Task<Result> CalculateAsync(Input data, CancellationToken cancellationToken = default)
    {
        // это метод можно будет вынести в абстрактный класс, если будут другие реализации

        // блокируем общий ресурс на время обновления и получения нового ID вызова (просто, топорно, но очень быстрая операция)
        ulong callId;
        lock (_callLock)
        {
            callId = ++_callId;
        }
        DateTime start = DateTime.Now; // время старта операции

        try
        {
            return new Result
            {
                Success = true,
                Distance = await GetDistanceAsync(data.PointA, data.PointB, cancellationToken),
                Message = $"Thread {callId}. Started: {start:O}. Duration: {(DateTime.Now - start).TotalMilliseconds} ms"
            };
        }
        catch (OperationCanceledException ex)
        {
            return new Result
            {
                Success = false,
                Message = $"Thread {callId}. Request canceled. started: {start:O}. Duration: {(DateTime.Now - start).TotalMilliseconds} ms",
                Exception = ex
            };
        }
        catch (Exception ex)
        {
            return await Task.FromResult(new Result
            {
                Success = false,
                Message = $"Thread {callId}. Internal service Error. Duration: {(DateTime.Now - start).TotalMilliseconds} ms. See result object exception for more information.",
                Exception = ex
            });
        }
    }

    /// <summary>
    /// Метод фактического рассчёта расстояния для конкретной реализации.
    /// </summary>
    /// <remarks>ТРЕБУЕТСЯ ПЕРЕОПРЕДЕЛЕНИЕ! При вызове генерирует исключение</remarks>
    /// <param name="A">Точка А</param>
    /// <param name="B">Точка Б</param>
    /// <param name="cancellationToken">Токен отмены работы запроса на расчёт</param>
    /// <returns>Расстояние в километрах от точки А до точки Б</returns>
    /// <exception cref="NotImplementedException">ВСегда требуется перегрузка - в абстракции не работает</exception>
    protected virtual async Task<double> GetDistanceAsync(Point2D A, Point2D B, CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"You need to override virtual method {nameof(GetDistanceAsync)}.");
    }
}
