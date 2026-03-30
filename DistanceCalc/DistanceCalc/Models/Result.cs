namespace DistanceCalc.Models;

/// <summary>
/// Структура выдачи информации на запрос
/// </summary>
/// <remarks>Позволяет вызывающему коду получить стандартизированный вывод информации</remarks>
[Serializable]
public sealed record Result
{
    /// <summary>
    /// Флаг успешного расчёта расстояния
    /// </summary>
    public bool Success { get; internal set; }

    /// <summary>
    /// Раcстояние между указанными точками
    /// </summary>
    public double Distance { get; internal set; }

    /// <summary>
    /// Код внутренней ошибки.
    /// </summary>
    /// <remarks>0 "NoErrors" - всё хорошо. Любое иное значение - ошибка: см. логи</remarks>
    public ErrorCodes ErrorCode { get; internal set; }

    /// <summary>
    /// Сообщение (если требуется) о результате выполнения состояния
    /// </summary>
    /// <remarks>Для вывода ошибок и иных состояний выполнения запроса</remarks>
    public string? Message { get; internal set; }

}
