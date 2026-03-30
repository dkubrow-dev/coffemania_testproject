namespace DistanceCalc.Models;

/// <summary>
/// Структура выдачи информации на запрос
/// </summary>
/// <remarks>Позволяет вызывающему коду получить стандартизированный вывод информации</remarks>
[Serializable]
public struct Result
{
    /// <summary>
    /// Флаг успешного расчёта расстояния
    /// </summary>
    public bool Success { get; internal set; }

    /// <summary>
    /// Сообщение (если требуется) о результате выполнения состояния
    /// </summary>
    /// <remarks>Для вывода ошибок и иных состояний выполнения запроса</remarks>
    public string? Message { get; internal set; }

    /// <summary>
    /// Информация о произошедшей ошибке (если произошла)
    /// </summary>
    public Exception? Exception { get; internal set; }

    /// <summary>
    /// Растояние между указанными точками
    /// </summary>
    public double Distance { get; internal set; }
}
