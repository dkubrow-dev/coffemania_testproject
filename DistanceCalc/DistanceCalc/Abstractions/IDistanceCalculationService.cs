using DistanceCalc.Models;

namespace DistanceCalc.Abstractions;

/// <summary>
/// Сервис, предоставляющий расчёт расстояния по входным данным
/// </summary>
public interface IDistanceCalculationService
{
    /// <summary>
    /// (асинхронно) Рассчитывает расстояние между двумя точками и возвращает результат
    /// </summary>
    /// <param name="data">Входные данные для расчёта</param>
    /// <param name="cancellationToken">Токен отмены асинхронной операции</param>
    /// <returns>Задача с результатом выполнения</returns>
    Task<Result> CalculateAsync(Input data, CancellationToken cancellationToken = default);
}