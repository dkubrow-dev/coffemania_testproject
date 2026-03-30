namespace DistanceCalc.Services;

/// <summary>
/// Настройки сервиса. 
/// </summary>
/// <remarks>
/// Сейчас крайне топорная - для реализации фабрики и будущего развития.
/// Проблема этой реализации - вшитость в код, то есть буквально уход от смысла реализации идеи "настроек".
/// Потом можно заменить на ini-файл или что-то иное типа БД, либо текущего состояния в ОЗУ, задаваемого внешними командами
/// </remarks>
internal static class Settings
{
    /// <summary>
    /// Значения текущих настроек
    /// </summary>
    private static readonly Dictionary<string, string> Values = new()
    {
        {"CalculationServiceMode", "DirectLine" }
    };

    /// <summary>
    /// Возвращает строковое значение текущей настройки. Вернёт пустую строку, если такой настройки нет.
    /// </summary>
    /// <param name="key">Наименование настройки / ключ словаря</param>
    /// <returns>Текущее строковое значение настройки</returns>
    internal static string GetValue(string key) => Values.TryGetValue(key, out string? value)
        ? (value ?? string.Empty)
        : string.Empty;
}
