using DistanceCalc.Models;

namespace DistanceCalc.Abstractions;

/// <summary>
/// Интерфейс провайдера настроек для выполнения расчёта расстояний
/// </summary>
/// <remarks>
/// Реализовать сервис должен вызывающий код, чтобы предоставить настройки сервису расчёта
/// </remarks>
public interface ISettingsProvider
{
    /// <summary>
    /// Режим запуска калькулятора: определяет, какой фактический исполнитель будет искать расстояние
    /// </summary>
    public CalculatorServiceModes Mode { get; }

    /// <summary>
    /// Режим симуляции долгого выполнения: определяет задержку, в расчёте расстояния
    /// </summary>
    /// <remarks>В фактическом сервисе не требуется, отладочный механизм</remarks>
    public int SimulateDelayMs { get; }
}
