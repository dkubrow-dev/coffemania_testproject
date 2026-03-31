namespace DistanceCalc.Models;

/// <summary>
/// Коды внутренних ошибок для передачи клиенту в случае возникновения
/// </summary>
public enum ErrorCodes
{
    /// <summary>
    /// Нет ошибок - всё хорошо
    /// </summary>
    NoErrors = 0,

    /// <summary>
    /// Запрос был прерван вызывающим кодом
    /// </summary>
    Canceled = 1,

    /// <summary>
    /// Произошло внутреннее исключение
    /// </summary>
    InternalException = 2
}
